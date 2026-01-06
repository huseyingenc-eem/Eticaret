// src/routes/modules/auth.tsx
import type { AppRouteObject } from "../types";
import AuthMinimalLayout from "@/layouts/auth/AuthMinimalLayout";
import { PublicOnly } from "@/guards/PublicOnly";
import { lazy } from "react";
const Auth = lazy(() => import("@/pages/AuthPages"));

const authRoutes: AppRouteObject[] = [
    {
        path: "/auth",
        element: (
            <PublicOnly>
                <AuthMinimalLayout />
            </PublicOnly>
        ),
        children: [
            { index: true, element: <Auth /> },
            { path: "login", element: <Auth /> },
        ],
    },
];

export default authRoutes;
