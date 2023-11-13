// Purpose: This file contains the routes for the application. It is used by the App.tsx file to render the correct page based on the URL.

//Import dependencies
import React from 'react';
import { BrowserRouter as Router, Route, Routes } from 'react-router-dom';

//Import page components
import HomePage from '../Pages/HomePage';
import LogInPage from '../Pages/LogInPage';
import RegisterPage from '../Pages/RegisterPage';


//Define Routes

const AppRoutes = () => {
    return (
        <Router>
            <Routes>
                <Route path="/" element={<HomePage />} />
                <Route path="/login" element={<LogInPage />} />
                <Route path="/register" element={<RegisterPage />} />
            </Routes>
        </Router>
    );
}

export default AppRoutes;