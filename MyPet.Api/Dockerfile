FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ./*.sln ./
COPY ./*/*.csproj ./
RUN for file in $(ls *.csproj); do mkdir -p ./${file%.*}/ && mv $file ./${file%.*}/; done
RUN dotnet restore ./MyPet.Api.sln
COPY . .
RUN dotnet publish ./MyPet.Api/MyPet.Api.csproj -c Release -o /publish

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /publish
COPY --from=build /publish .
ENV ASPNETCORE_URLS="http://*:5000"
EXPOSE 5000
ENTRYPOINT ["dotnet", "MyPet.Api.dll"]

