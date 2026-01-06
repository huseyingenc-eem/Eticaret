// src/routes/renderRoutes.tsx
import { Fragment, Suspense } from "react";
import { Route, Routes } from "react-router-dom";
import type { AppRouteObject } from "./types";
import { RequireAuth } from "@/guards/RequireAuth";
import { RequireRole } from "@/guards/RequireRole";

function withGuards(element: React.ReactElement, meta?: AppRouteObject["meta"]) {
    let wrapped = element;
    if (meta?.roles && meta.roles.length > 0) {
        wrapped = <RequireRole roles={meta.roles}>{wrapped}</RequireRole>;
    }
    if (meta?.requiresAuth) {
        wrapped = <RequireAuth>{wrapped}</RequireAuth>;
    }
    return wrapped;
}

function renderTree(routes: AppRouteObject[]) {
    const walk = (rts: AppRouteObject[]) =>
        rts.map((r, idx) => {
            const Element = r.element ? (
                <Suspense fallback={<div className="p-6">Loading...</div>}>
                    {withGuards(<Fragment>{r.element as React.ReactElement}</Fragment>, r.meta)}
                </Suspense>
            ) : undefined;

            if (r.index) {
                return <Route key={`index-${idx}`} index element={Element} />;
            }
            return (
                <Route key={r.path ?? idx} path={r.path} element={Element}>
                    {r.children ? walk(r.children) : null}
                </Route>
            );
        });

    return walk(routes);
}

export function RenderRoutes({ routes }: { routes: AppRouteObject[] }) {
    return <Routes>{renderTree(routes)}</Routes>;
}
