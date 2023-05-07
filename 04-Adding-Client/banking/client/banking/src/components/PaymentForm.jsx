import React, { useState } from "react";

function PaymentForm({ setIsPaymentSent }) {
  // Define state variables for each input field
  const [amount, setAmount] = useState(0);
  const [currency, setCurrency] = useState("NOK");
  const [fromAccount, setFromAccount] = useState("");
  const [toAccount, setToAccount] = useState("");
  const [creditorName, setCreditorName] = useState("");

  // Define a function to handle form submission
  const handleSubmit = (event) => {
    // Prevent default browser behavior
    event.preventDefault();

    // Create a payment request object from the state variables
    const paymentRequest = {
      Amount: amount,
      Currency: currency,
      FromAccount: fromAccount,
      ToAccount: toAccount,
      CreditorName: creditorName,
    };

    // Convert the payment request object to JSON
    const jsonRequest = JSON.stringify(paymentRequest);

    // Use fetch() to send a POST request to /api/payments with the JSON data
    fetch("/api/payments", {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      body: jsonRequest,
    })
      .then((response) => response.json())
      .then((data) => {
        // Handle the response data
        console.log(data);
        setIsPaymentSent(true);
      })
      .catch((error) => {
        // Handle any errors
        console.error(error);
      });
  };

  return (
    <form onSubmit={handleSubmit}>
      <label htmlFor="amount">Amount:</label>
      <input
        type="number"
        id="amount"
        value={amount}
        onChange={(e) => setAmount(e.target.value)}
      />
      <label htmlFor="currency">Currency:</label>
      <select
        id="currency"
        value={currency}
        onChange={(e) => setCurrency(e.target.value)}
      >
        <option value="NOK">NOK</option>
        <option value="USD">USD</option>
        <option value="EUR">EUR</option>
      </select>
      <label htmlFor="fromAccount">From account:</label>
      <input
        type="text"
        id="fromAccount"
        value={fromAccount}
        onChange={(e) => setFromAccount(e.target.value)}
      />
      <label htmlFor="toAccount">To account:</label>
      <input
        type="text"
        id="toAccount"
        value={toAccount}
        onChange={(e) => setToAccount(e.target.value)}
      />
      <label htmlFor="creditorName">Creditor name:</label>
      <input
        type="text"
        id="creditorName"
        value={creditorName}
        onChange={(e) => setCreditorName(e.target.value)}
      />
      <button type="submit">Send payment</button>
    </form>
  );
}

export default PaymentForm;