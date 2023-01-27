FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env
WORKDIR /app

# -------------- Step 1. Copy only required files for nuget restore for better caching
# 1.1 Copy solution & nuget config file (if exists)
COPY ./* ./
# 1.2 Copy csproj and create folders for projects
COPY ./*/*.csproj ./
# 1.3 Create folders for projects
RUN for file in $(ls *.csproj); do mkdir -p ${file%.*} && mv $file ${file%.*}/; done

RUN dotnet restore
# RUN dotnet restore --disable-parallel

# -------------- Step 2. Copy all other files
COPY . .

# -------------- Step 3. Build all
RUN dotnet build -c Release --no-restore

#-------------- Step 4. Build runtime image
RUN dotnet publish Tim.Backend -c Release -o /app/out --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY --from=build-env /app/out .

ENTRYPOINT ["dotnet", "Tim.Backend.dll"]
