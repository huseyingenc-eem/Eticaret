import React from 'react';
import { Navigate } from 'react-router-dom';

interface Props {
    children: JSX.Element;
}

const PublicRoute = ({ children }: Props): JSX.Element => {
    const token = localStorage.getItem('token');
    return token ? <Navigate to="/" replace /> : children;
};

export default PublicRoute;
