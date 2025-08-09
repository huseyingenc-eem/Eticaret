import axios from 'axios';

const API_BASE_URL = import.meta.env.VITE_BACKEND_URL + ':' + import.meta.env.VITE_BACKEND_PORT;

export const apiClient = axios.create({
    baseURL: `${API_BASE_URL}/api`,
    headers: {
        'Content-Type': 'application/json',
    },
});

// Token interceptor
apiClient.interceptors.request.use((config) => {
    const token = localStorage.getItem('token');
    if (token) {
        config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
});