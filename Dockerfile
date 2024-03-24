# # Use the .NET 6.0 SDK Docker image as the base image
# FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env
# WORKDIR /app

# # Copy csproj and restore dependencies
# # COPY ./Repositories/*.csproj ./
# # COPY ./Services/*.csproj ./
# COPY ./WebAPI/*.csproj ./
# RUN dotnet restore

# # Copy everything else and build
# COPY . ./
# RUN dotnet publish -c Release -o out

# # Use the .NET 6.0 runtime Docker image as the base image
# FROM mcr.microsoft.com/dotnet/aspnet:6.0
# WORKDIR /app
# COPY --from=build-env /app/out .

# # Expose port 80 for the application
# EXPOSE 7296
# ENTRYPOINT ["dotnet", "WebAPI.dll"]


FROM mcr.microsoft.com/dotnet/sdk:6.0-focal AS build
WORKDIR /source

# Copy everything
COPY . .
RUN dotnet restore "./MomoPayment/MomoPayment.API.csproj" --disable-parallel
RUN dotnet build "./MomoPayment/MomoPayment.API.csproj" -c release -o /app --no-restore

RUN dotnet restore "./MomoPayment.Repositories/MomoPayment.Repositories.csproj" --disable-parallel
RUN dotnet build "./MomoPayment.Repositories/MomoPayment.Repositories.csproj" -c release -o /app --no-restore

RUN dotnet restore "./MomoPayment.Services/MomoPayment.Services.csproj" --disable-parallel
RUN dotnet build "./MomoPayment.Services/MomoPayment.Services.csproj" -c release -o /app --no-restore

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:6.0-focal 
WORKDIR /app
COPY --from=build /app ./
EXPOSE 7291
ENTRYPOINT ["dotnet", "MomoPayment.API.dll"]