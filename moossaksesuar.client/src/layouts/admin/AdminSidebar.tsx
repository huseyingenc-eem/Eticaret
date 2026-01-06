import { useEffect, useMemo, useState, isValidElement, cloneElement } from "react";
import { NavLink as RouterNavLink, useLocation } from "react-router-dom";
import {
    type LucideIcon,
    ChevronDown, Folder, FolderOpen, CircleDot,
    LayoutDashboard, Settings, Users, FileText, Package, ShoppingCart,
    Crown, ArrowUpRight
} from "lucide-react";
import clsx from "clsx";
import { useAdminNavigation } from "@/hooks/useNavigation";
import type { NavLink } from "@/configs";
import { useAdminShell } from "./_ui/AdminShellContext";
import AdminLogo from "./_ui/AdminLogo";

function useIsActive(path?: string, children?: NavLink[]): boolean {
    const { pathname } = useLocation();
    return useMemo(() => {
        const walk = (p?: string, kids?: NavLink[]): boolean => {
            if (p && pathname.startsWith(p)) return true;
            if (!kids || kids.length === 0) return false;
            return kids.some((c) => walk(c.to, c.children));
        };
        return walk(path, children);
    }, [pathname, path, children]);
}

function SidebarLink({
                         to,
                         external,
                         className,
                         children,
                     }: {
    to?: string;
    external?: boolean;
    className?: string;
    children: React.ReactNode;
}) {
    const base =
        "group flex items-center gap-3 px-3 py-2.5 rounded-2xl transition-colors duration-100 ease-out focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-primary/30 focus-visible:ring-offset-2 focus-visible:ring-offset-background relative";

    if (external && to) {
        return (
            <a
                href={to}
                target="_blank"
                rel="noreferrer"
                className={clsx(
                    base,
                    "text-slate-700 hover:text-primary hover:bg-slate-50",
                    className
                )}
            >
                {children}
            </a>
        );
    }

    return (
        <RouterNavLink
            to={to || "#"}
            end
            className={({ isActive }) =>
                clsx(
                    base,
                    "text-slate-700 hover:text-primary hover:bg-slate-50",
                    isActive && "bg-primary/10 text-primary border border-primary/20 font-semibold",
                    className
                )
            }
        >
            {children}
        </RouterNavLink>
    );
}

// --- Utility Functions ---
function depthTextSize(depth: number) {
    if (depth >= 2) return "text-xs font-medium";
    if (depth === 1) return "text-sm font-medium";
    return "text-sm font-semibold";
}

function depthIconSize(depth: number) {
    if (depth >= 2) return "w-4 h-4";
    if (depth === 1) return "w-4 h-4";
    return "w-5 h-5";
}

function depthPadding(depth: number, collapsed: boolean) {
    if (collapsed) return "";
    if (depth >= 2) return "pl-10";
    if (depth === 1) return "pl-6";
    return "";
}

const ICONS: Record<string, LucideIcon> = {
    dashboard: LayoutDashboard,
    settings: Settings,
    users: Users,
    file: FileText,
    products: Package,
    cart: ShoppingCart,
};

// --- Icon Component ---
function ItemIcon({
                      item,
                      depth = 0,
                      expanded = false,
                  }: {
    item: NavLink;
    depth?: number;
    expanded?: boolean;
}) {
    const anyItem = item as any;
    const sizeCls = depthIconSize(depth);

    // 1) URL icon
    if (anyItem.iconSrc) {
        return (
            <img
                src={anyItem.iconSrc}
                alt=""
                aria-hidden="true"
                className={clsx("shrink-0 object-contain", sizeCls)}
            />
        );
    }

    // 2) icon prop: string | component type | element
    const prop = anyItem.icon ?? anyItem.iconName;

    if (typeof prop === "string") {
        const Icon = ICONS[prop.toLowerCase()];
        if (Icon) {
            return <Icon className={clsx("shrink-0", sizeCls)} aria-hidden="true" />;
        }
    }

    if (typeof prop === "function") {
        const Icon = prop as LucideIcon;
        return <Icon className={clsx("shrink-0", sizeCls)} aria-hidden="true" />;
    }

    if (isValidElement(prop)) {
        return cloneElement(prop, {
            ...((prop as any).props ?? {}),
            className: clsx("shrink-0", sizeCls, (prop as any).props?.className),
            "aria-hidden": true,
        });
    }

    const hasChildren = !!item.children?.length;
    const Fallback = hasChildren ? (expanded ? FolderOpen : Folder) : CircleDot;
    return <Fallback className={clsx("shrink-0", sizeCls)} aria-hidden="true" />;
}

