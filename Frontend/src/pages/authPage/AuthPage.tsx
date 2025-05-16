import React from 'react';
import AuthForm from '../../components/auth/AuthForm'; // AuthForm component'inin yolunu güncelleyin

const AuthPage: React.FC = () => {
    return (
        <div className="auth-page-container">
            <AuthForm />
        </div>
    );
};

export default AuthPage;