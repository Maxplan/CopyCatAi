import React from "react";
import AuthForm from "../Components/Shared/AuthForm";
import { useNavigate } from "react-router-dom";
import axios from "axios";

const RegisterPage = () => { 

  const navigate = useNavigate();
  const registerUser = async (credentials: { email: string; password: string; username?: string }) => {
    try {
      await axios.post('http://localhost:5119/api/v1/User/register', credentials, { withCredentials: true });
      navigate('/interaction');
    } catch (error) {
      console.error(error);
    }
  }
  return (
    <div>
      <h1>Register Page</h1>
      <AuthForm action="register" onFormSubmit={registerUser} />
    </div>
  );
};

export default RegisterPage;