function Leaf({ item, depth = 0 }: { item: NavLink; depth?: number }) {
    const { collapsed } = useAdminShell();
    const cls = clsx("group relative", depthPadding(depth, collapsed), collapsed && "justify-center");

    return (
        <SidebarLink to={item.to} external={(item as any).external} className={cls}>
            <ItemIcon item={item} depth={depth} />
            {!collapsed && (
                <>
          <span
              className={clsx(
                  "whitespace-nowrap truncate transition-colors duration-100 group-hover:text-primary",
                  depthTextSize(depth)
              )}
          >
            {item.label}
          </span>
                    {(item as any).badge && (
                        <span className="ml-auto inline-flex items-center justify-center px-2.5 py-1 text-xs font-bold text-white bg-primary rounded-full">
              {(item as any).badge}
            </span>
                    )}
                </>
            )}
            {collapsed && (
                <div className="absolute left-full ml-3 px-3 py-2 bg-white text-slate-800 text-sm rounded-lg border border-slate-200 opacity-0 invisible group-hover:opacity-100 group-hover:visible transition-opacity duration-100 z-50 whitespace-nowrap font-medium">
                    {item.label}
                    <div className="absolute left-0 top-1/2 -translate-y-1/2 -translate-x-1 w-2 h-2 bg-white rotate-45 border-l border-t border-slate-200" />
                </div>
            )}
        </SidebarLink>
    );
}

function Node({
                  item,
                  expanded,
                  toggle,
                  depth = 0,
              }: {
    item: NavLink;
    expanded: boolean;
    toggle: () => void;
    depth?: number;
}) {
    const { collapsed } = useAdminShell();
    const active = useIsActive(item.to, item.children);
    const hasBorder = (item as any).separator;

    return (
        <div>
            <button
                type="button"
                onClick={collapsed ? undefined : toggle}
                aria-expanded={expanded}
                className={clsx(
                    "group flex items-center w-full px-3 py-2.5 rounded-2xl transition-colors duration-100 ease-out relative",
                    "text-slate-700 hover:text-primary hover:bg-slate-50",
                    active && "bg-primary/10 text-primary border border-primary/20 font-semibold",
                    hasBorder && "after:absolute after:bottom-0 after:left-4 after:right-4 after:h-px after:bg-slate-200",
                    depthPadding(depth, collapsed),
                    collapsed && "justify-center"
                )}
            >
                <ItemIcon item={item} depth={depth} expanded={expanded} />
                {!collapsed && (
                    <>
            <span
                className={clsx(
                    "flex-1 ml-3 text-left truncate transition-colors duration-100 group-hover:text-primary",
                    depthTextSize(depth)
                )}
            >
              {item.label}
            </span>
                        <ChevronDown
                            className={clsx(
                                depthIconSize(depth),
                                "text-slate-400 transition-transform duration-100",
                                expanded ? "rotate-180" : "rotate-0"
                            )}
                        />
                    </>
                )}
                {collapsed && (
                    <div className="absolute left-full ml-3 px-3 py-2 bg-white text-slate-800 text-sm rounded-lg border border-slate-200 opacity-0 invisible group-hover:opacity-100 group-hover:visible transition-opacity duration-100 z-50 whitespace-nowrap font-medium">
                        {item.label}
                        <div className="absolute left-0 top-1/2 -translate-y-1/2 -translate-x-1 w-2 h-2 bg-white rotate-45 border-l border-t border-slate-200" />
                    </div>
                )}
            </button>

            {!collapsed && expanded && (
                <div className="mt-1 space-y-0.5">
                    {item.children?.map((c) => (
                        <div key={c.id}>
                            {c.children && c.children.length > 0 ? (
                                <Collapsible item={c} depth={depth + 1} />
                            ) : (
                                <Leaf item={c} depth={depth + 1} />
                            )}
                        </div>
                    ))}
                </div>
            )}
        </div>
    );
}

