// Header including app name, logo, logged in user name
import React from "react";
import "../../Styles/header.css";

const Header = () => {
  return (
    <div className="header">
      <h1 className="logo"><a href="/">CopyCat AI</a></h1>
      <h2 className="user"><span>Username</span></h2>
    </div>
  );
};

export default Header;