import type { NavLink } from '@/configs/types';

export const headerNav: NavLink[] = [
    { id: "home", label: "Home", to: "/", icon: "Home" },
    { id: "shop", label: "Shop", to: "/store", icon: "ShoppingBag" },
    { id: "cart", label: "Cart", to: "/cart", icon: "ShoppingCart", visibility: "auth" },
    { id: "account", label: "Account", to: "/account", icon: "User", visibility: "auth" },
    { id: "login", label: "Login", to: "/auth", icon: "LogIn", visibility: "guest" },
];