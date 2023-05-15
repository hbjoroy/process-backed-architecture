import React, { useState, useEffect } from "react";
import PaymentItem from "./PaymentItem.jsx";
import PaymentForm from "./PaymentForm.jsx";

async function fetchData(login) {
  console.log("fetchData: " + login);
  return fetch("/api/payments", {
    headers: {  
      "x-user-id": login,
    }
  })
    .then((response) => response.json());
}

function Payments( {login} ) {
  const [data, setData] = useState([]);
  const [isPaymentSent, setIsPaymentSent] = useState(true);
  const [time, setTime] = useState(0);
  console.log("Payments: " + login);
  useEffect(() => {
    if (isPaymentSent) {
      // Fetch the updated list of payments
      fetchData(login)
        .then((data) => setData(data))
        .catch((error) => console.error("Anonymous function in useEffect: " + error));
      // Reset the isPaymentSent state to false
      setIsPaymentSent(false);
    }
  }, [isPaymentSent,login]);

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
        <PaymentItem item = { item } />
    </li>
  ));

  if (data.length === 0) {
    return (
      <>
        <h1>Payments</h1>
        <PaymentForm login={login} setIsPaymentSent={setIsPaymentSent} />
        <p>No payments loaded</p>
      </>
    )
  }
  else {
    return (
      <>
        <h1>Payments</h1>
        <PaymentForm login={login} setIsPaymentSent={setIsPaymentSent} />
          <h3>Sent transactions:</h3>
          <ul className="transactions">{listItems}</ul>
         {/* <button className="bankingButton" onClick={() => setIsPaymentSent(true)}>Update Payments</button> */} 
      
      </> 
    );

  }
}

export default Payments;