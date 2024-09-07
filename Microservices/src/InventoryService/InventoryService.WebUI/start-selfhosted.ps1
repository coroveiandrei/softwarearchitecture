# specify 'consul' as the first argument to use consul for name resolution
$configFile = if ($Args[0] -eq "consul") 
{  
    "../../dapr/config/consul-config.yaml"
} 
else 
{ 
    "../../dapr/config/config.yaml"
} 

dapr run `
    --app-id inventoryservice `
    --app-port 4000 `
    --dapr-http-port 3400 `
    --dapr-grpc-port 40000 `
    --config $configFile `
    --resources-path ../../dapr/components `
    dotnet run