import React from "react";
import { Navigate } from "react-router-dom";

interface ProtectedRouteProps {
    component: React.ComponentType;
}

const isAuthenticated = () => {
    return !!localStorage.getItem('token');
};

const ProtectedRoute: React.FC<ProtectedRouteProps> = ({ component: Component }) => {
    const [isAuthed, setIsAuthed] = React.useState<boolean | null>(null);

    React.useEffect(() => {
        const authed = isAuthenticated();
        setIsAuthed(authed);
    }, []);

    if (isAuthed === null) {
        return <div>Loading...</div>;
    }

    return isAuthed ? <Component /> : <Navigate to="/login" />;
};

export default ProtectedRoute;
