import type { AppRouteObject } from "../types";
import { ADMIN } from "../paths";
import AdminLayout from "@/layouts/admin/AdminLayout";
import { Dashboard, Categories } from "./lazy";

const adminRoutes: AppRouteObject[] = [
    {
        path: ADMIN.ROOT,
        element: <AdminLayout />,
        meta: { requiresAuth: true, roles: ["Admin"] },
        children: [
            { index: true, element: <Dashboard />, meta: { title: "Dashboard" } },
            { path: "categories", element: <Categories />, meta: { title: "Categories", roles: ["Admin"] } },
            // { path: "users", element: <Users />, meta: { title: "Users", roles: ["Admin"] } },
        ],
    },
];

export default adminRoutes;
