import { Navigate, useLocation } from "react-router-dom";
import { useAuth } from "@/hooks";

export function PublicOnly({ children }: { children: React.ReactElement }) {
    const { isAuthenticated, authReady } = useAuth();
    const location = useLocation();

    if (!authReady) return null;
    if (isAuthenticated) {
        const from = (location.state as any)?.from?.pathname || "/";
        return <Navigate to={from} replace />;
    }
    return children;
}
