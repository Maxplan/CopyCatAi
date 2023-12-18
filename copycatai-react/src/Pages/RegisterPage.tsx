import React from "react";
import AuthForm from "../Components/Shared/AuthForm";
import { useNavigate } from "react-router-dom";
import axios from "axios";

const RegisterPage = () => { 

  const navigate = useNavigate();
  const registerUser = async (credentials: { email: string; password: string; username?: string }) => {
    try {
      await axios.post('http://localhost:5119/api/v1/User/register', credentials, { withCredentials: true });
      navigate('/login');
    } catch (error) {
      console.error(error);
    }
  }
  return (
    <div className="authform-container">
      <h1 className="sign-in-title">Register</h1>
      <AuthForm action="register" onFormSubmit={registerUser} />
    </div>
  );
};

export default RegisterPage;