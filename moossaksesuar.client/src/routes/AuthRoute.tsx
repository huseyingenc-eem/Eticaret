import { lazy, Suspense } from "react";
import Loading from "@/components/ui/Loading/Loading";
import type { AppRouteObject } from "@/routes/types";

const AuthPages = lazy(() => import("@/pages/AuthPages"));

const authRoutes: AppRouteObject[] = [
    {
        path: "/auth/*",
        element: (
            <Suspense fallback={<Loading />}>
                <AuthPages />
            </Suspense>
        ),
    },
];

export default authRoutes;