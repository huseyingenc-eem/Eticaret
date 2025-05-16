import { BrowserRouter, Routes, Route } from 'react-router-dom';
import HomePage from './pages/HomePage';
import AuthForm from './components/auth/AuthForm';
import MainLayout from './components/layout/MainLayout';

function App() {
    return (
        <BrowserRouter>
            <Routes>
                <Route element={<MainLayout />}>
                    <Route path="/" element={<HomePage />} />
                    <Route path="/login" element={<AuthForm />} />
                    <Route path="/register" element={<AuthForm />} />
                </Route>
            </Routes>
        </BrowserRouter>
    );
}

export default App;
