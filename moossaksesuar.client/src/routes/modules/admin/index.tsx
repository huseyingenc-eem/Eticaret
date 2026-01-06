import type { AppRouteObject } from "../../types";
import AdminLayout from "@/layouts/admin/AdminLayout";
import { Dashboard, Categories, CategoryDetailPage, ButtonPage , TimelinePage, Suppliers} from "./lazy";

const adminRoutes: AppRouteObject[] = [
    {
        path: "/admin",
        element: <AdminLayout />,
        meta: { requiresAuth: true, roles: ["Admin"] },
        children: [
            { index: true, element: <Dashboard />, meta: { title: "Dashboard" } },
            { path: "categories", element: <Categories />, meta: { title: "Categories" } },
            { path: "categories/:idSlug", element: <CategoryDetailPage />, meta: { title: "Category Detail :idSlug" } },
            { path: "suppliers", element: <Suppliers />, meta: { title: "Ui Components -> Timeline" } },

            // ui
            { path: "ui/buttons", element: <ButtonPage />, meta: { title: "Ui Components -> Button" } },
            { path: "ui/timeline", element: <TimelinePage />, meta: { title: "Ui Components -> Timeline" } },



        ],
    },
];

export default adminRoutes;
