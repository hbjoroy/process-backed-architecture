import React, { useState, useEffect } from "react";
import PaymentItem from "./PaymentItem.jsx";
import PaymentForm from "./PaymentForm.jsx";



function Payments( {profile} ) {
  const [data, setData] = useState([]);
  const [isPaymentSent, setIsPaymentSent] = useState(true);
  const [time, setTime] = useState(0);
  useEffect(() => {
    async function fetchData(login) {
      console.log("fetchData: " + login);
      return fetch("/api/payments", {
        headers: {  
          "x-user-id": login,
        }
      })
    }
    if (isPaymentSent) {
      setIsPaymentSent(false);

      // Fetch the updated list of payments
      fetchData(profile.userId)
      .then(async (data) => {
        if (data.ok) {
          return data.json();
        } else {
          const error = await data.json();
          return await Promise.reject(error);
        }
      })
      .then((data) => setData(data)) 
      .catch((error) => {
        if (error.status === 404) { 
          console.log("No payments found");
          setData([]); 
          Promise.resolve(error);
        }
        else {
          console.error("Not 404" +JSON.stringify(error));
        } 
      });
      // Reset the isPaymentSent state to false
    }
  }, [isPaymentSent,profile]);

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
        <PaymentForm login={profile.userId} setIsPaymentSent={setIsPaymentSent} />
        <p>No payments loaded</p>
      </>
    )
  }
  else {
    return (
      <>
        <h1>Payments</h1>
        <PaymentForm login={profile.userId} setIsPaymentSent={setIsPaymentSent} />
          <h3>Sent transactions:</h3>
          <ul className="transactions">{listItems}</ul>
         {/* <button className="bankingButton" onClick={() => setIsPaymentSent(true)}>Update Payments</button> */} 
      
      </> 
    );

  }
}

export default Payments;