import React, { useState, useEffect } from "react";
import PaymentForm from "./PaymentForm.jsx";

function fetchData() {
  return fetch("/api/payments")
    .then((response) => response.json())
    .catch((error) => console.error(error));
}

function PaymentList() {
  const [data, setData] = useState([]);
  const [isPaymentSent, setIsPaymentSent] = useState(false);
  
  useEffect(() => {
    fetchData().then((data) => setData(data));
  }, []);

  useEffect(() => {
    if (isPaymentSent) {
      // Fetch the updated list of payments
      fetchData().then((data) => setData(data));
      // Reset the isPaymentSent state to false
      setIsPaymentSent(false);
    }
  }, [isPaymentSent]);

  const listItems = data.map((item) => (
    <li key={item.paymentID}>
      {item.paymentRequest.amount} {item.paymentRequest.currency} from{" "}
      {item.paymentRequest.fromAccount} to {item.paymentRequest.toAccount} status {item.transactionStatus}
    </li>
  ));

  return (
    <div>
      <h1>Payments</h1>
      <ul>{listItems}</ul>
      <PaymentForm setIsPaymentSent={setIsPaymentSent} />
      <button onClick={() => setIsPaymentSent(true)}>Update Payments</button>
    </div>
  );
}

export default PaymentList;