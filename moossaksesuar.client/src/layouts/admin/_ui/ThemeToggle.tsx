import { useDispatch, useSelector } from "react-redux";
import type { AdminRootState, AdminDispatch } from "../_redux/adminStore";
import { toggleTheme } from "@/layouts/admin/theme/themeConfigSlice";
import { Sun, Moon } from "lucide-react";

export function ThemeToggle() {
    const dispatch: AdminDispatch = useDispatch();
    const currentTheme = useSelector((state: AdminRootState) => state.themeConfig.theme);

    const handleThemeToggle = () => {
        const nextTheme = currentTheme === 'dark' ? 'light' : 'dark';
        dispatch(toggleTheme(nextTheme));
    };

    return (
        <button
            type="button"
            onClick={handleThemeToggle}
            className="inline-flex justify-center items-center rounded-md p-2 hover:bg-muted"
            aria-label="Toggle theme"
        >
            {currentTheme === 'dark' ? (
                <Sun size={20} className="text-yellow-500" />
            ) : (
                <Moon size={20} className="text-slate-700" />
            )}
        </button>
    );
}