namespace Emm.Domain.Services;

/// <summary>
/// Base service cung cấp logic generic để đồng bộ các entity con
/// Thêm mới nếu không có Id (default value), cập nhật nếu có Id, xóa các record có trong DB nhưng không có trong request
/// </summary>
public abstract class CollectionEntitySyncService
{
    /// <summary>
    /// Generic method để đồng bộ các entity con với hỗ trợ nhiều kiểu khóa (TKey)
    /// </summary>
    /// <typeparam name="TEntity">Loại entity cần đồng bộ</typeparam>
    /// <typeparam name="TKey">Loại khóa chính của entity (Guid, int, string, ...)</typeparam>
    /// <param name="existingEntities">Danh sách entities hiện có trong DB</param>
    /// <param name="requestEntities">Danh sách entities từ request</param>
    /// <param name="getId">Function để lấy Id từ entity</param>
    /// <param name="addAction">Action để thêm mới entity</param>
    /// <param name="updateAction">Action để cập nhật entity (có thể null nếu không hỗ trợ update)</param>
    /// <param name="removeAction">Action để xóa entity</param>
    /// <param name="isDefaultKey">Function để kiểm tra xem key có phải giá trị mặc định (vd: Guid.Empty, 0, null, ...)</param>
    protected void SyncEntities<TEntity, TKey>(
        IEnumerable<TEntity> existingEntities,
        IEnumerable<TEntity> requestEntities,
        Func<TEntity, TKey> getId,
        Action<TEntity> addAction,
        Action<TEntity>? updateAction,
        Action<TKey> removeAction,
        Func<TKey, bool>? isDefaultKey = null)
        where TKey : notnull
    {
        // Nếu không cung cấp isDefaultKey, sử dụng EqualityComparer để so sánh với default
        isDefaultKey ??= key => EqualityComparer<TKey>.Default.Equals(key, default!);

        var requestList = requestEntities.ToList();
        var existingList = existingEntities.ToList();

        // Lấy danh sách Id từ request (bỏ qua những cái có Id là default value)
        var requestIds = requestList
            .Where(e => !isDefaultKey(getId(e)))
            .Select(e => getId(e))
            .ToHashSet();

        // Xóa các entities có trong DB nhưng không có trong request
        var entitiesToRemove = existingList
            .Where(existing => !requestIds.Contains(getId(existing)))
            .ToList();

        foreach (var entity in entitiesToRemove)
        {
            removeAction(getId(entity));
        }

        // Xử lý từng entity trong request
        foreach (var requestEntity in requestList)
        {
            var entityId = getId(requestEntity);
            var existsInDb = existingList.Any(e => EqualityComparer<TKey>.Default.Equals(getId(e), entityId));

            // Nếu Id là default value hoặc không tồn tại trong DB => Thêm mới
            if (isDefaultKey(entityId) || !existsInDb)
            {
                addAction(requestEntity);
            }
            // Cập nhật nếu có updateAction và entity đã tồn tại
            else if (updateAction != null)
            {
                updateAction(requestEntity);
            }
        }
    }

    /// <summary>
    /// Overload method để hỗ trợ backward compatibility với Guid (giá trị mặc định là Guid.Empty)
    /// </summary>
    /// <typeparam name="TEntity">Loại entity cần đồng bộ</typeparam>
    /// <param name="existingEntities">Danh sách entities hiện có trong DB</param>
    /// <param name="requestEntities">Danh sách entities từ request</param>
    /// <param name="getId">Function để lấy Id từ entity</param>
    /// <param name="addAction">Action để thêm mới entity</param>
    /// <param name="updateAction">Action để cập nhật entity (có thể null nếu không hỗ trợ update)</param>
    /// <param name="removeAction">Action để xóa entity</param>
    protected void SyncEntities<TEntity>(
        IEnumerable<TEntity> existingEntities,
        IEnumerable<TEntity> requestEntities,
        Func<TEntity, Guid> getId,
        Action<TEntity> addAction,
        Action<TEntity>? updateAction,
        Action<Guid> removeAction)
    {
        SyncEntities(
            existingEntities,
            requestEntities,
            getId,
            addAction,
            updateAction,
            removeAction,
            key => key == Guid.Empty);
    }
}
