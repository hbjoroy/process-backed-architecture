/* eslint-disable import/no-anonymous-default-export */
import React from 'react';
import { useState } from 'react';
import Payments from './Payments.jsx';
import RegisterProfile from './RegisterProfile.jsx';
export default () => 

    {
        const doLogin = (user) => {
            fetch("/api/profile", { headers: { "x-user-id": user } })
                .then((response) =>  {
                    if (response.ok) {
                        return response.json();
                    }   
                    else {
                        return response.json().then((error) => Promise.reject(error));
                    }
                })
                .catch((error) =>  {
                    setLoginMessage(`${error.status} ${error.detail} `)
                    return Promise.resolve(error);
                })
                .then((result) => {
                    if (result.status === 404) {
                        setShowRegisterProfile(true);
                    }
                    else {
                        setProfile(result); 
                        setLogin(user); 
                    }
                }
            )
        }

        const [showRegisterProfile, setShowRegisterProfile] = useState(false);
        const [login, setLogin] = useState(null);
        const [user, setUser] = useState(null);
        const [profile, setProfile] = useState(null);
        const [loginMessage, setLoginMessage] = useState("");

        const renderRegisterProfile = () => {
            if (showRegisterProfile) {
                return (
                    <RegisterProfile setShowRegisterProfile={setShowRegisterProfile} userIdIn={user} doLogin={doLogin} />
                )
            }
        }

        if (login===null) 
            return (
                <>
                    <header>
                    <h1>Banking</h1>
                    </header>
                    <p>Please login</p>
                    <input type="text" onChange={(e) => setUser(e.target.value)} />
                    <button onClick={() => doLogin(user)}>Login</button>
                    <p>{loginMessage}</p>
                    {renderRegisterProfile()}
                </>
            )        
        else 
            return (
            <>
                <header>
                    <div className="Title"><h1>Banking</h1></div>
                    <div className="User">Logged in as {profile.name} ({profile.userId})</div>
                </header>
                <Payments profile = {profile}/>
            </>    
        )
    }
