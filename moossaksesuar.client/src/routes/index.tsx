// src/routes/index.tsx
import { Fragment } from "react";
import storeRoutes from "./modules/store";
import authRoutes from "./AuthRoute";
import adminRoutes from "./modules/admin";
import { RenderRoutes } from "./renderRoutes";
import type { AppRouteObject } from "./types";
import Forbidden from "@/pages/system/Forbidden";
import NotFound from "@/pages/system/NotFound";

const systemRoutes: AppRouteObject[] = [
    { path: "/403", element: <Forbidden /> },
    { path: "*", element: <NotFound /> },
];

const allRoutes: AppRouteObject[] = [
    ...storeRoutes,
    ...adminRoutes,
    ...(authRoutes ?? []),
    ...systemRoutes,
];

export default function AppRoutes() {
    return (
        <Fragment>
            <RenderRoutes routes={allRoutes} />
        </Fragment>
    );
}
