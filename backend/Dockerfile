FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env
WORKDIR /app

# -------------- Step 1. Copy only required files for nuget restore for better caching
# Copy solution & nuget config file (if exists)
COPY *.sln NuGet*.Config Directory.Packages.props .

# Copy csproj and create folders for projects
COPY Tim.Backend/Tim.Backend.csproj Tim.Backend/Tim.Backend.csproj

# Install nuget packages
RUN dotnet restore Tim.Backend

# -------------- Step 2. Copy all other files
COPY . .

# -------------- Step 3. Build all
RUN dotnet build -c Release --no-restore Tim.Backend

#-------------- Step 4. Build runtime image
RUN dotnet publish Tim.Backend -c Release -o /app/out --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY --from=build-env /app/out .

ENTRYPOINT ["dotnet", "Tim.Backend.dll"]
