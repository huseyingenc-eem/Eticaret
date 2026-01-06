import {
    Menu, ChevronsLeft, ChevronsRight, Search, Bell, User,
    Settings, LogOut, Sparkles, Zap, Command
} from "lucide-react";
import clsx from "clsx";
import { useAdminShell } from "./_ui/AdminShellContext";
import { Dropdown } from "@/components/ui/Dropdown/Dropdown.tsx";
import { ThemeToggle } from "./_ui/ThemeToggle";

function SearchBar() {
    return (
        <div className="relative group">
            {/* Container (sade) */}
            <div className="relative flex items-center">
                <div className="absolute inset-0 bg-white rounded-2xl border border-slate-200 transition-colors duration-400" />

                <div className="relative flex items-center w-full">
                    <Search className="absolute left-4 w-4 h-4 text-slate-400" />
                    <Command className="absolute right-4 w-3 h-3 text-slate-300" />

                    <input
                        type="text"
                        placeholder="Search anything..."
                        autoComplete="off"
                        className={clsx(
                            "w-96 h-11 pl-11 pr-10 bg-transparent text-slate-700 placeholder:text-slate-400 text-sm font-medium",
                            "border-0 outline-none focus:outline-none relative z-10"
                        )}
                    />
                </div>
            </div>

            {/* Search Suggestions (yalnızca opacity geçişi) */}
            <div className="absolute top-full mt-2 left-0 right-0 bg-white rounded-xl border border-slate-200 shadow-sm opacity-0 invisible group-focus-within:opacity-100 group-focus-within:visible transition-opacity duration-100 z-50">
                <div className="p-3 text-xs text-slate-500 font-medium">
                    <div className="flex items-center gap-2 mb-2">
                        <Sparkles className="w-3 h-3" />
                        <span>Quick search suggestions</span>
                    </div>
                    <div className="space-y-1 text-slate-400">
                        <div>• Try "users", "settings", or "dashboard"</div>
                        <div>
                            • Use <kbd className="px-1 py-0.5 bg-slate-100 rounded text-xs">Cmd+K</kbd> for quick access
                        </div>
                    </div>
                </div>
            </div>
        </div>
    );
}

function NotificationButton() {
    return (
        <button
            className="relative p-3 rounded-2xl bg-white border border-slate-200 hover:bg-slate-50 transition-colors duration-100"
            aria-label="Notifications"
        >
            <Bell className="w-5 h-5 text-slate-600" />
            <div className="absolute -top-1 -right-1 w-5 h-5 bg-red-500 rounded-full flex items-center justify-center">
                <span className="text-xs font-bold text-white">3</span>
            </div>
        </button>
    );
}

