$container = docker ps -aq --filter "name=^/sqlserver$"

if ($container) {
    docker start sqlserver
}
else {
    docker pull mcr.microsoft.com/mssql/server:2022-latest
    docker run -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=P@ssw0rd98763123" -p 1433:1433 --name sqlserver -d mcr.microsoft.com/mssql/server:2022-latest
}
