foreach ($containerName in @("sqlserver", "dtc-rabbitmq", "dtc-azurite")) {
    $container = docker ps -q --filter "name=^/$containerName$"
    if ($container) {
        docker stop $containerName
    }
}
