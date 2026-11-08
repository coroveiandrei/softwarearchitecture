$container = docker ps -aq --filter "name=^/dtc-rabbitmq$"

if ($container) {
    docker start dtc-rabbitmq
}
else {
    docker run -d -p 5672:5672 -p 15672:15672 --name dtc-rabbitmq rabbitmq:3-management-alpine
}
