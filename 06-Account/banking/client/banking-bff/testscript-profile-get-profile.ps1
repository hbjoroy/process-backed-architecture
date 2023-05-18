# Importer Invoke-WebRequest cmdlet
Import-Module Microsoft.PowerShell.Utility

# Opprett en header med x-user-id
$header = @{"x-user-id" = "uola"}

# Kall GET http://localhost:5282/profile med headeren
$response = Invoke-WebRequest -Uri "http://localhost:5282/profile" -Method Get -Headers $header

# Skriv ut statuskode og innhold av responsen
Write-Host "Statuskode: $($response.StatusCode)"
Write-Host "Innhold: $($response.Content)"
