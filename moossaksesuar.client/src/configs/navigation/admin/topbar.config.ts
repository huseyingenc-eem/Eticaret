import type { NavLink } from '@/configs/types';

export const topbarNav: NavLink[] = [
    { id: "profile", label: "Profile", to: "/admin/profile", icon: "User" },
    { id: "signout", label: "Sign out", action: "logout", icon: "LogOut" },
];

export const userMenuItems: NavLink = [
    { id: "profile", label: 'Profile', to: '/admin/profile' },
    { id: "inbox", label: 'Inbox', to: '/admin/inbox', badge: '15' },
    { id: "upgrade",label: 'Upgrade', to: '/pricing', badge: 'Pro' },
    { id: "signout",label: 'Sign Out', action: "logout", icon: "LogOut" },
];