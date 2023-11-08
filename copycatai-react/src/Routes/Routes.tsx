// Purpose: This file contains the routes for the application. It is used by the App.tsx file to render the correct page based on the URL.

//Import dependencies
import React from 'react';
import { BrowserRouter as Router, Route, Routes } from 'react-router-dom';

//Import page components
import HomePage from '../Pages/HomePage';
import AboutPage from '../Pages/AboutPage';
import InteractionPage from '../Pages/InteractionPage';
import ProfilePage from '../Pages/ProfilePage';
import LogInPage from '../Pages/LogInPage';
import RegisterPage from '../Pages/RegisterPage';
import SettingsPage from '../Pages/SettingsPage';
import ErrorPage from '../Pages/ErrorPage';


//Define Routes

const AppRoutes = () => {
    return (
        <Router>
            <Routes>
                <Route path="/" element={<HomePage />} />
                <Route path="/about" element={<AboutPage />} />
                <Route path="/interaction" element={<InteractionPage />} />
                <Route path="/profile" element={<ProfilePage />} />
                <Route path="/login" element={<LogInPage />} />
                <Route path="/register" element={<RegisterPage />} />
                <Route path="/settings" element={<SettingsPage />} />
                <Route path="*" element={<ErrorPage />} />
            </Routes>
        </Router>
    );
}

export default AppRoutes;