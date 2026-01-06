import React, { createContext, useContext, useEffect, useMemo, useState } from "react";
import ReactDOM from "react-dom";
import clsx from "clsx";

type Side = "start" | "end" | "top" | "bottom";

type DrawerContextType = {
    open: boolean;
    setOpen: (v: boolean) => void;
    side: Side;
    setSide: (v: Side) => void;
    title?: React.ReactNode;
    setTitle: (t?: React.ReactNode) => void;
};

const DrawerContext = createContext<DrawerContextType | null>(null);
const useDrawerCtx = () => {
    const ctx = useContext(DrawerContext);
    if (!ctx) throw new Error("Drawer compound components must be used within <Drawer>");
    return ctx;
};

type DrawerProps = {
    defaultOpen?: boolean;
    open?: boolean; // controlled kullanım için
    onOpenChange?: (open: boolean) => void;
    side?: Side;
    title?: React.ReactNode;
    children: React.ReactNode;
    portalContainer?: Element; // varsayılan: document.body
};

export function Drawer({
                           defaultOpen = false,
                           open: controlledOpen,
                           onOpenChange,
                           side: initialSide = "end",
                           title: initialTitle,
                           children,
                           portalContainer,
                       }: DrawerProps) {
    const [uncontrolledOpen, setUncontrolledOpen] = useState(defaultOpen);
    const open = controlledOpen ?? uncontrolledOpen;

    const setOpen = (v: boolean) => {
        if (onOpenChange) onOpenChange(v);
        if (controlledOpen === undefined) setUncontrolledOpen(v);
    };

    const [side, setSide] = useState<Side>(initialSide);
    const [title, setTitle] = useState<React.ReactNode>(initialTitle);

    const value = useMemo<DrawerContextType>(
        () => ({ open, setOpen, side, setSide, title, setTitle }),
        [open, side, title]
    );

    // ESC ile kapat
    useEffect(() => {
        if (!open) return;
        const onKey = (e: KeyboardEvent) => {
            if (e.key === "Escape") setOpen(false);
        };
        window.addEventListener("keydown", onKey);
        return () => window.removeEventListener("keydown", onKey);
    }, [open]);

    return (
        <DrawerContext.Provider value={value}>
            {children}
            <DrawerPortal container={portalContainer}>
                <DrawerOverlay />
                <DrawerPanel />
            </DrawerPortal>
        </DrawerContext.Provider>
    );
}

/* ------ Trigger ---------------------------------------------------------- */
Drawer.Trigger = function DrawerTrigger({
                                            asChild,
                                            children,
                                            side,
                                            title,
                                        }: {
    asChild?: boolean;
    children: React.ReactNode;
    side?: Side;
    title?: React.ReactNode;
}) {
    const { setOpen, setSide, setTitle } = useDrawerCtx();
    const child = React.Children.only(children) as React.ReactElement<{ onClick?: (e: React.MouseEvent) => void }>;
        if (asChild && React.isValidElement(child)) {
        return React.cloneElement(child, {
            onClick: (e: any) => {
                setSide(side ?? "end");
                setTitle(title);
                setOpen(true);
                child.props?.onClick?.(e);
            },
        });
    }
    return (
        <button
            type="button"
            onClick={() => {
                setSide(side ?? "end");
                setTitle(title);
                setOpen(true);
            }}
            className="px-3 py-2 rounded-md bg-blue-600 text-white hover:bg-blue-700"
        >
            {children}
        </button>
    );
};

/* ------ Portal + Overlay + Panel ---------------------------------------- */
function DrawerPortal({
                          children,
                          container,
                      }: {
    children: React.ReactNode;
    container?: Element;
}) {
    const target = container ?? (typeof document !== "undefined" ? document.body : undefined);
    if (!target) return null;
    return ReactDOM.createPortal(children, target);
}

function DrawerOverlay() {
    const { open, setOpen } = useDrawerCtx();
    return (
        <div
            aria-hidden="true"
            className={clsx(
                "fixed inset-0 z-50 bg-black/40 transition-opacity",
                open ? "opacity-100 pointer-events-auto" : "opacity-0 pointer-events-none"
            )}
            onClick={() => setOpen(false)}
        />
    );
}

function DrawerPanel() {
    const { open, side } = useDrawerCtx();
    const base = "fixed z-50 bg-white shadow-lg transition-transform duration-300 ease-in-out flex flex-col";

    const pos = {
        end: "inset-y-0 right-0 w-full md:w-96",
        start: "inset-y-0 left-0 w-full md:w-96",
        bottom: "left-0 right-0 bottom-0 h-[80vh] md:h-80",
        top: "left-0 right-0 top-0 h-[80vh] md:h-80",
    }[side];

    const trans = {
        end: open ? "translate-x-0" : "translate-x-full",
        start: open ? "translate-x-0" : "-translate-x-full",
        bottom: open ? "translate-y-0" : "translate-y-full",
        top: open ? "translate-y-0" : "-translate-y-full",
    }[side];

    return <div className={clsx(base, pos, trans)} role="dialog" aria-modal={open} aria-hidden={!open} />;
}

/* ------ Header / Body / Footer slots ------------------------------------ */
Drawer.Header = function DrawerHeader({
                                          children,
                                          className,
                                      }: {
    children?: React.ReactNode;
    className?: string;
}) {
    const { open, setOpen, title } = useDrawerCtx();
    if (!open) return null;
    return (
        <div className={clsx("fixed z-[60] w-full md:w-96 bg-white border-b border-gray-200", className)}>
            <div className="flex items-center justify-between p-4">
                <div className="text-base font-semibold">{children ?? title}</div>
                <button
                    type="button"
                    onClick={() => setOpen(false)}
                    aria-label="Close"
                    className="p-2 rounded hover:bg-gray-100"
                >
                    <svg viewBox="0 0 24 24" className="w-4 h-4" aria-hidden="true">
                        <path stroke="currentColor" strokeWidth="2" strokeLinecap="round" d="M6 6l12 12M18 6L6 18" />
                    </svg>
                </button>
            </div>
        </div>
    );
};

Drawer.Body = function DrawerBody({
                                      children,
                                      className,
                                  }: {
    children: React.ReactNode;
    className?: string;
}) {
    const { open, side } = useDrawerCtx();
    if (!open) return null;

    // Header 56px + Footer 64px boşluk bırak
    const padTop = "pt-14";
    const padBottom = "pb-16";

    const height =
        side === "end" || side === "start" ? "h-full" : "h-[80vh] md:h-80";

    return (
        <div className={clsx("relative z-[55] overflow-y-auto", height, padTop, padBottom, className)}>
            <div className="p-4">{children}</div>
        </div>
    );
};

Drawer.Footer = function DrawerFooter({
                                          children,
                                          className,
                                      }: {
    children: React.ReactNode;
    className?: string;
}) {
    const { open } = useDrawerCtx();
    if (!open) return null;
    return (
        <div
            className={clsx(
                "fixed z-[60] bottom-0 w-full md:w-96 bg-white border-t border-gray-200",
                className
            )}
        >
            <div className="flex items-center justify-end gap-2 p-3">{children}</div>
        </div>
    );
};
