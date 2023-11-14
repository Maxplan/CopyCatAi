import React, { useState, useEffect } from "react";
import axios from "axios";
import "../../Styles/header.css";

const Header = () => {
  const [username, setUsername] = useState("Loading...");

  useEffect(() => {
    const fetchCurrentUser = async () => {
      try {
        const response = await axios.get('http://localhost:5119/api/v1/User/getCurrentUser', {
          headers: {
            // Include your authentication token here if needed
            'Authorization': `Bearer ${localStorage.getItem('token')}`
          }
        });
        setUsername(response.data.UserName);
      } catch (error) {
        console.error("Error fetching user data:", error);
        setUsername("User not found");
      }
    };

    fetchCurrentUser();
  }, []);

  return (
    <div className="header">
      <h1 className="logo"><a href="/">CopyCat AI</a></h1>
      <h2 className="user"><span>{username}</span></h2>
    </div>
  );
};

export default Header;
