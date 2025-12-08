using Emm.Application.Abstractions;
using Emm.Domain.Abstractions;
using Emm.Domain.Entities.AssetCatalog;
using Emm.Infrastructure.Data;
using Emm.Infrastructure.Data.Interceptors;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Emm.Infrastructure.Tests.Interceptors;

public class AuditableEntityInterceptorTests
{
    private class TestOutbox : IOutbox
    {
        public void Enqueue(IDomainEvent @event) { }
        public void EnqueueRange(IEnumerable<IDomainEvent> events) { }
        public Task PublishImmediateAsync(IDomainEvent @event, CancellationToken cancellationToken = default) => Task.CompletedTask;
        public Task PublishImmediateRangeAsync(IEnumerable<IDomainEvent> events, CancellationToken cancellationToken = default) => Task.CompletedTask;
    }

    private XDbContext CreateInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<XDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        // Create a service provider with scope factory for the interceptor
        var services = new ServiceCollection();
        services.AddScoped<IOutbox, TestOutbox>();
        var serviceProvider = services.BuildServiceProvider();

        var auditableInterceptor = new AuditableEntityInterceptor();
        var domainEventInterceptor = new DomainEventInterceptor(serviceProvider.GetRequiredService<IServiceScopeFactory>());

        return new XDbContext(options, auditableInterceptor, domainEventInterceptor);
    }

    [Fact]
    public async Task WhenAddingEntity_ShouldSetCreatedAtAndUpdatedAt()
    {
        // Arrange
        await using var context = CreateInMemoryDbContext();
        var category = new AssetCategory("CAT001",false, "Test Category", "Test Description");

        // Assert initial state - timestamps should be default
        Assert.Equal(default, category.CreatedAt);
        Assert.Equal(default, category.UpdatedAt);

        // Act
        context.Add(category);
        await context.SaveChangesAsync();

        // Assert - timestamps should be set by interceptor
        Assert.NotEqual(default, category.CreatedAt);
        Assert.NotEqual(default, category.UpdatedAt);
        Assert.True(category.CreatedAt > DateTime.UtcNow.AddSeconds(-5));
        Assert.True(category.UpdatedAt > DateTime.UtcNow.AddSeconds(-5));
        Assert.Equal(category.CreatedAt, category.UpdatedAt);
    }

    [Fact]
    public async Task WhenUpdatingEntity_ShouldUpdateUpdatedAtOnly()
    {
        // Arrange
        await using var context = CreateInMemoryDbContext();
        var category = new AssetCategory("CAT002",false, "Test Category 2", "Description");

        context.Add(category);
        await context.SaveChangesAsync();

        var originalCreatedAt = category.CreatedAt;
        var originalUpdatedAt = category.UpdatedAt;

        // Wait a bit to ensure time difference
        await Task.Delay(100);

        // Act
        category.Update("Updated Name", "Updated Description", true);
        await context.SaveChangesAsync();

        // Assert
        Assert.Equal(originalCreatedAt, category.CreatedAt); // CreatedAt should NOT change
        Assert.NotEqual(originalUpdatedAt, category.UpdatedAt); // UpdatedAt should change
        Assert.True(category.UpdatedAt > originalUpdatedAt);
    }

    [Fact]
    public async Task WhenAddingMultipleEntities_ShouldSetTimestampsForAll()
    {
        // Arrange
        await using var context = CreateInMemoryDbContext();
        var category1 = new AssetCategory("CAT003", false, "Category 1", "Desc 1");
        var category2 = new AssetCategory("CAT004", false, "Category 2", "Desc 2");
        var category3 = new AssetCategory("CAT005", false, "Category 3", "Desc 3");

        // Act
        context.AddRange(category1, category2, category3);
        await context.SaveChangesAsync();

        // Assert
        Assert.NotEqual(default, category1.CreatedAt);
        Assert.NotEqual(default, category2.CreatedAt);
        Assert.NotEqual(default, category3.CreatedAt);
    }
}
