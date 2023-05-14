import React, { useState, useEffect } from "react";
import PaymentItem from "./PaymentItem.jsx";
import PaymentForm from "./PaymentForm.jsx";

function fetchData() {
  return fetch("/api/payments")
    .then((response) => response.json());
}

function PaymentList() {
  const [data, setData] = useState([]);
  const [isPaymentSent, setIsPaymentSent] = useState(true);
  const [time, setTime] = useState(0);

  useEffect(() => {
    if (isPaymentSent) {
      // Fetch the updated list of payments
      fetchData()
        .then((data) => setData(data))
        .catch((error) => console.error("Anonymous function in useEffect: " + error));
      // Reset the isPaymentSent state to false
      setIsPaymentSent(false);
    }
  }, [isPaymentSent]);

  useEffect(() => {    
    function hasUnfinishedPayments() {
      const finishedStatuses = ["ACCC", "RJCT"];
      return data.filter((item) => !finishedStatuses.includes(item.transactionStatus)).count>0;
    }
      function update() {
      if (hasUnfinishedPayments())
        setIsPaymentSent(true); // Force update of payments
      setTime(time + 1);
    }

    const intervalId = setInterval(update, 1000);
    return () => {
      clearInterval(intervalId);
    };    
  }, [time, data]);

  const listItems = data.map((item) => (
    <li key={item.paymentID}>
        <PaymentItem item= { item } />
    </li>
  ));

  if (data.length === 0) {
    return (
      <>
        <p>No payments loaded</p>
      </>
    )
  }
  else {
    return (
      <>
        <h1>Payments</h1>
        <PaymentForm setIsPaymentSent={setIsPaymentSent} />
          <h3>Sent transactions:</h3>
          <ul className="transactions">{listItems}</ul>
         {/* <button className="bankingButton" onClick={() => setIsPaymentSent(true)}>Update Payments</button> */} 
      
      </> 
    );

  }
}

export default PaymentList;