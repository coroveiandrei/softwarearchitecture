$ErrorActionPreference = "Stop"
& (Join-Path $PSScriptRoot "start-localdb.ps1")
if ($LASTEXITCODE -ne 0) { throw "SQL Server startup failed." }
& (Join-Path $PSScriptRoot "start-rabbitmq.ps1")
if ($LASTEXITCODE -ne 0) { throw "RabbitMQ startup failed." }
& (Join-Path $PSScriptRoot "start-azurite.ps1")
if ($LASTEXITCODE -ne 0) { throw "Azurite startup failed." }
