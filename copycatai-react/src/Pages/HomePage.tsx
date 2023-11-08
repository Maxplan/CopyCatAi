import React from "react";
import WelcomeBanner from "../Components/HomePage/WelcomeBanner";
import HomePageActions from "../Components/HomePage/HomePageActions";

const HomePage = () => {
  return (
    <div>
      <h1>Home Page</h1>
      <p>This is the home page.</p>
      <WelcomeBanner />
      <HomePageActions />
    </div>
  );
};

export default HomePage;