import type { AppRouteObject } from "../../types";
import StoreLayout from "@/layouts/store/StoreLayout";
import {HomePage} from "./lazy"

const storeRoutes: AppRouteObject[] = [
    {
        element: <StoreLayout />,
        children: [
            { index: true, element: <HomePage />, meta: { title: "Home" } },
        ],
    },
];

export default storeRoutes;
