import { Outlet } from "react-router-dom";
import clsx from "clsx";
import { Provider as ReduxProvider, useDispatch, useSelector } from "react-redux";
import { useEffect } from "react";

import AdminSidebar from "./AdminSidebar";
import AdminTopbar from "./AdminTopbar";
import AdminFoother from "./AdminFoother.tsx";
import { AdminShellProvider, useAdminShell } from "./_ui/AdminShellContext";

import { adminStore, type AdminRootState, type AdminDispatch } from "./_redux/adminStore";
import { toggleTheme } from "@/layouts/admin/theme/themeConfigSlice";
import themeConfig from "@/theme.config";

function Frame() {
    const { collapsed } = useAdminShell();

    return (
        <div className="min-h-screen bg-gradient-to-br from-slate-50 via-white to-slate-100 text-foreground relative overflow-hidden">
            {/* Background Pattern */}
            <div className="absolute inset-0 bg-[radial-gradient(circle_at_50%_50%,rgba(99,102,241,0.03),transparent_50%)] pointer-events-none" />
            <div className="absolute inset-0 bg-[linear-gradient(45deg,transparent_25%,rgba(99,102,241,0.01)_50%,transparent_75%)] pointer-events-none" />

            <AdminSidebar />

            <div className={clsx(
                "transition-all duration-500 ease-out sm:pl-0 relative",
                collapsed ? "sm:ml-20" : "sm:ml-72"
            )}>
                <AdminTopbar />

                {/* Main Content Area with Glass Effect */}
                <main className="relative">
                    {/* Content Background with Glass Morphism */}
                    <div className="absolute inset-0 bg-white/40 backdrop-blur-sm" />

                    <div className="relative z-10 p-6 pt-8 min-h-[calc(100vh-5rem)]">
                        {/* Content Container with Subtle Animation */}
                        <div className="animate-in fade-in slide-in-from-bottom-4 duration-700 ease-out">
                            <Outlet />
                        </div>
                    </div>
                </main>

                <AdminFoother />
            </div>

            {/* Floating Elements for Visual Interest */}
            <div className="fixed top-20 right-10 w-32 h-32 bg-gradient-to-br from-indigo-400/10 to-purple-400/10 rounded-full blur-3xl animate-pulse pointer-events-none" />
            <div className="fixed bottom-20 left-1/4 w-24 h-24 bg-gradient-to-br from-pink-400/10 to-orange-400/10 rounded-full blur-2xl animate-pulse delay-1000 pointer-events-none" />
        </div>
    );
}

function AppWrapper({ children }: { children: React.ReactNode }) {
    const theme = useSelector((state: AdminRootState) => state.themeConfig.theme);
    const dispatch: AdminDispatch = useDispatch();

    useEffect(() => {
        dispatch(toggleTheme(localStorage.getItem('theme') || themeConfig.theme));
    }, [dispatch]);

    return (
        <div className={clsx(theme, "transition-colors duration-300 ease-out")}>
            {children}
        </div>
    );
}

// Ana Layout bileşeni, Provider'ları ve yapıyı kurar.
export default function AdminLayout() {
    return (
        <ReduxProvider store={adminStore}>
            <AdminShellProvider>
                <AppWrapper>
                    <Frame />
                </AppWrapper>
            </AdminShellProvider>
        </ReduxProvider>
    );
}