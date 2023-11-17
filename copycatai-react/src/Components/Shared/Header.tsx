import React, { useState, useRef, useEffect } from "react";
import "../../Styles/header.css";

const Header = () => {
    const [isMenuOpen, setIsMenuOpen] = useState(false);
    // Explicitly type the ref as HTMLDivElement
    const menuRef = useRef<HTMLDivElement>(null);

    const toggleMenu = () => {
        setIsMenuOpen(!isMenuOpen);
    };

    const logout = () => {
        localStorage.removeItem('token');
        window.location.reload();
    };

    const handleClickOutside = (event: MouseEvent) => {
        if (menuRef.current && !menuRef.current.contains(event.target as Node)) {
            setIsMenuOpen(false);
        }
    };

    useEffect(() => {
        if (isMenuOpen) {
            document.addEventListener("mousedown", handleClickOutside);
        }

        // Cleanup
        return () => {
            document.removeEventListener("mousedown", handleClickOutside);
        };
    }, [isMenuOpen]);

  return (
    <div className="header">
            <h1 className="logo"><a href="/">CopyCat AI</a></h1>
            <div className="hamburger-menu" onClick={toggleMenu}>
                <span></span>
                <span></span>
                <span></span>
            </div>
            {isMenuOpen && (
                <div className="dropdown-menu" ref={menuRef}>
                    <p>Profile</p>
                    <p>Settings</p>
                    <p onClick={logout}>Logout</p>
                </div>
            )}
        </div>
  );
};

export default Header;
