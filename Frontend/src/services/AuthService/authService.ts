// src/services/authService.ts
import * as authApi from '../../api/auth/authApi.ts'; // api/auth/index.ts'i authApi olarak import et
import type { LoginCommand, RegisterCommand, AccessTokenDto } from '../../types/authTypes';

export const loginUser = async (credentials: LoginCommand): Promise<AccessTokenDto> => {
    try {
        const response = await authApi.loginRequest(credentials);
        const accessTokenData = response.data; // Axios response.data otomatik olarak çözümlenmiş veriyi verir

        if (accessTokenData.token) {
            localStorage.setItem('token', accessTokenData.token);
        }
        return accessTokenData;
    } catch (error: any) {
        console.error('Login service error:', error);
        throw error.response?.data || error.message || error;
    }
};

export const registerUser = async (userData: RegisterCommand): Promise<void> => {
    try {
        await authApi.registerRequest(userData);
    } catch (error: any) {
        console.error('Register service error:', error);
        throw error.response?.data || error.message || error;
    }
};

export const fetchCurrentUser = async () => {
    try {
        const response = await authApi.getCurrentUserRequest();
        return response.data;
    } catch (error: any) {
        console.error('Fetch current user service error:', error);
        throw error.response?.data || error.message || error;
    }
};

export const logoutUserService = async () => { // Asenkron olabilir (eğer backend çağrısı varsa)
    try {
        // await authApi.logoutRequest(); // Eğer backend'de logout endpoint'i varsa
        localStorage.removeItem('token');
        // Diğer temizlik işlemleri...
    } catch (error) {
        console.error('Logout service error:', error);
        // Hata yönetimi
    }
};