function UserMenu() {
    return (
        <Dropdown placement="bottom-end" offsetPx={12} portalId="popper-portal">
            <Dropdown.Trigger className="relative p-3 rounded-2xl bg-white border border-slate-200 hover:bg-slate-50 transition-colors duration-100">
                <div className="flex items-center gap-3">
                    <div className="w-8 h-8 rounded-full bg-gradient-to-br from-indigo-600 to-purple-600 flex items-center justify-center">
                        <User className="w-4 h-4 text-white" />
                    </div>
                    <div className="hidden md:block text-left">
                        <div className="text-sm font-semibold text-slate-700">Admin User</div>
                        <div className="text-xs text-slate-500">admin@example.com</div>
                    </div>
                </div>
            </Dropdown.Trigger>

            <Dropdown.Content className="min-w-56 bg-white border border-slate-200 shadow-sm rounded-2xl p-2">
                <div className="px-4 py-3 border-b border-slate-200 mb-2">
                    <div className="flex items-center gap-3">
                        <div className="w-10 h-10 rounded-full bg-gradient-to-br from-indigo-600 to-purple-600 flex items-center justify-center">
                            <User className="w-5 h-5 text-white" />
                        </div>
                        <div>
                            <div className="text-sm font-semibold text-slate-800">Admin User</div>
                            <div className="text-xs text-slate-500">admin@example.com</div>
                        </div>
                    </div>
                </div>

                <div className="space-y-1">
                    <button className="group flex items-center gap-3 w-full px-4 py-3 text-left rounded-xl hover:bg-slate-50 transition-colors duration-100">
                        <div className="p-1.5 rounded-lg bg-indigo-100">
                            <User className="w-4 h-4 text-indigo-600" />
                        </div>
                        <span className="text-sm font-medium text-slate-700 group-hover:text-slate-900 transition-colors duration-100">
              Profile Settings
            </span>
                    </button>

                    <button className="group flex items-center gap-3 w-full px-4 py-3 text-left rounded-xl hover:bg-slate-50 transition-colors duration-100">
                        <div className="p-1.5 rounded-lg bg-purple-100">
                            <Settings className="w-4 h-4 text-purple-600" />
                        </div>
                        <span className="text-sm font-medium text-slate-700 group-hover:text-slate-900 transition-colors duration-100">
              Account Settings
            </span>
                    </button>

                    <button className="group flex items-center gap-3 w-full px-4 py-3 text-left rounded-xl hover:bg-slate-50 transition-colors duration-100">
                        <div className="p-1.5 rounded-lg bg-amber-100">
                            <Zap className="w-4 h-4 text-amber-600" />
                        </div>
                        <span className="text-sm font-medium text-slate-700 group-hover:text-slate-900 transition-colors duration-100">
              Upgrade to Pro
            </span>
                    </button>
                </div>

                <div className="border-t border-slate-200 mt-2 pt-2">
                    <button className="group flex items-center gap-3 w-full px-4 py-3 text-left rounded-xl hover:bg-red-50 transition-colors duration-100">
                        <div className="p-1.5 rounded-lg bg-red-100">
                            <LogOut className="w-4 h-4 text-red-600" />
                        </div>
                        <span className="text-sm font-medium text-red-700 group-hover:text-red-900 transition-colors duration-100">
              Sign Out
            </span>
                    </button>
                </div>
            </Dropdown.Content>
        </Dropdown>
    );
}

export default function AdminTopbar() {
    const { toggleSidebar, collapsed, toggleCollapsed } = useAdminShell();

    return (
        <header className="sticky top-0 z-50">
            {/* Arkaplan (sade) */}
            <div className="absolute inset-0 bg-white border-b border-slate-200" />

            <div className="relative z-10 flex items-center h-20 px-6 gap-4">
                {/* Mobile Menu Button */}
                <button
                    type="button"
                    onClick={toggleSidebar}
                    className="sm:hidden p-3 rounded-2xl bg-white border border-slate-200 hover:bg-slate-50 transition-colors duration-100"
                    aria-label="Open sidebar"
                >
                    <Menu className="w-5 h-5 text-slate-600" />
                </button>

                {/* Desktop Collapse Toggle */}
                <button
                    type="button"
                    onClick={toggleCollapsed}
                    className="hidden sm:flex items-center justify-center p-3 rounded-2xl bg-white border border-slate-200 hover:bg-slate-50 transition-colors duration-100"
                    title={collapsed ? "Expand sidebar" : "Collapse sidebar"}
                    aria-label="Toggle sidebar width"
                >
                    {collapsed ? (
                        <ChevronsRight className="w-5 h-5 text-slate-600" />
                    ) : (
                        <ChevronsLeft className="w-5 h-5 text-slate-600" />
                    )}
                </button>

                {/* Search */}
                <div className="hidden lg:block flex-1 max-w-2xl mx-6">
                    <SearchBar />
                </div>

                {/* Right Actions */}
                <div className="flex items-center gap-3 ml-auto">
                    <ThemeToggle />
                    <NotificationButton />
                    <UserMenu />
                </div>
            </div>

            {/* Alt çizgi efekti (statik) */}
            <div className="absolute bottom-0 left-0 right-0 h-px bg-gradient-to-r from-transparent via-slate-200 to-transparent" />
        </header>
    );
}
