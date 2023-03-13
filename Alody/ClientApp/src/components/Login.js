import React, { useState } from 'react';
import '../css/LoginForm.css';

function LoginForm() {
    const [isRegistering, setIsRegistering] = useState(false);

    const handleSubmit = (event) => {
        event.preventDefault();
        // handle login or registration
    }

    return (
        <div className="login-form">
            <h1>{isRegistering ? 'Register' : 'Login'}</h1>
            <form onSubmit={handleSubmit}>
                <label htmlFor="email">Email</label>
                <input type="email" id="email" required />

                <label htmlFor="password">Password</label>
                <input type="password" id="password" required />

                {isRegistering && (
                    <div>
                        <label htmlFor="confirmPassword">Confirm Password</label>
                        <input type="password" id="confirmPassword" required />
                    </div>
                )}

                <button type="submit" className="login-btn">
                    {isRegistering ? 'Register' : 'Login'}
                </button>
            </form>

            <button onClick={() => setIsRegistering(!isRegistering)} className="toggle-form-btn">
                {isRegistering ? 'Already have an account? Login' : "Don't have an account? Register"}
            </button>
        </div>
    );
}

export default LoginForm;