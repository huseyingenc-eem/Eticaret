import { RenderRoutes } from "./renderRoutes";
import type { AppRouteObject } from "./types";

const modules = import.meta.glob("./modules/**/*.{ts,tsx}", { eager: true });

const allRoutes: AppRouteObject[] = [];
for (const path in modules) {
    const mod = modules[path] as { default?: AppRouteObject[] };
    if (mod?.default) allRoutes.push(...mod.default);
}

export default function AutoRoutes() {
    return <RenderRoutes routes={allRoutes} />;
}
