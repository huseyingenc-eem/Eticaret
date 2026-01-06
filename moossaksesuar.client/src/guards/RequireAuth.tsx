import { Navigate, useLocation } from "react-router-dom";
import { useAuth } from "@/hooks";
export function RequireAuth({ children }: { children: React.ReactElement }) {
    const { isAuthenticated, authReady } = useAuth();
    const location = useLocation();
    if (!authReady) return null;
    if (!isAuthenticated) return <Navigate to="/auth/login" replace state={{ from: location }} />;
    return children;
}