// src/App.tsx
import { Suspense } from "react";
import { useAuth } from "@/hooks";
import AutoRoutes from "@/routes/auto";
import NavigatorBinder from "@/services/NavigatorBinder";
import Notifier from "@/components/ui/Notifier/Notifier";
import ErrorBoundary from "@/components/common/ErrorBoundary/ErrorBoundary.tsx";
import Loading from "@/components/ui/Loading/Loading";
import './styles/App.css';
export default function App() {
    const { authReady } = useAuth();
    if (!authReady) return <Loading />;

    return (
        <ErrorBoundary>
            <Notifier />
            <NavigatorBinder />
            <Suspense fallback={<Loading />}>
                <AutoRoutes />
            </Suspense>
        </ErrorBoundary>
    );
}
