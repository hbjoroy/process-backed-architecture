# Define the base URL for the webapi
$baseURL = "http://localhost:5282"

# Define a sample payment request object
$paymentRequest = @{
    Amount = 100.00
    Currency = "NOK"
    FromAccount = "1234567890"
    ToAccount = "0987654321"
    CreditorName = "Creditor Name"
}

# Convert the payment request object to JSON
$jsonRequest = $paymentRequest | ConvertTo-Json

# Define the payment service and product parameters
$paymentService = "payments"

Write-Output "Post Req $baseURL/$paymentService"
# Invoke the webapi POST method with the JSON request and the parameters
$response = Invoke-RestMethod -Uri "$baseURL/$paymentService" -Method Post -Body $jsonRequest -ContentType "application/json"

# Display the response from the webapi
Write-Output "Post Res $response"
#$paymentId = $response.PaymentId
$response = Invoke-RestMethod -Uri "$baseURL/$paymentService" -Method Get -ContentType "application/json"

# Display the response from the webapi
Write-Output "Get $response"
