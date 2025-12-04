# Giai đoạn 1: Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy file solution
COPY ["Emm.sln", "./"]

# Copy các file csproj theo đúng cấu trúc thư mục
COPY ["src/Emm.Shared/Emm.Shared.csproj", "src/Emm.Shared/"]
COPY ["src/Emm.Domain/Emm.Domain.csproj", "src/Emm.Domain/"]
COPY ["src/Emm.Application/Emm.Application.csproj", "src/Emm.Application/"]
COPY ["src/Emm.Infrastructure/Emm.Infrastructure.csproj", "src/Emm.Infrastructure/"]
COPY ["src/Emm.Presentation/Emm.Presentation.csproj", "src/Emm.Presentation/"]

# Copy tools (nếu cần)
COPY ["tools/Emm.Tools.ErrorCodeGenerator/Emm.Tools.ErrorCodeGenerator.csproj", "tools/Emm.Tools.ErrorCodeGenerator/"]

# Restore các package dependencies
RUN dotnet restore "Emm.sln"

# Copy toàn bộ source code còn lại
COPY . .

# Build và Publish bản Release
WORKDIR "/src/src/Emm.Presentation"
RUN dotnet publish "Emm.Presentation.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Giai đoạn 2: Runtime (Final Image)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

# Tạo user non-root cho bảo mật (tùy chọn, nhưng khuyến nghị trên VPS)
# Ubuntu container mặc định có user 'app' từ .NET 8/9 images
USER $APP_UID

# Copy file đã publish từ giai đoạn build
COPY --from=build /app/publish .

# Cấu hình biến môi trường (có thể override khi chạy container)
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

# Entry point
ENTRYPOINT ["dotnet", "Emm.Presentation.dll"]
