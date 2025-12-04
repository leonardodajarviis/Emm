# Giai đoạn 1: Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy các file csproj theo đúng cấu trúc thư mục
COPY ["src/Emm.Shared/Emm.Shared.csproj", "src/Emm.Shared/"]
COPY ["src/Emm.Domain/Emm.Domain.csproj", "src/Emm.Domain/"]
COPY ["src/Emm.Application/Emm.Application.csproj", "src/Emm.Application/"]
COPY ["src/Emm.Infrastructure/Emm.Infrastructure.csproj", "src/Emm.Infrastructure/"]
COPY ["src/Emm.Presentation/Emm.Presentation.csproj", "src/Emm.Presentation/"]

# Restore các package dependencies cho Presentation project
RUN dotnet restore "src/Emm.Presentation/Emm.Presentation.csproj"

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
ENV ASPNETCORE_URLS=http://+:2310
ENV ASPNETCORE_ENVIRONMENT=Production

# Entry point
ENTRYPOINT ["dotnet", "Emm.Presentation.dll"]
