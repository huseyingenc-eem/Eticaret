import React from 'react';
import { Link } from 'react-router-dom';
import 'bootstrap/dist/css/bootstrap.min.css';

const Header: React.FC = () => {
    return (
        <header>
            <nav className="navbar navbar-expand-lg navbar-dark bg-primary px-4">
                <Link className="navbar-brand" to="/">ETicaret</Link>
                <button className="navbar-toggler" type="button" data-bs-toggle="collapse" data-bs-target="#navbarNav">
                    <span className="navbar-toggler-icon" />
                </button>
                <div className="collapse navbar-collapse justify-content-end" id="navbarNav">
                    <ul className="navbar-nav">
                        <li className="nav-item">
                            <Link className="nav-link" to="/login">Giriş Yap</Link>
                        </li>
                        <li className="nav-item">
                            <Link className="nav-link" to="/register">Kayıt Ol</Link>
                        </li>
                    </ul>
                </div>
            </nav>
        </header>
    );
};

export default Header;
