import type { FooterColumn } from '@/configs/types';

export const footerNav: FooterColumn[] = [
    {
        title: "Shop",
        links: [
            { id: "all", label: "All Products", to: "/store" },
            { id: "new", label: "New Arrivals", to: "/store/new" },
            { id: "sale", label: "Sale", to: "/store/sale", feature: "salePage" },
        ],
    },
    {
        title: "Support",
        links: [
            { id: "help", label: "Help Center", to: "/help" },
            { id: "shipping", label: "Shipping", to: "/shipping" },
            { id: "returns", label: "Returns", to: "/returns" },
        ],
    },
];