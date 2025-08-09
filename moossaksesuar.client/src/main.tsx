// src/main.tsx
import { StrictMode } from 'react';
import { createRoot } from 'react-dom/client';
import App from './App';
import './styles/App.css';

// baseApi.ts'de kullanılan environment variable'ları kontrol et
if (!import.meta.env.VITE_BACKEND_URL || !import.meta.env.VITE_BACKEND_PORT) {
    console.error('Backend URL or PORT is not configured');
    console.error('Required: VITE_BACKEND_URL, VITE_BACKEND_PORT');
}

const rootElement = document.getElementById('root');

if (!rootElement) {
    throw new Error('Root element not found');
}

createRoot(rootElement).render(
    <StrictMode>
        <App />
    </StrictMode>
);