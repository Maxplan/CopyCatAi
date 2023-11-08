import React from 'react';
import AuthForm from '../Components/Shared/AuthForm';
import { useNavigate } from 'react-router-dom';
import axios from 'axios'; // Ensure axios is installed and imported

const LoginPage = () => {
  const navigate = useNavigate();

  const loginUser = async (credentials: { email: string; password: string }) => {
    try {
      // Call API to log in
      const response = await axios.post('http://localhost:5119/api/v1/User/login', credentials);

      // If login is successful, store the received token (you might be doing this differently)
      localStorage.setItem('token', response.data.token);
      console.log(response.data.token)
      // Redirect the user to the interaction page
      navigate('/interaction');
    } catch (error) {
      // Handle errors, like showing a message to the user
      console.error('Login failed:', error);
      // You could set an error state here and display it if needed
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
