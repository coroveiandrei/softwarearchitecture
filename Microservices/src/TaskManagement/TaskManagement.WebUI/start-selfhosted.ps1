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
    --app-id taskmanagementservice `
    --app-port 7000 `
    --dapr-http-port 7300 `
    --config $configFile `
    --resources-path ../../dapr/components `
    dotnet run