import React from 'react';
import 'bootstrap/dist/css/bootstrap.min.css';

const Footer: React.FC = () => {
    return (
        <footer className="bg-dark text-white text-center py-3 mt-auto">
            <div className="container">
                <p className="mb-0">&copy; {new Date().getFullYear()} E-Ticaret Projesi | Tüm Hakları Saklıdır.</p>
            </div>
        </footer>
    );
};

export default Footer;
