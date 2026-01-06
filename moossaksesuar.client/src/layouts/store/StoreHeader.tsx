import { useState } from "react";
import { Link, NavLink } from "react-router-dom";
import { useNavigation } from "@/hooks/useNavigation";
import Container from "@/layouts/store/_components/Container";
import Logo from "@/layouts/store/_components/Logo";
import { IconMenu, IconClose } from "@/layouts/store/_components/icons";
import {
    Home,
    ShoppingBag,
    ShoppingCart,
    User,
    LogIn,
    type LucideIcon,
} from "lucide-react";

// İkonları string isimleriyle eşleştiren bir map
const iconMap: { [key: string]: LucideIcon } = {
    Home,
    ShoppingBag,
    ShoppingCart,
    User,
    LogIn,
};

export default function StoreHeader() {
    const [open, setOpen] = useState(false);
    const { header } = useNavigation();

    // Navigasyon linklerini ana menü ve kullanıcı aksiyonları olarak ayıralım
    // Bu ayırımı config dosyasına yeni bir property ekleyerek de yapabilirdik.
    const mainNav = header.filter(item => ["home", "shop"].includes(item.id));
    const userNav = header.filter(item => ["cart", "account", "login"].includes(item.id));

    return (
        <header className="sticky top-0 z-40 border-b bg-white/80 backdrop-blur">
            <Container className="h-16 flex items-center justify-between">
                {/* Sol Taraf: Mobil Menü Butonu ve Logo */}
                <div className="flex items-center gap-3">
                    <button className="md:hidden p-2" aria-label="Open menu" onClick={() => setOpen(true)}>
                        <IconMenu />
                    </button>
                    <Logo />
                </div>

                {/* Orta: Masaüstü Ana Navigasyon */}
                <nav className="hidden md:flex items-center gap-6 text-sm">
                    {mainNav.map(item => (
                        <NavLink
                            key={item.id}
                            to={item.to!}
                            className={({ isActive }) =>
                                `hover:text-black/80 ${isActive ? "text-black font-medium" : "text-black/60"}`
                            }
                        >
                            {item.label}
                        </NavLink>
                    ))}
                </nav>

                {/* Sağ Taraf: Kullanıcı Aksiyonları */}
                <div className="flex items-center gap-4">
                    {userNav.map(item => {
                        const Icon = item.icon ? iconMap[item.icon] : null;
                        return (
                            <Link
                                key={item.id}
                                to={item.to!}
                                className="inline-flex items-center gap-2 text-sm text-black/70 hover:text-black"
                                aria-label={item.label}
                            >
                                {Icon && <Icon className="h-5 w-5" />}
                                <span className="hidden sm:inline">{item.label}</span>
                            </Link>
                        );
                    })}
                </div>
            </Container>

            {/* Mobil Menü (Drawer) */}
            {open && (
                <div className="md:hidden fixed inset-0 z-50">
                    <div className="absolute inset-0 bg-black/30" onClick={() => setOpen(false)} />
                    <div className="ml-0 h-full w-72 bg-white shadow-xl p-4 animate-in slide-in-from-left">
                        <div className="flex items-center justify-between h-12">
                            <Logo />
                            <button className="p-2" aria-label="Close menu" onClick={() => setOpen(false)}>
                                <IconClose />
                            </button>
                        </div>
                        <nav className="mt-4 space-y-1">
                            {header.map(item => (
                                <NavLink
                                    key={item.id}
                                    to={item.to!}
                                    onClick={() => setOpen(false)}
                                    className={({ isActive }) =>
                                        `flex items-center gap-3 rounded px-3 py-2 text-base hover:bg-gray-50 ${isActive ? "font-semibold bg-gray-50" : ""}`
                                    }
                                >
                                    {item.icon && iconMap[item.icon] && (
                                        <span>{iconMap[item.icon]({ className: "w-5 h-5 text-gray-600" })}</span>
                                    )}
                                    <span>{item.label}</span>
                                </NavLink>
                            ))}
                        </nav>
                    </div>
                </div>
            )}
        </header>
    );
}
