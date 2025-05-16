import React, { useState } from 'react';
import 'bootstrap/dist/css/bootstrap.min.css';
import './AuthForm.css';
import { loginUser, registerUser } from '../../services/AuthService/authService.ts'; // <- API bağlantısı
import { useNavigate } from 'react-router-dom';

const AuthForm: React.FC = () => {
    const [isRegistering, setIsRegistering] = useState(false);
    const [loginEmail, setLoginEmail] = useState('');
    const [loginPassword, setLoginPassword] = useState('');
    const [registerFirstName, setRegisterFirstName] = useState('');
    const [registerLastName, setRegisterLastName] = useState('');
    const [registerEmail, setRegisterEmail] = useState('');
    const [registerPassword, setRegisterPassword] = useState('');
    const [error, setError] = useState<string | null>(null);
    const navigate = useNavigate();

    const handleLoginSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        setError(null);
        try {
            const result = await loginUser({
                email: loginEmail,
                password: loginPassword,
            });
            console.log('Login successful:', result);
            localStorage.setItem('token', result.token);
            navigate('/');
        } catch (err: any) {
            console.error('Login error:', err);
            setError(err.userFriendlyMessage || err.message || 'Login failed');
        }
    };

    const handleRegisterSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        setError(null);
        try {
            const result = await registerUser({
                firstName: registerFirstName,
                lastName: registerLastName,
                email: registerEmail,
                password: registerPassword,
            });
            console.log('Register successful:', result);
            alert('Registration successful! You can now log in.');
            setIsRegistering(false);
        } catch (err: any) {
            console.error('Register error:', err);
            setError(err.userFriendlyMessage || err.message || 'Registration failed');
        }
    };

    return (
        <div className="container d-flex align-items-center justify-content-center min-vh-100">
            <div className="card p-4 shadow-lg" style={{ maxWidth: '900px', width: '100%', borderRadius: '20px' }}>
                <div className="row g-0">
                    {/* Toggle Panel */}
                    <div className="col-md-6 d-flex flex-column justify-content-center align-items-center text-white bg-primary p-4 rounded-start">
                        <h2 className="fw-bold mb-2">
                            {isRegistering ? 'Welcome Back!' : 'Hello, Welcome!'}
                        </h2>
                        <p>
                            {isRegistering ? 'Already have an account?' : `Don't have an account?`}
                        </p>
                        <button
                            className="btn btn-outline-light"
                            onClick={() => setIsRegistering(!isRegistering)}
                        >
                            {isRegistering ? 'Login' : 'Register'}
                        </button>
                    </div>

                    {/* Form Panel */}
                    <div className="col-md-6 p-4">
                        {error && <div className="alert alert-danger text-center">{error}</div>}

                        {isRegistering ? (
                            <form onSubmit={handleRegisterSubmit}>
                                <h3 className="mb-4 text-center">Register</h3>
                                <div className="mb-3">
                                    <input
                                        type="text"
                                        className="form-control"
                                        placeholder="First Name"
                                        value={registerFirstName}
                                        onChange={(e) => setRegisterFirstName(e.target.value)}
                                        required
                                    />
                                </div>
                                <div className="mb-3">
                                    <input
                                        type="text"
                                        className="form-control"
                                        placeholder="Last Name"
                                        value={registerLastName}
                                        onChange={(e) => setRegisterLastName(e.target.value)}
                                        required
                                    />
                                </div>
                                <div className="mb-3">
                                    <input
                                        type="email"
                                        className="form-control"
                                        placeholder="Email"
                                        value={registerEmail}
                                        onChange={(e) => setRegisterEmail(e.target.value)}
                                        required
                                    />
                                </div>
                                <div className="mb-3">
                                    <input
                                        type="password"
                                        className="form-control"
                                        placeholder="Password"
                                        value={registerPassword}
                                        onChange={(e) => setRegisterPassword(e.target.value)}
                                        required
                                    />
                                </div>
                                <button type="submit" className="btn btn-primary w-100">Register</button>
                            </form>
                        ) : (
                            <form onSubmit={handleLoginSubmit}>
                                <h3 className="mb-4 text-center">Login</h3>
                                <div className="mb-3">
                                    <input
                                        type="email"
                                        className="form-control"
                                        placeholder="Email"
                                        value={loginEmail}
                                        onChange={(e) => setLoginEmail(e.target.value)}
                                        required
                                    />
                                </div>
                                <div className="mb-3">
                                    <input
                                        type="password"
                                        className="form-control"
                                        placeholder="Password"
                                        value={loginPassword}
                                        onChange={(e) => setLoginPassword(e.target.value)}
                                        required
                                    />
                                </div>
                                <button type="submit" className="btn btn-primary w-100">Login</button>
                            </form>
                        )}
                    </div>
                </div>
            </div>
        </div>
    );
};

export default AuthForm;
