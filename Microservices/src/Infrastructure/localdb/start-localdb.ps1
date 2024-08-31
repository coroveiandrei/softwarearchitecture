docker pull mcr.microsoft.com/mssql/server:2022-latest
docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=P@ssw0rd98763123" -p 1433:1433 --name sqlserver -d mcr.microsoft.com/mssql/server:2022-latest
