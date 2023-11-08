import React from "react";

const HomePageActions: React.FC = () => {
    return (
        <div>
            <ul>
                <li><a href="/login">Log In</a></li>
                <li><a href="/register">Register</a></li>
                <li><a href="/Interaction">Continue As Guest</a></li>
            </ul>
        </div>
    );
}

export default HomePageActions;