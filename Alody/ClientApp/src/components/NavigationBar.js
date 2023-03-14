import React from 'react';
import { Link } from 'react-router-dom';
import '../css/App.css';

const NavigationBar = ({ menuItems }) => {
    return (
        <nav className="navbarMainPage">
            {menuItems.map((item, index) => (
                <Link key={index} to={item.link} className="navbarMainPageLink">{item.label}</Link>
            ))}
        </nav>
    );
};

export default NavigationBar;