import React, { useState } from "react";

function PaymentItem({ item }) {
    const [open, setOpen] = useState(false);

    const toggleOpen = () => setOpen(!open);
    const openLayout = () => (
        <div onClick = { toggleOpen }>
            From: <span className="account">{item.paymentRequest.fromAccount}</span> <br />
            To: <span className="account">{item.paymentRequest.toAccount}</span> ({item.paymentRequest.creditorName})<br />
            Amount: {item.paymentRequest.currency} <span className="amount">{item.paymentRequest.amount}</span> <br />
            Status: <span className="status">{item.transactionStatus}</span> <br />
        </div>        
    );

    const closedLayout = () => (
        <div onClick = { toggleOpen }>
            From: <span className="account">{item.paymentRequest.fromAccount}</span>&nbsp; 
            To: <span className="accountOwner">{item.paymentRequest.creditorName}</span>:&nbsp;
            {item.paymentRequest.currency}&nbsp;<span className="amount">{item.paymentRequest.amount}</span>
            &nbsp;({item.transactionStatus})
        </div>
    );

    return open ? openLayout() : closedLayout();
}

export default PaymentItem;