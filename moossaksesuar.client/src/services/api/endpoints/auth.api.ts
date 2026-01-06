import { baseApi } from '../baseApi';

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

export const authApi = {
    register: (data: RegisterRequest) =>
        baseApi.post<AuthResponse, RegisterRequest>('/Auth/register', data),

    login: (data: LoginRequest) =>
        baseApi.post<AuthResponse, LoginRequest>('/Auth/login', data),

    getCurrentUser: () =>
        baseApi.get<CurrentUserResponse>('/Auth/current'),

    logout: () => {
        localStorage.removeItem('token');
        localStorage.removeItem('refreshToken');
    }
};
