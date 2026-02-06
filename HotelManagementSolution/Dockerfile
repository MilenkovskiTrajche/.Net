# ----------------------
# 1️⃣ Build stage
# ----------------------
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

# Copy solution and all projects
COPY *.sln ./
COPY HotelManagement.Domain/*.csproj ./HotelManagement.Domain/
COPY HotelManagement.Infrastructure/*.csproj ./HotelManagement.Infrastructure/
COPY HotelManagement.Application/*.csproj ./HotelManagement.Application/
COPY HotelManagement.Web/*.csproj ./HotelManagement.Web/

# Restore solution
RUN dotnet restore

# Copy all files
COPY . .

# Publish the Web project
WORKDIR /app/HotelManagement.Web
RUN dotnet publish -c Release -o out

# ----------------------
# 2️⃣ Runtime stage
# ----------------------
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/HotelManagement.Web/out ./

EXPOSE 8080
ENTRYPOINT ["dotnet", "HotelManagement.Web.dll"]
