FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build-env

WORKDIR /app
COPY . ./

RUN dotnet restore ./NahareKari.sln 
RUN dotnet publish ./src/NahareKari.API/ -c Release /p:Version=1.0.0 -o out

FROM mcr.microsoft.com/dotnet/aspnet:7.0

WORKDIR /app
COPY --from=build-env /app/out .

ENV TZ=Asia/Tehran
RUN ln -snf /usr/share/zoneinfo/$TZ /etc/localtime && echo $TZ > /etc/timezone
RUN dpkg-reconfigure -f noninteractive tzdata

EXPOSE 80 80/tcp

ENTRYPOINT ["dotnet", "NahareKari.API.dll"]
