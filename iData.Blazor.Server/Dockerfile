#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
RUN apt-get update && apt-get upgrade -y && apt-get install -y \
    libfontconfig1 \
    wget \
    unzip \   
    && apt-get clean && rm -rf /var/lib/apt/lists/* 
   
# Install Terraform
RUN wget https://releases.hashicorp.com/terraform/1.9.4/terraform_1.9.4_linux_amd64.zip
RUN unzip terraform_1.9.4_linux_amd64.zip -d /usr/local/bin/
RUN rm terraform_1.9.4_linux_amd64.zip

WORKDIR /app
EXPOSE 80
EXPOSE 443


FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
RUN apt-get update && rm -rf /var/lib/apt/lists/* 
RUN apt-get update && apt-get upgrade -y && apt-get clean && rm -rf /var/lib/apt/lists/* 

RUN ls -R /src

COPY ["iData.Blazor.Server/iData.Blazor.Server.csproj", "iData.Blazor.Server/"]
COPY ["iData.Module/iData.Module.csproj", "iData.Module/"]
COPY ["Roche.Common.Workflow/Roche.Common.Workflow.csproj", "Roche.Common.Workflow/"]
COPY ["Roche.Common/Roche.Common.csproj", "Roche.Common/"]

COPY ./NuGet.Config /nuget.config

COPY _.devexpress.com.crt /usr/local/share/ca-certificates/
RUN update-ca-certificates

RUN dotnet restore "./iData.Blazor.Server/./iData.Blazor.Server.csproj" 

COPY . .
WORKDIR "/src/iData.Blazor.Server"
RUN dotnet build "./iData.Blazor.Server.csproj" -c $BUILD_CONFIGURATION -o /app/build
RUN ls -R /app


FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./iData.Blazor.Server.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV ASPNETCORE_URLS=http://*:80/

ENTRYPOINT ["dotnet", "iData.Blazor.Server.dll"]