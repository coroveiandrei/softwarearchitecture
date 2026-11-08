$ErrorActionPreference = "Stop"
Push-Location (Join-Path $PSScriptRoot "Microservices/src/InventoryService/InventoryService.WebUI")
try {
    dapr run `
        --app-id inventoryservice `
        --app-port 4000 `
        --dapr-http-port 3400 `
        --dapr-grpc-port 40000 `
        --config ../../dapr/config/config.yaml `
        --resources-path ../../dapr/components `
        dotnet run
}
finally {
    Pop-Location
}
