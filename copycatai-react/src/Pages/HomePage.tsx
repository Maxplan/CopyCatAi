import React, { useEffect } from "react";
import ChatWindow from "../Components/HomePage/ChatWindow";
import Sidebar from "../Components/HomePage/SideBar";

import "../Styles/homePage.css";

const HomePage: React.FC = () => {
  const [userId, setUserId] = React.useState<string | null>(null);
  const authToken = localStorage.getItem('token');

  useEffect(() => {
    const fetchUserId = async () => {
      const response = await fetch('http://localhost:5119/api/v1/User/GetCurrentUser', {
        method: 'GET',
        headers: {
          'Authorization': `Bearer ${authToken}`
        }
      });
      if (response.ok) {
        const data = await response.json();
        setUserId(data.id);
      }
    };

    if (authToken) {
      fetchUserId();
    }
  }, [authToken]);

  return (
    <div className="app-container">
      {authToken && userId ? (
        <>
          <Sidebar authToken={authToken} userId={userId as string} />
          <ChatWindow />
        </>
      ) : (
        <p>Loading...</p>  // Or any other loading indicator
      )}
    </div>
  );
};

export default HomePage;