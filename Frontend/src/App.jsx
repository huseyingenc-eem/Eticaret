import React, { lazy, Suspense } from 'react';
import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import { ToastContainer } from './utils/toast';
import AccessibleNavigationAnnouncer from './components/AccessibleNavigationAnnouncer.jsx';
import PrivateRoute from './components/login/PrivateRoute';
import { AuthProvider } from './contexts/AuthContext';

// Lazy-loaded pages
const Layout = lazy(() => import('./layout/Layout'));
const Login = lazy(() => import('./pages/Login'));
const SignUp = lazy(() => import('./pages/SignUp'));
const ForgetPassword = lazy(() => import('./pages/ForgotPassword'));
const ResetPassword = lazy(() => import('./pages/ResetPassword'));

function App() {
    return (
        <AuthProvider>
            <BrowserRouter>
                <ToastContainer />
                <AccessibleNavigationAnnouncer />
                <Suspense fallback={<div>Loading...</div>}>
                    <Routes>
                        {/* Public Routes */}
                        <Route path="/login" element={<Login />} />
                        <Route path="/signup" element={<SignUp />} />
                        <Route path="/forgot-password" element={<ForgetPassword />} />
                        <Route path="/reset-password/:token" element={<ResetPassword />} />

                        {/* Private Routes */}
                        <Route
                            path="/*"
                            element={
                                <PrivateRoute>
                                    <Layout />
                                </PrivateRoute>
                            }
                        />

                        {/* Redirect root to login */}
                        <Route path="/" element={<Navigate to="/login" replace />} />
                    </Routes>
                </Suspense>
            </BrowserRouter>
        </AuthProvider>
    );
}

export default App;
