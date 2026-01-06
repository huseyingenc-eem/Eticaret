// src/hooks/useAuth.tsx
import { createContext, useContext, useEffect, useMemo, useState } from "react";
import { authApi, type LoginRequest, type RegisterRequest, type CurrentUserResponse } from "@/services/api/endpoints/auth.api";
import toast from "react-hot-toast";

type AuthContextType = {
    user: CurrentUserResponse | null;
    loading: boolean;
    isAuthenticated: boolean;
    authReady: boolean;
    login: (credentials: LoginRequest) => Promise<any>;
    register: (data: RegisterRequest) => Promise<any>;
    logout: () => void;
};

const AuthContext = createContext<AuthContextType | undefined>(undefined);

export function AuthProvider({ children }: { children: React.ReactNode }) {
    const [user, setUser] = useState<CurrentUserResponse | null>(null);
    const [loading, setLoading] = useState(false);
    const [isAuthenticated, setIsAuthenticated] = useState(false);
    const [authReady, setAuthReady] = useState(false); // ✅

    // Uygulama açılışında token varsa kullanıcıyı doğrula
    useEffect(() => {
        let mounted = true;

        const checkAuth = async () => {
            const token = localStorage.getItem("token");
            if (!token) {
                // Token yok → kontrol bitti
                if (mounted) setAuthReady(true);
                return;
            }

            try {
                const currentUser = await authApi.getCurrentUser();
                if (!mounted) return;
                setUser(currentUser);
                setIsAuthenticated(true);
            } catch {
                // Token geçersiz → temizle
                authApi.logout();
                if (!mounted) return;
                setIsAuthenticated(false);
                setUser(null);
            } finally {
                if (mounted) setAuthReady(true); // ✅ her durumda kontrol tamam
            }
        };

        void checkAuth();
        return () => { mounted = false; };
    }, []);

    const login = async (credentials: LoginRequest) => {
        setLoading(true);
        try {
            const response = await authApi.login(credentials);

            localStorage.setItem("token", response.token);
            if (response.refreshToken) {
                localStorage.setItem("refreshToken", response.refreshToken);
            }

            if (response.user) {
                setUser({ id: response.user.id, roles: response.user.roles });
            } else {
                const currentUser = await authApi.getCurrentUser();
                setUser(currentUser);
            }

            setIsAuthenticated(true);
            setAuthReady(true);
            toast.success("Giriş başarılı!");
            return response;
        } finally {
            setLoading(false);
        }
    };

    const register = async (userData: RegisterRequest) => {
        setLoading(true);
        try {
            const response = await authApi.register(userData);

            localStorage.setItem("token", response.token);

            if (response.user) {
                setUser({ id: response.user.id, roles: response.user.roles });
            } else {
                const currentUser = await authApi.getCurrentUser();
                setUser(currentUser);
            }

            setIsAuthenticated(true);
            setAuthReady(true); // ✅ register sonrası da hazır
            toast.success("Kayıt başarılı!");
            return response;
        } finally {
            setLoading(false);
        }
    };

    const logout = () => {
        authApi.logout();
        setUser(null);
        setIsAuthenticated(false);
        // authReady'yi true bırakıyoruz; çünkü kontrol yapılmış durumda
        toast.success("Çıkış yapıldı");
    };

    const value = useMemo<AuthContextType>(
        () => ({ user, loading, isAuthenticated, authReady, login, register, logout }),
        [user, loading, isAuthenticated, authReady]
    );

    return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
}

export function useAuth() {
    const ctx = useContext(AuthContext);
    if (!ctx) throw new Error("useAuth must be used within <AuthProvider>");
    return ctx;
}
