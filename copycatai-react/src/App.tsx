import React from "react";
import Routes from "./Routes/Routes";
import "./Styles/global.css";
import Header from "./Components/Shared/Header";
import { ConversationProvider } from "./Components/Shared/ConversationContext";

const app: React.FC = () => {
  return (

    <ConversationProvider>
      <Header />
      <Routes /> 
    </ConversationProvider>
  );
}

export default app;