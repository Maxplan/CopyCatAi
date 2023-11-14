import React from 'react';
import AuthForm from '../Components/Shared/AuthForm';
import { useNavigate } from 'react-router-dom';
import axios from 'axios'; // To Call API

const LoginPage = () => {
  const navigate = useNavigate();
  
  const loginUser = async (credentials: { email: string; password: string }) => {
    try {

      // Call API to log in
      const response = await axios.post('http://localhost:5119/api/v1/User/login', credentials, { withCredentials: true });
      
      // Store token in local storage
      localStorage.setItem("token", response.data.token);
      // Redirect the user to the landing page
      navigate('/');
    } catch (error) {
      // Error handling
      console.error(error);
      throw error;
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
