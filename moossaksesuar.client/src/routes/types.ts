import type { RouteObject } from "react-router-dom";

export type RouteMeta = {
    title?: string;
    requiresAuth?: boolean;
    roles?: string[];         // ör: ["Admin","Editor"]
    breadcrumb?: string[];
    hidden?: boolean;         // menüde gösterme
};

export type AppRouteObject = RouteObject & {
    meta?: RouteMeta;
    children?: AppRouteObject[];
};
