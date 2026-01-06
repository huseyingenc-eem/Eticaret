// src/layouts/admin/_ui/AdminShellContext.tsx
import { createContext, useContext, useMemo, useState, useEffect, type ReactNode } from "react";

type Ctx = {
    sidebarOpen: boolean;
    openSidebar: () => void;
    closeSidebar: () => void;
    toggleSidebar: () => void;

    // desktop mini mode
    collapsed: boolean;
    toggleCollapsed: () => void;
};

const AdminShellContext = createContext<Ctx | null>(null);

export function AdminShellProvider({ children }: { children: ReactNode }) {
    const [sidebarOpen, setSidebarOpen] = useState(false);
    const [collapsed, setCollapsed] = useState(false);

    useEffect(() => {
        if (sidebarOpen) document.body.classList.add("overflow-hidden");
        else document.body.classList.remove("overflow-hidden");
    }, [sidebarOpen]);

    const value = useMemo<Ctx>(() => ({
        sidebarOpen,
        openSidebar: () => setSidebarOpen(true),
        closeSidebar: () => setSidebarOpen(false),
        toggleSidebar: () => setSidebarOpen(s => !s),
        collapsed,
        toggleCollapsed: () => setCollapsed(s => !s)
    }), [sidebarOpen, collapsed]);

    return <AdminShellContext.Provider value={value}>{children}</AdminShellContext.Provider>;
}

export const useAdminShell = () => {
    const ctx = useContext(AdminShellContext);
    if (!ctx) throw new Error("useAdminShell must be used inside AdminShellProvider");
    return ctx;
};