function Collapsible({ item, depth = 0 }: { item: NavLink; depth?: number }) {
    const active = useIsActive(item.to, item.children);
    const [open, setOpen] = useState<boolean>(active);

    useEffect(() => setOpen(active), [active]);

    return <Node item={item} expanded={open} toggle={() => setOpen((s) => !s)} depth={depth} />;
}

// Footer (hafifletildi)
function UpgradeFooter({ collapsed }: { collapsed: boolean }) {
    if (collapsed) {
        return (
            <div className="p-3">
                <button className="w-full p-3 rounded-2xl bg-gradient-to-br from-indigo-600 to-purple-600 text-white transition-colors duration-100 hover:from-indigo-700 hover:to-purple-700">
                    <Crown className="w-5 h-5 mx-auto" />
                </button>
            </div>
        );
    }

    return (
        <div className="p-4 border-t border-slate-200">
            <div className="p-5 rounded-3xl bg-gradient-to-br from-indigo-50 via-purple-50 to-pink-50 border border-slate-200">
                <div className="flex items-center gap-4 mb-4">
                    <div className="p-2.5 rounded-2xl bg-gradient-to-br from-indigo-600 to-purple-600">
                        <Crown className="w-5 h-5 text-white" />
                    </div>
                    <div className="flex-1">
                        <h4 className="text-sm font-bold text-slate-800">Upgrade to Pro</h4>
                        <p className="text-xs text-slate-600">Unlock advanced features</p>
                    </div>
                </div>

                <button className="w-full px-4 py-3 rounded-2xl bg-gradient-to-r from-indigo-600 to-purple-600 text-white text-sm font-bold transition-colors duration-100 hover:from-indigo-700 hover:to-purple-700 flex items-center justify-center gap-2">
                    <span>Upgrade Now</span>
                    <ArrowUpRight className="w-4 h-4" />
                </button>
            </div>
        </div>
    );
}

export default function AdminSidebar() {
    const { sidebar } = useAdminNavigation();
    const { sidebarOpen, closeSidebar, collapsed } = useAdminShell();

    return (
        <>
            {/* Mobile Overlay */}
            <div
                className={clsx(
                    "fixed inset-0 z-30 bg-slate-900/60 sm:hidden transition-opacity duration-100",
                    sidebarOpen ? "opacity-100 visible" : "opacity-0 invisible"
                )}
                onClick={closeSidebar}
            />

            {/* Sidebar */}
            <aside
                id="admin-sidebar"
                aria-label="Sidebar"
                className={clsx(
                    "fixed top-0 left-0 z-40 h-screen flex flex-col transition-transform duration-100",
                    "bg-white border-r border-slate-200",
                    collapsed ? "w-20" : "w-72",
                    sidebarOpen ? "translate-x-0" : "-translate-x-full",
                    "sm:translate-x-0"
                )}
            >
                {/* Header with Logo */}
                <div className="h-20 flex items-center justify-center border-b border-slate-200 bg-white">
                    <AdminLogo collapsed={collapsed} />
                </div>

                {/* Navigation */}
                <div className="flex-1 overflow-y-auto overflow-x-hidden custom-scrollbar">
                    <div className={clsx("p-4", collapsed && "p-3")}>
                        <nav className="space-y-1.5">
                            {sidebar.map((item) => {
                                const hasChildren = !!item.children && item.children.length > 0;
                                return (
                                    <div key={item.id}>
                                        {hasChildren ? <Collapsible item={item} depth={0} /> : <Leaf item={item} depth={0} />}
                                    </div>
                                );
                            })}
                        </nav>
                    </div>
                </div>

                {/* Footer */}
                <UpgradeFooter collapsed={collapsed} />
            </aside>

        </>
    );
}
