import React, { useState, useEffect } from "react";
import PaymentItem from "./PaymentItem.jsx";
import PaymentForm from "./PaymentForm.jsx";

function fetchData() {
  return fetch("/api/payments")
    .then((response) => response.json())
    .catch((error) => console.error(error));
}

function PaymentList() {
  const [data, setData] = useState([]);
  const [isPaymentSent, setIsPaymentSent] = useState(false);
  const [time, setTime] = useState(0);
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
  
  function update() {
    setIsPaymentSent(true);
  }

  useEffect(() => {    
    const intervalId = setInterval(update, 1000);
    return () => {
      clearInterval(intervalId);
    };    
  }, [time]);

  const listItems = data.map((item) => (
    <li key={item.paymentID}>
        <PaymentItem item= { item } />
    </li>
  ));

  return (
    <div>
      <h1>Payments</h1>
      <PaymentForm setIsPaymentSent={setIsPaymentSent} />
      <h3>Sent transactions:</h3>
      <ul class="transactions">{listItems}</ul>
      <button className="bankingButton" onClick={() => setIsPaymentSent(true)}>Update Payments</button>
    </div>
  );
}

export default PaymentList;