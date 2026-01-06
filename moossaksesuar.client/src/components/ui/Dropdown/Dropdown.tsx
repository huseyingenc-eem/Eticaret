// Dropdown.tsx
import React, {
    createContext, useContext, useMemo, useState, useImperativeHandle,
    forwardRef, type ReactNode, type ElementType, useEffect
} from "react";
import { createPortal } from "react-dom";
import {
    useFloating, offset, flip, shift, autoUpdate,
    useClick, useDismiss, useRole, useInteractions,
    type Placement
} from "@floating-ui/react";

/* ---------- Tipler ---------- */

type Handle = { open: () => void; close: () => void; toggle: () => void };

type DropdownProps = {
    as?: ElementType;
    className?: string;
    children?: ReactNode;
    placement?: Placement;
    offsetPx?: number;
    portalId?: string;
};

type Interactions = ReturnType<typeof useInteractions>;
type GetReferenceProps = Interactions["getReferenceProps"];
type GetFloatingProps = Interactions["getFloatingProps"];

type Ctx = {
    open: boolean;
    setOpen: (v: boolean) => void;
    refs: {
        setReference: (node: HTMLElement | null) => void;
        setFloating: (node: HTMLElement | null) => void;
    };
    floatingStyles: React.CSSProperties;
    getReferenceProps: GetReferenceProps;
    getFloatingProps: GetFloatingProps;
    portalEl: HTMLElement | null;
    isPositioned: boolean;
    x: number | null;
    y: number | null;
};

const Ctx = createContext<Ctx | null>(null);
const useDD = () => {
    const v = useContext(Ctx);
    if (!v) throw new Error("Use Dropdown.Trigger/Content inside <Dropdown>.");
    return v;
};

/* ---------- Ana Dropdown ---------- */

const _Dropdown = forwardRef<Handle, DropdownProps>(function Dropdown(
    { as: Component = "div", className, children, placement = "bottom-end", offsetPx = 8, portalId },
    ref
) {
    const [open, setOpen] = useState(false);

    const { refs, floatingStyles, context, x, y, isPositioned } = useFloating({
        open,
        onOpenChange: setOpen,
        placement,
        strategy: "fixed",
        middleware: [offset(offsetPx), flip({ padding: 8 }), shift({ padding: 8 })],
        whileElementsMounted: autoUpdate,
    });

    const click = useClick(context);
    const dismiss = useDismiss(context, { outsidePress: true, escapeKey: true });
    const role = useRole(context, { role: "menu" });
    const { getReferenceProps, getFloatingProps } = useInteractions([click, dismiss, role]);

    useImperativeHandle(ref, () => ({
        open: () => setOpen(true),
        close: () => setOpen(false),
        toggle: () => setOpen(v => !v),
    }), []);

    const portalEl = useMemo(() => {
        if (typeof document === "undefined") return null;
        if (portalId) {
            const el = document.getElementById(portalId);
            if (el) return el;
        }
        return document.body;
    }, [portalId]);


    const ctx: Ctx = {
        open,
        setOpen,
        refs: {
            setReference: refs.setReference as (n: HTMLElement | null) => void,
            setFloating: refs.setFloating as (n: HTMLElement | null) => void,
        },
        floatingStyles,
        getReferenceProps,
        getFloatingProps,
        portalEl,
        x,
        y,
        isPositioned,
    };

    return (
        <Ctx.Provider value={ctx}>
            <Component className={className}>{children}</Component>
        </Ctx.Provider>
    );
});

/* ---------- Trigger ---------- */

type TriggerProps = {
    as?: ElementType;
    className?: string;
    children: ReactNode;
} & React.HTMLAttributes<HTMLElement>;

const Trigger = forwardRef<HTMLElement, TriggerProps>(function Trigger(
    { as: Comp = "button", className, children, ...rest },
    fRef
) {
    const { refs, getReferenceProps, open } = useDD();

    return (
        <Comp
            {...getReferenceProps(rest)}
            ref={(el: any) => {
                refs.setReference(el);
                if (typeof fRef === "function") fRef(el);
                else if (fRef) (fRef as React.MutableRefObject<HTMLElement | null>).current = el;
            }}
            aria-haspopup="menu"
            aria-expanded={open}
            className={className}
        >
            {children}
        </Comp>
    );
});

/* ---------- Content ---------- */

type ContentProps = {
    as?: ElementType;
    className?: string;
    children: ReactNode;
    closeOnClickInside?: boolean;
} & React.HTMLAttributes<HTMLDivElement>;

const Content = forwardRef<HTMLDivElement, ContentProps>(function Content(
    { as: Comp = "div", className, children, closeOnClickInside = true, onClick, ...rest },
    fRef
) {
    const { open, setOpen, refs, floatingStyles, getFloatingProps, portalEl, isPositioned } = useDD();
    
    // Animasyonun başlamaya hazır olup olmadığını kontrol eden yeni state
    const [isReadyToAnimate, setIsReadyToAnimate] = useState(false);

    useEffect(() => {
        // Dropdown kapandığında animasyon durumunu sıfırla
        if (!open) {
            setIsReadyToAnimate(false);
        }
    }, [open]);

    useEffect(() => {
        // Pozisyon hesaplandığında, bir sonraki animasyon karesinde animasyonu tetikle
        if (isPositioned) {
            const frameId = requestAnimationFrame(() => {
                setIsReadyToAnimate(true);
            });
            return () => cancelAnimationFrame(frameId);
        }
    }, [isPositioned]);

    // Eğer `open` değilse, hiçbir şey render etme
    if (!open) {
        return null;
    }
    
    const baseClasses = [
        "z-[9999]", "bg-white backdrop-blur-md",
        "border border-slate-200/60 dark:border-slate-700/60",
        "shadow-xl shadow-slate-900/10 dark:shadow-slate-900/30",
        "rounded-xl", "p-2",
        "ring-1 ring-slate-950/5 dark:ring-slate-50/10"
    ];

    const animationClasses = "animate-in fade-in-0 zoom-in-95 duration-200";
    const closingAnimationClasses = "data-[state=closed]:animate-out data-[state=closed]:fade-out-0 data-[state=closed]:zoom-out-95";

    // Animasyona hazırsa animasyon class'larını ekle, değilse şeffaf yap
    const dynamicClasses = isReadyToAnimate
        ? `${animationClasses} ${closingAnimationClasses}`
        : "opacity-0";
    
    const finalClasses = [...baseClasses, dynamicClasses, className].filter(Boolean).join(" ");

    const node = (
        <Comp
            {...getFloatingProps(rest)}
            ref={(el: any) => {
                refs.setFloating(el);
                if (typeof fRef === "function") fRef(el);
                else if (fRef) (fRef as React.MutableRefObject<HTMLDivElement | null>).current = el;
            }}
            // Her zaman doğru pozisyona yerleştir, görünürlüğü class ile kontrol et
            style={floatingStyles}
            className={finalClasses}
            onClick={(e: any) => {
                onClick?.(e);
                if (closeOnClickInside) setOpen(false);
            }}
            role="menu"
        >
            {children}
        </Comp>
    );

    return portalEl ? createPortal(node, portalEl) : node;
});


interface DropdownCompound extends React.ForwardRefExoticComponent<
    DropdownProps & React.RefAttributes<Handle>
> {
    Trigger: typeof Trigger;
    Content: typeof Content;
}

const Dropdown = _Dropdown as DropdownCompound;
Dropdown.Trigger = Trigger;
Dropdown.Content = Content;

export { Dropdown };
export default Dropdown;