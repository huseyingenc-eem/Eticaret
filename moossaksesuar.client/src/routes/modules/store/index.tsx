import type { AppRouteObject } from "../types";
import { STORE } from "../paths";
import StoreLayout from "@/layouts/store/StoreLayout";
// import { HomePage, ProductDetail, Cart, AccountLayout, Overview, Orders } from "./lazy";
import { HomePage } from "./lazy";

const storeRoutes: AppRouteObject[] = [
    {
        path: STORE.ROOT,
        element: <StoreLayout />,        // Header/Footer içerir
        children: [
            { index: true, element: <HomePage />, meta: { title: "Home" } },

            // Account: Store altında,  şartı
            // {
            //     path: STORE.ACCOUNT_ROOT.replace("/", ""),
            //     element: <AccountLayout />,
            //     meta: { requiresAuth: true, title: "My Account" },
            //     children: [
            //         { index: true, element: <Overview />, meta: { title: "Overview" } },
            //         { path: "orders", element: <Orders />, meta: { title: "Orders" } },
            //     ],
            // },
        ],
    },
];

export default storeRoutes;
