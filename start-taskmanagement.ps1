$ErrorActionPreference = "Stop"
Push-Location (Join-Path $PSScriptRoot "Microservices/src/TaskManagement/TaskManagement.WebUI")
try {
    dapr run `
        --app-id taskmanagementservice `
        --app-port 7000 `
        --dapr-http-port 7300 `
        --config ../../dapr/config/config.yaml `
        --resources-path ../../dapr/components `
        dotnet run
}
finally {
    Pop-Location
}
