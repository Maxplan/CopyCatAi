import React from "react";
import Routes from "./Routes/Routes";
import "./Styles/global.css";
import Header from "./Components/Shared/Header";

const app: React.FC = () => {
  return (
    <div>
      <Header />
      <Routes />
      
    </div>
  );
}

export default app;