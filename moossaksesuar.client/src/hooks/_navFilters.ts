import type { NavLink } from "@/configs/types";

export type FeatureFlags = Record<string, boolean>;

const byOrder = (a: NavLink, b: NavLink) => (a.order ?? 0) - (b.order ?? 0);

function hasRole(userRoles: string[] | undefined, itemRoles?: string[]) {
    if (!itemRoles || itemRoles.length === 0) return true;
    if (!userRoles) return false;
    return itemRoles.some((r) => userRoles.includes(r));
}

function visibleForAuth(isAuthenticated: boolean, v?: "always" | "auth" | "guest") {
    if (!v || v === "always") return true;
    if (v === "auth") return isAuthenticated;
    if (v === "guest") return !isAuthenticated;
    return true;
}

function featureEnabled(flags: FeatureFlags | undefined, key?: string) {
    if (!key) return true;
    return !!flags?.[key];
}

export function filterNavTree(
    items: NavLink[] = [],
    ctx: { roles?: string[]; isAuthenticated: boolean; flags?: FeatureFlags }
): NavLink[] {
    return items
        .filter((i) => !i.hidden)
        .filter((i) => visibleForAuth(ctx.isAuthenticated, i.visibility))
        .filter((i) => hasRole(ctx.roles, i.roles))
        .filter((i) => featureEnabled(ctx.flags, i.feature))
        .map((i) => ({
            ...i,
            children: i.children ? filterNavTree(i.children, ctx) : undefined,
        }))
        .filter((i) => (i.children ? i.children.length > 0 || i.to : true))
        .sort(byOrder);
}
