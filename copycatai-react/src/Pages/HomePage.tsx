import React, { useEffect } from "react";
import ChatWindow from "../Components/HomePage/ChatWindow";
import Sidebar from "../Components/HomePage/SideBar";

import "../Styles/homePage.css";

const HomePage: React.FC = () => {
  const [userId, setUserId] = React.useState<string | null>(null);
  const [selectedConversation, setSelectedConversation] = React.useState(null);
  const authToken = localStorage.getItem('token') || "";

  useEffect(() => {
    const fetchUserId = async () => {
      if (authToken){
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
    }
    fetchUserId();
  }, [authToken]);

  const handleStartNewConversation = () => {
    setSelectedConversation(null);
  }

  const handleConversationSelect = async (conversationId: number) => {
    if (authToken) { // Check if authToken is not empty
      const response = await fetch(`http://localhost:5119/api/v1/Interaction/GetConversationDetails?conversationId=${conversationId}`, {
        method: 'GET',
        headers: { 'Authorization': `Bearer ${authToken}` }
      });

      if (response.ok) {
        const conversationData = await response.json();
        setSelectedConversation(conversationData);
      }
    }
  };

  return (
    <div className="app-container">
      {authToken && userId && (
        <>
          <Sidebar authToken={authToken} userId={userId} onConversationSelect={handleConversationSelect} onStartNewConversation={handleStartNewConversation} />
          <ChatWindow selectedConversation={selectedConversation} />
        </>
      )}
    </div>
  );
};

export default HomePage;