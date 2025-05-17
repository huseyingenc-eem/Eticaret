import React, { lazy, Suspense } from 'react';
import { Routes, Route, Navigate } from 'react-router-dom';
import { ToastContainer } from './utils/toast.jsx';
import AccessibleNavigationAnnouncer from './components/AccessibleNavigationAnnouncer.jsx';
import PrivateRoute from './components/login/PrivateRoute.jsx';

const Layout = lazy(() => import('./layout/Layout.jsx'));
const Login = lazy(() => import('./pages/Login.jsx'));
const SignUp = lazy(() => import('./pages/SignUp'));
const ForgetPassword = lazy(() => import('./pages/ForgotPassword'));
const ResetPassword = lazy(() => import('./pages/ResetPassword'));

function App() {
    return (
        <>
            <ToastContainer />
            <AccessibleNavigationAnnouncer />
            <Suspense fallback={<div>Loading...</div>}>
                <Routes>
                    {/* Public Routes */}
                    <Route path="/login" element={<Login />} />
                    <Route path="/signup" element={<SignUp />} />
                    <Route path="/forgot-password" element={<ForgetPassword />} />
                    <Route path="/reset-password/:token" element={<ResetPassword />} />

                    {/* Root path direkt /dashboard’a yönlendirsin */}
                    <Route path="/" element={<Navigate to="/dashboard" replace />} />

                    {/* Private Layout Route */}
                    <Route
                        path="/*"
                        element={
                            <PrivateRoute>
                                <Layout />
                            </PrivateRoute>
                        }
                    />
                </Routes>
            </Suspense>
        </>
    );
}

export default App;
