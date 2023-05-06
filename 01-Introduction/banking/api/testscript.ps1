# Define the base URL for the webapi
$baseURL = "http://localhost:5158/v1"

# Define a sample payment request object
$paymentRequest = @{
    Amount = 100.00
    Currency = "NOK"
    DebtorAccount = "1234567890"
    CreditorAccount = "0987654321"
}

# Convert the payment request object to JSON
$jsonRequest = $paymentRequest | ConvertTo-Json

# Define the payment service and product parameters
$paymentService = "payments"
$paymentProduct = "sepa-credit-transfers"

Write-Output "$baseURL/$paymentService/$paymentProduct"

# Invoke the webapi POST method with the JSON request and the parameters
$response = Invoke-RestMethod -Uri "$baseURL/$paymentService/$paymentProduct" -Method Post -Body $jsonRequest -ContentType "application/json"

# Display the response from the webapi
Write-Output "Post $response"
$paymentId = $response.PaymentId
$response = Invoke-RestMethod -Uri "$baseURL/$paymentService/$paymentProduct/$paymentId" -Method Get -ContentType "application/json"

# Display the response from the webapi
Write-Output "Get $response"
