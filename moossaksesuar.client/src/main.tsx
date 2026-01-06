import ReactDOM from "react-dom/client";
import { BrowserRouter } from "react-router-dom";
import App from "./App";
import { AuthProvider } from "@/hooks";
import './styles/App.css';
import 'simplebar-react/dist/simplebar.min.css';
if (!import.meta.env.VITE_BACKEND_URL || !import.meta.env.VITE_BACKEND_PORT) {
    console.error('Backend URL or PORT is not configured');
    console.error('Required: VITE_BACKEND_URL, VITE_BACKEND_PORT');
}

const rootElement = document.getElementById('root');

if (!rootElement) {
    throw new Error('Root element not found');
}

ReactDOM.createRoot(rootElement).render(
    <BrowserRouter>
        <AuthProvider>
            <App />
        </AuthProvider>
    </BrowserRouter>
);