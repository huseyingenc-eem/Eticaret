import { Navigate, useLocation } from "react-router-dom";
import { useAuth } from "@/hooks";

export function RequireRole({ roles, children }: { roles: string[]; children: React.ReactElement }) {
    const { user, isAuthenticated, authReady } = useAuth();
    const location = useLocation();
    if (!authReady) return null; // ✅
    if (!isAuthenticated) return <Navigate to="/auth/login" replace state={{ from: location }} />;
    const ok = roles.length === 0 || roles.some(r => user?.roles?.includes(r));
    if (!ok) return <Navigate to="/403" replace state={{ from: location }} />;
    return children;
}