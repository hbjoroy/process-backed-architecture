/* eslint-disable import/no-anonymous-default-export */
import React from 'react';
import { useState } from 'react';
import Payments from './Payments.jsx';
export default () => 

    {

        const [login, setLogin] = useState(null);
        const [user, setUser] = useState(null);

        if (login===null) 
            return (
                <>
                    <header>
                    <h1>Banking</h1>
                    </header>
                    <p>Please login</p>
                    <input type="text" onChange={(e) => setUser(e.target.value)} />
                    <button onClick={() => setLogin(user)}>Login</button>
                </>
            )        
        else
            return (
            <>
                <header>
                    <div className="Title"><h1>Banking</h1></div>
                    <div className="User">Logged in as {login}</div>
                </header>
                <Payments login = {login}/>
            </>    
        )
    }
