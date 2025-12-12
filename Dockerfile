# Giai đoạn 1: Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy TẤT CẢ các file csproj để dotnet restore có thể tìm thấy các dependency
# Cần phải tạo cấu trúc thư mục đúng:
COPY ["src/Emm.Shared/Emm.Shared.csproj", "src/Emm.Shared/"]
COPY ["src/Emm.Domain/Emm.Domain.csproj", "src/Emm.Domain/"]
COPY ["src/Emm.Application/Emm.Application.csproj", "src/Emm.Application/"]
COPY ["src/Emm.Infrastructure/Emm.Infrastructure.csproj", "src/Emm.Infrastructure/"]
COPY ["src/Emm.Presentation/Emm.Presentation.csproj", "src/Emm.Presentation/"]

# Restore các package dependencies cho Presentation project
RUN dotnet restore "src/Emm.Presentation/Emm.Presentation.csproj"

# Copy toàn bộ source code còn lại (bao gồm các file code và wwwroot)
COPY . .

# Build và Publish bản Release
# Trở lại thư mục gốc /src để dễ quản lý đường dẫn
WORKDIR /src
RUN dotnet publish "src/Emm.Presentation/Emm.Presentation.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Giai đoạn 2: Runtime (Final Image)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

# Khai báo port mặc định mà ứng dụng lắng nghe
EXPOSE 2310

# Sử dụng user non-root để tăng cường bảo mật
USER $APP_UID

# Copy file đã publish từ giai đoạn build
COPY --from=build /app/publish .

# Cấu hình biến môi trường
ENV ASPNETCORE_URLS=http://+:2310
ENV ASPNETCORE_ENVIRONMENT=Production

# Entry point
ENTRYPOINT ["dotnet", "Emm.Presentation.dll"]
