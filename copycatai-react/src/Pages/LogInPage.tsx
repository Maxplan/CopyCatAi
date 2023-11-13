import React from 'react';
import AuthForm from '../Components/Shared/AuthForm';
import { useNavigate } from 'react-router-dom';
import axios from 'axios'; // Ensure axios is installed and imported

const LoginPage = () => {
  const navigate = useNavigate();
  
  const loginUser = async (credentials: { email: string; password: string }) => {
    try {

      // Call API to log in
      await axios.post('http://localhost:5119/api/v1/User/login', credentials, { withCredentials: true });
    
      // Redirect the user to the landing page
      navigate('/');
    } catch (error) {
      // Handle Error Here
      console.error(error);
    }
  };
  
  return (
    <div>
      <h1>Login Page</h1>
      <AuthForm action="login" onFormSubmit={loginUser} />
    </div>
  );
};

export default LoginPage;
