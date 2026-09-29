Push-Location -Path localdb
& ./start-localdb.ps1
Pop-Location

Push-Location -Path rabbitmq
& ./start-rabbitmq.ps1
Pop-Location

Push-Location -Path azurite
& ./start-azurite.ps1
Pop-Location
