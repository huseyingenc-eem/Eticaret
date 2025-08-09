import axios from 'axios';
import type {  AxiosResponse, AxiosError ,InternalAxiosRequestConfig} from 'axios';
import { handleApiError } from './errorHandler';

export interface ApiError {
    type?: string;
    title: string;
    status: number;
    detail: string;
    instance?: string;
    errors?: Record<string, string[]>;
    errorCode?: string;
    userFriendlyMessage?: string;
    developerDetail?: string;
    additionalData?: Record<string, any>;
}

const API_BASE_URL = `${import.meta.env.VITE_BACKEND_URL}:${import.meta.env.VITE_BACKEND_PORT}`;

export const apiClient = axios.create({
    baseURL: `${API_BASE_URL}/api`,
    headers: {
        'Content-Type': 'application/json',
    },
});

// Request interceptor
apiClient.interceptors.request.use(
    (config: InternalAxiosRequestConfig) => {
        const token = localStorage.getItem('token');
        if (token && config.headers) {
            config.headers.Authorization = `Bearer ${token}`;
        }
        return config;
    },
    (error) => Promise.reject(error)
);

// Response interceptor
apiClient.interceptors.response.use(
    (response: AxiosResponse) => response,
    (error: AxiosError<ApiError>) => {
        handleApiError(error);
        return Promise.reject(error);
    }
);