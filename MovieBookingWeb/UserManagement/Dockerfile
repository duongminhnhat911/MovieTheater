# -------- BUILD STAGE --------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy file .csproj và restore package
COPY *.csproj ./
RUN dotnet restore

# Copy toàn bộ source code và publish
COPY . ./
RUN dotnet publish -c Release -o /app/publish

# -------- RUNTIME STAGE --------
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Copy từ build stage
COPY --from=build /app/publish .

# Mở cổng mặc định
EXPOSE 80

# Bảo đảm ứng dụng lắng nghe cổng đúng
ENV ASPNETCORE_URLS=http://+:80
ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

# Entry point khởi chạy ứng dụng
ENTRYPOINT ["dotnet", "UserManagement.dll"]
