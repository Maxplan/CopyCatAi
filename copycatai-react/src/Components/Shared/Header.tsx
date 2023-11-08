//header including a logout button only

import React from "react";
import axios from "axios";

const Header = () => {
  return (
    <div>
      <button type="submit" onClick={
        async () => {
          try {
            await axios.post('http://localhost:5119/api/v1/User/logout', {}, { withCredentials: true });
          } catch (error) {
            console.error(error);
          }
        }
      }>Logout</button>
    </div>
  );
};

export default Header;