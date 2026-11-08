$ErrorActionPreference = "Stop"
Push-Location (Join-Path $PSScriptRoot "Microservices/src/PlacementService")
try {
    dapr run `
        --app-id stockplacementservice `
        --app-port 6000 `
        --dapr-http-port 3600 `
        --dapr-grpc-port 60000 `
        --config ../dapr/config/config.yaml `
        --resources-path ../dapr/components `
        --log-level debug `
        dotnet run
}
finally {
    Pop-Location
}
