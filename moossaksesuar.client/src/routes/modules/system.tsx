import type { AppRouteObject } from "../types";
import Forbidden from "@/pages/system/Forbidden";
import NotFound from "@/pages/system/NotFound";

const systemRoutes: AppRouteObject[] = [
    { path: "/403", element: <Forbidden /> },
    { path: "*",   element: <NotFound /> },
];

export default systemRoutes;
