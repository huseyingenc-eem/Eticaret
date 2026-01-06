import { lazy } from "react";

export const Dashboard = lazy(() => import("@/pages/admin/Dashboard.tsx"));
export const Categories = lazy(() => import("@/pages/admin/categories"));
export const CategoryDetailPage  = lazy(() => import("@/pages/admin/categories/CategoryDetailPage.tsx"));
export const Customers = lazy(() => import("@/pages/admin/customers"))
export const Suppliers = lazy(() => import("@/pages/admin/suppliers"))


export const ButtonPage  = lazy(() => import("@/pages/admin/ui/Basic/ButtonPage"));



export const TimelinePage  = lazy(() => import("@/pages/admin/ui/DataDisplay/TimelinePage"));






