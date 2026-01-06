import { useMemo } from "react";
import { useAuth } from "@/hooks/useAuth";
import { storeNavigation, adminNavigation } from "@/configs/navigation";
import { filterNavTree, type FeatureFlags } from "./_navFilters";
import type { FooterColumn } from "@/configs/types";

/**
 * STORE için navigation (varsayılan).
 * usage: const { header, footer } = useNavigation();
 */
export function useNavigation(flags?: FeatureFlags) {
    const { user, isAuthenticated } = useAuth();
    const roles = user?.roles;

    const header = useMemo(
        () => filterNavTree(storeNavigation.header, { roles, isAuthenticated, flags }),
        [roles, isAuthenticated, flags]
    );

    const footer: FooterColumn[] = useMemo(
        () =>
            (storeNavigation.footer ?? []).map((col) => ({
                ...col,
                links: filterNavTree(col.links, { roles, isAuthenticated, flags }),
            })),
        [roles, isAuthenticated, flags]
    );

    return { header, footer };
}

/**
 * ADMIN için navigation.
 * usage: const { sidebar, topbar } = useAdminNavigation();
 */
export function useAdminNavigation(flags?: FeatureFlags) {
    const { user, isAuthenticated } = useAuth();
    const roles = user?.roles;

    const sidebar = useMemo(
        () => filterNavTree(adminNavigation.sidebar, { roles, isAuthenticated, flags }),
        [roles, isAuthenticated, flags]
    );
    const topbar = useMemo(
        () => filterNavTree(adminNavigation.topbar, { roles, isAuthenticated, flags }),
        [roles, isAuthenticated, flags]
    );

    return { sidebar, topbar };
}
