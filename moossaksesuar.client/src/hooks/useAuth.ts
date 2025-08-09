// src/hooks/useAuth.ts
import { useState, useEffect } from 'react';
import { authApi, type LoginRequest, type RegisterRequest, type CurrentUserResponse } from '@/services/api/endpoints/auth.api';
import toast from 'react-hot-toast';

export const useAuth = () => {
    const [user, setUser] = useState<CurrentUserResponse | null>(null);
    const [loading, setLoading] = useState(false);
    const [isAuthenticated, setIsAuthenticated] = useState(false);

    // Sayfa yüklendiğinde token kontrolü
    useEffect(() => {
        const checkAuth = async () => {
            const token = localStorage.getItem('token');
            if (token) {
                try {
                    const currentUser = await authApi.getCurrentUser();
                    setUser(currentUser);
                    setIsAuthenticated(true);
                } catch {
                    // Error zaten baseApi'de handle edildi
                    authApi.logout();
                    setIsAuthenticated(false);
                }
            }
        };

        checkAuth();
    }, []);

    const login = async (credentials: LoginRequest) => {
        setLoading(true);

        try {
            const response = await authApi.login(credentials);

            // Token'ı kaydet
            localStorage.setItem('token', response.token);
            if (response.refreshToken) {
                localStorage.setItem('refreshToken', response.refreshToken);
            }

            // Backend'den user bilgisi gelirse kullan, yoksa getCurrentUser çağır
            if (response.user) {
                setUser({
                    id: response.user.id,
                    roles: response.user.roles
                });
            } else {
                // Eğer login response'unda user bilgisi yoksa, ayrı bir çağrı yap
                const currentUser = await authApi.getCurrentUser();
                setUser(currentUser);
            }

            setIsAuthenticated(true);
            toast.success('Giriş başarılı!');
            return response;
        } finally {
            setLoading(false);
        }
    };

    const register = async (userData: RegisterRequest) => {
        setLoading(true);

        try {
            const response = await authApi.register(userData);

            localStorage.setItem('token', response.token);

            // Backend'den user bilgisi gelirse kullan, yoksa getCurrentUser çağır
            if (response.user) {
                setUser({
                    id: response.user.id,
                    roles: response.user.roles
                });
            } else {
                const currentUser = await authApi.getCurrentUser();
                setUser(currentUser);
            }

            setIsAuthenticated(true);
            toast.success('Kayıt başarılı!');
            return response;
        } finally {
            setLoading(false);
        }
    };

    const logout = () => {
        authApi.logout();
        setUser(null);
        setIsAuthenticated(false);
        toast.success('Çıkış yapıldı');
    };

    return {
        user,
        loading,
        isAuthenticated,
        login,
        register,
        logout
    };
};