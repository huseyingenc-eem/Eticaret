export type NavVisibility = "always" | "auth" | "guest";

export type NavLink = {
    id: string;
    label: string;
    to?: string;
    external?: boolean;
    icon?: string;
    roles?: string[];
    feature?: string;
    visibility?: NavVisibility;
    children?: NavLink[];
    order?: number;
    hidden?: boolean;
    action?: "logout" | string;
};

export type FooterLink = NavLink;

export type FooterColumn = {
    title: string;
    links: FooterLink[];
};
