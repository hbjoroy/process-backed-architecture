# Importer Invoke-RestMethod cmdlet
Import-Module Microsoft.PowerShell.Utility

# Opprett en liste med profiler som skal settes inn
$profiles = @(
  @{UserId = "uola"; PsuId = "psu-ola"; Name = "Ola"; Accounts = @("1234", "5678")},
  @{UserId = "uhans"; PsuId = "psu-hans"; Name = "Hans"; Accounts = @("4321", "8765")},
  @{UserId = "ugrete"; PsuId = "psu-grete"; Name = "Grete"; Accounts = @("1111", "2222")}
)

# Loop gjennom profilene og kall POST http://localhost:5282/profile med hver profil som json body
foreach ($profile in $profiles) {
  # Konverter profilen til json
  $json = $profile | ConvertTo-Json

  # Kall POST http://localhost:5282/profile med json body og content-type header
  $response = Invoke-RestMethod -Uri "http://localhost:5282/profile" -Method Post -Body $json -ContentType "application/json"

  # Skriv ut statuskode og innhold av responsen
  Write-Host "Statuskode: $($response.StatusCode)"
  Write-Host "Innhold: $($response.Content)"
}
