import { apiClient } from '../baseApi';

// Request/Response Types
export interface RegisterRequest {
    firstName: string;
    lastName: string;
    email: string;
    password: string;
    confirmPassword: string;
}

export interface LoginRequest {
    email: string;
    password: string;
}

export interface AuthResponse {
    token: string;
    refreshToken?: string;
    user: {
        id: string;
        firstName: string;
        lastName: string;
        email: string;
        roles: string[];
    };
}

export interface CurrentUserResponse {
    id: string;
    roles: string[];
}

// Auth API
export const authApi = {
    // Kayıt ol
    register: async (data: RegisterRequest): Promise<AuthResponse> => {
        const response = await apiClient.post<AuthResponse>('/Auth/register', data);
        return response.data;
    },

    // Giriş yap
    login: async (data: LoginRequest): Promise<AuthResponse> => {
        const response = await apiClient.post<AuthResponse>('/Auth/login', data);
        return response.data;
    },

    // Mevcut kullanıcı bilgisi
    getCurrentUser: async (): Promise<CurrentUserResponse> => {
        const response = await apiClient.get<CurrentUserResponse>('/Auth/current');
        return response.data;
    },

    // Çıkış yap (localStorage temizleme)
    logout: () => {
        localStorage.removeItem('token');
        localStorage.removeItem('refreshToken');
    }
};