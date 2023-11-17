import React, { useState } from "react";
import { useNavigate } from "react-router-dom";
import "../../Styles/authForm.css";

type AuthFormProps = {
    action: "login" | "register";
    onFormSubmit: (credentials: { email: string; password: string; username?: string }) => Promise<void>;
}


const AuthForm = ({ action, onFormSubmit }: AuthFormProps) => {
  const isLogin = action === "login";
  const [username, setUsername] = useState("");
  const [password, setPassword] = useState("");
  const [email, setEmail] = useState("");
  const [firstName, setFirstName] = useState("");
  const [lastName, setLastName] = useState("");
  const [error, setError] = useState("");

  const navigate = useNavigate();

  const navigatetoRegister = () => {
    navigate('/register');
  }

  const handleSubmit = (e: React.FormEvent<HTMLFormElement>) => {
    e.preventDefault();
    setError("");

    // If logging in, use email and password; if registering, include username
    const credentials = isLogin
      ? { email, password }
      : { email, password, username, firstName, lastName };

    onFormSubmit(credentials)
      .then(() => {
      })
      .catch((error) => {
        // Set error message if the API call fails
        setError(error.response?.data?.message || 'An error occurred.');
      });
  };

  return (
    <form onSubmit={handleSubmit}>
      <label>
        Email:
        <input
          type="email"
          value={email}
          onChange={(e) => setEmail(e.target.value)}
          required
        />
      </label>

      {!isLogin && (
        <>
          <label>
            Username:
            <input
              type="text"
              value={username}
              onChange={(e) => setUsername(e.target.value)}
              required
            />
          </label>
          <label>
            First Name:
            <input
              type="text"
              value={firstName}
              onChange={(e) => setFirstName(e.target.value)}
              required
            />
          </label>
          <label>
            Last Name:
            <input
              type="text"
              value={lastName}
              onChange={(e) => setLastName(e.target.value)}
              required
            />
          </label>
        </>
      )}

      <label>
        Password:
        <input
          type="password"
          value={password}
          onChange={(e) => setPassword(e.target.value)}
          required
        />
      </label>

      <button type="submit">{isLogin ? 'Login' : 'Register'}</button>

      {isLogin && (
                <button className="reg-btn" onClick={navigatetoRegister}>Register</button>
            )}

      {error && <p className="error">{error}</p>}
    </form>
  );
};
export default AuthForm;