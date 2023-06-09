/* eslint-disable import/no-anonymous-default-export */
import { useState } from "react";

export default ({setShowRegisterProfile, userIdIn, doLogin}) => {
    let [userId, setUserId] = useState(userIdIn);
    let [name, setName] = useState("");
    let [psuId, setPsuId] = useState("");

    const handleSubmit = (event) => {
        event.preventDefault();
        fetch("/api/profile", {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({ userId, name, psuId }),
        })
            .then((response) => {
                if (response.ok) {
                    return response.json();
                } else {
                    throw response;
                }
            })
            .then((profile) => {
                console.log(profile);
                setUserId(profile.userId);
                setName(profile.name);
                setPsuId(profile.psuId);
                setShowRegisterProfile(false);
                doLogin(profile.userId);
            })
            .catch((error) => {
                console.error(error);
            });
    }

    return (
        <>
            <p>Register profile</p>
            <form className="registerForm" onSubmit={handleSubmit}>
                <div>
                <label htmlFor="userId">User ID:</label>
                <input
                    type="text"
                    id="userId"
                    value={userId}
                    onChange={(e) => setUserId(e.target.value)}
                />
                </div>
                <div>
                <label htmlFor="name">Name:</label>
                <input
                    type="text"
                    id="name"
                    value={name}
                    onChange={(e) => setName(e.target.value)}
                />
                </div>
                <div>
                <label htmlFor="psuId">PSU ID:</label>
                <input
                    type="text"
                    id="psuId"
                    value={psuId}
                    onChange={(e) => setPsuId(e.target.value)}
                />
                </div>
                <div className="registerButtons">
                    <button type="submit">Register</button>
                    <button onClick={() => setShowRegisterProfile(false)}>Cancel</button>
                </div>
            </form>
        </>
    );

}