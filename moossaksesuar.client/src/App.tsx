// src/App.tsx
import { BrowserRouter as Router, Routes, Route, Navigate } from 'react-router-dom';
import { Toaster } from 'react-hot-toast';
import { useAuth } from '@/hooks/useAuth';
import AuthPage from '@/pages/AuthPage';
import HomePage from '@/pages/HomePage';

import ErrorBoundary from '@/components/common/ErrorBoundary';
import Loading from '@/components/ui/Loading/Loading';

function App() {
    const { isAuthenticated, loading, user } = useAuth();

    if (loading) {
        return <Loading />;
    }

    const isAdmin = user?.roles?.includes('Admin');

    return (
        <ErrorBoundary>
            <Router>
                <Routes>
                    {/* Public Routes - Herkes Erişebilir */}
                    <Route path="/" element={<HomePage />} />

                    {/* Auth Routes */}
                    <Route
                        path="/auth/*"
                        element={!isAuthenticated ? <AuthPage /> : <Navigate to="/" />}
                    />

                    {/* Semi-Protected Routes - Sepet görüntüleme herkese açık, checkout için auth gerekli */}
                    {/* Protected Routes - Auth Gerekli */}




                    {/* Fallback */}
                    <Route path="*" element={<Navigate to="/" />} />
                </Routes>

                {/* Global Toast Container */}
                <Toaster
                    position="top-right"
                    toastOptions={{
                        duration: 4000,
                        style: {
                            background: '#fff',
                            color: '#333',
                            border: '1px solid #e2e8f0',
                            boxShadow: '0 4px 6px -1px rgba(0, 0, 0, 0.1)',
                        },
                        success: {
                            iconTheme: {
                                primary: '#10B981',
                                secondary: '#fff',
                            },
                        },
                        error: {
                            iconTheme: {
                                primary: '#EF4444',
                                secondary: '#fff',
                            },
                        },
                    }}
                />
            </Router>
        </ErrorBoundary>
    );
}

export default App;