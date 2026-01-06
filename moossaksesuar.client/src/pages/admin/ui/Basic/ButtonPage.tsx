import React, { useMemo, useRef, useState } from "react";
import { Save, Upload, Loader2, ArrowRight, RefreshCw, Check, X, ExternalLink, Star } from "lucide-react";
import Button from "@/components/ui";

import { Spinner, GhostSpinner } from "./Spinner"; // uses your local Spinner.tsx

function Badge({ children }: { children: React.ReactNode }) {
    return <span className="inline-flex items-center rounded-full border px-2 py-0.5 text-xs font-medium opacity-80">{children}</span>;
}

const COLOR_OPTIONS = ["neutral", "primary", "success", "warning", "danger", "info", "secondary"] as const;
const VARIANT_OPTIONS = ["filled", "soft", "outline", "ghost"] as const;
const COMPONENT_OPTIONS = ["button", "a", "div"] as const;

export default function ButtonSpinnerPlayground() {
    // --- Controls state
    const [label, setLabel] = useState("Click me");
    const [color, setColor] = useState<(typeof COLOR_OPTIONS)[number]>("neutral");
    const [variant, setVariant] = useState<(typeof VARIANT_OPTIONS)[number]>("filled");
    const [component, setComponent] = useState<(typeof COMPONENT_OPTIONS)[number]>("button");
    const [isGlow, setIsGlow] = useState(false);
    const [isIcon, setIsIcon] = useState(false);
    const [loading, setLoading] = useState(false);
    const [disabled, setDisabled] = useState(false);
    const [unstyled, setUnstyled] = useState(false);
    const [btnType, setBtnType] = useState<"button" | "submit" | "reset">("button");

    const demoHref = "#playground"; // used when component === "a"

    const focusRef = useRef<HTMLButtonElement | null>(null);

    // For prettier code sample preview
    const codePreview = useMemo(() => {
        const lines: string[] = [];
        const props: string[] = [];
        if (component !== "button") props.push(`component=\"${component}\"`);
        if (unstyled) props.push("unstyled");
        if (!unstyled) {
            if (color !== "neutral") props.push(`color=\"${color}\"`);
            if (variant !== "filled") props.push(`variant=\"${variant}\"`);
            if (isGlow) props.push("isGlow");
            if (isIcon) props.push("isIcon");
        }
        if (loading) props.push("loading");
        if (disabled) props.push("disabled");
        if (btnType !== "button" && component === "button") props.push(`type=\"${btnType}\"`);

        lines.push(`<Button ${props.join(" ")}${component === "a" ? " href=\"#\"" : ""}>${label}</Button>`);
        return lines.join("\n");
    }, [label, color, variant, component, isGlow, isIcon, loading, disabled, unstyled, btnType]);

    return (
        <div className="min-h-screen w-full bg-gradient-to-b from-slate-950 via-slate-900 to-slate-950 text-slate-100 p-6 sm:p-10">
            <div className="mx-auto max-w-6xl">
                <header className="mb-8 flex flex-col gap-2">
                    <h1 className="text-2xl sm:text-3xl font-semibold tracking-tight">Design System Playground</h1>
                    <p className="text-slate-300">Everything-on-one-page demo for <Badge>Button</Badge> and <Badge>Spinner</Badge>.</p>
                </header>

                {/* CONTROL PANEL */}
                <section className="mb-8 grid gap-6 rounded-2xl border border-white/10 bg-white/5 p-4 sm:p-6 shadow-xl backdrop-blur">
                    <h2 className="text-lg font-semibold">Controls</h2>
                    <div className="grid grid-cols-1 md:grid-cols-3 lg:grid-cols-4 gap-4">
                        <label className="grid gap-1 text-sm">
                            <span>Label</span>
                            <input value={label} onChange={(e) => setLabel(e.target.value)} className="rounded-xl bg-white/10 px-3 py-2 outline-none ring-0 focus:bg-white/15" />
                        </label>

                        <label className="grid gap-1 text-sm">
                            <span>Color</span>
                            <select value={color} onChange={(e) => setColor(e.target.value as any)} className="rounded-xl bg-white/10 px-3 py-2">
                                {COLOR_OPTIONS.map((c) => (
                                    <option key={c} value={c}>{c}</option>
                                ))}
                            </select>
                        </label>

                        <label className="grid gap-1 text-sm">
                            <span>Variant</span>
                            <select value={variant} onChange={(e) => setVariant(e.target.value as any)} className="rounded-xl bg-white/10 px-3 py-2">
                                {VARIANT_OPTIONS.map((v) => (
                                    <option key={v} value={v}>{v}</option>
                                ))}
                            </select>
                        </label>

                        <label className="grid gap-1 text-sm">
                            <span>Component</span>
                            <select value={component} onChange={(e) => setComponent(e.target.value as any)} className="rounded-xl bg-white/10 px-3 py-2">
                                {COMPONENT_OPTIONS.map((v) => (
                                    <option key={v} value={v}>{v}</option>
                                ))}
                            </select>
                        </label>

                        <label className="flex items-center gap-2 text-sm">
                            <input type="checkbox" checked={isGlow} onChange={(e) => setIsGlow(e.target.checked)} /> isGlow
                        </label>
                        <label className="flex items-center gap-2 text-sm">
                            <input type="checkbox" checked={isIcon} onChange={(e) => setIsIcon(e.target.checked)} /> isIcon
                        </label>
                        <label className="flex items-center gap-2 text-sm">
                            <input type="checkbox" checked={loading} onChange={(e) => setLoading(e.target.checked)} /> loading
                        </label>
                        <label className="flex items-center gap-2 text-sm">
                            <input type="checkbox" checked={disabled} onChange={(e) => setDisabled(e.target.checked)} /> disabled
                        </label>
                        <label className="flex items-center gap-2 text-sm">
                            <input type="checkbox" checked={unstyled} onChange={(e) => setUnstyled(e.target.checked)} /> unstyled
                        </label>

                        {component === "button" && (
                            <label className="grid gap-1 text-sm">
                                <span>type</span>
                                <select value={btnType} onChange={(e) => setBtnType(e.target.value as any)} className="rounded-xl bg-white/10 px-3 py-2">
                                    <option value="button">button</option>
                                    <option value="submit">submit</option>
                                    <option value="reset">reset</option>
                                </select>
                            </label>
                        )}
                    </div>

                    {/* Live demo button */}
                    <div className="mt-4 flex flex-wrap items-center gap-3">
                        <Button
                            // Polymorphic rendering
                            component={component as any}
                            href={component === "a" ? demoHref : undefined}
                            role={component === "div" ? "button" : undefined}
                            tabIndex={component === "div" ? 0 : undefined}
                            // Styling / behaviors
                            color={color as any}
                            variant={variant as any}
                            isGlow={isGlow}
                            isIcon={isIcon}
                            loading={loading}
                            disabled={disabled}
                            unstyled={unstyled}
                            type={component === "button" ? btnType : undefined}
                            className={unstyled ? "rounded-2xl border px-4 py-2 hover:bg-white/10 active:scale-[.99]" : undefined}
                            onClick={() => focusRef.current?.focus()}
                        >
                            {isIcon ? <Save className="h-4 w-4" /> : <span className="inline-flex items-center gap-2">{label} <ArrowRight className="h-4 w-4" /></span>}
                        </Button>

                        <Badge>
                            data-variant=<code className="ml-1">{variant}</code>
                        </Badge>
                        <Badge>
                            data-color=<code className="ml-1">{color}</code>
                        </Badge>

                        <button
                            ref={focusRef}
                            onClick={() => alert("Ref focus target clicked ✅")}
                            className="ml-auto rounded-xl border border-white/10 bg-white/5 px-3 py-1.5 text-sm hover:bg-white/10"
                        >Focus target</button>
                    </div>

                    {/* Code preview */}
                    <pre className="mt-4 overflow-x-auto rounded-xl bg-black/60 p-4 text-xs text-slate-200">
            <code>{codePreview}</code>
          </pre>
                </section>

                {/* BUTTON SHOWCASE */}
                <section className="mb-8 rounded-2xl border border-white/10 bg-white/5 p-4 sm:p-6 shadow-xl backdrop-blur">
                    <div className="mb-3 flex items-center justify-between">
                        <h2 className="text-lg font-semibold">Buttons — Matrix of colors × variants</h2>
                        <p className="text-xs text-slate-300">If any item looks plain, your theme may not define that combination.</p>
                    </div>
                    <div className="grid gap-6">
                        {VARIANT_OPTIONS.map((v) => (
                            <div key={v} className="grid grid-cols-2 sm:grid-cols-3 md:grid-cols-4 lg:grid-cols-6 gap-3">
                                {COLOR_OPTIONS.map((c) => (
                                    <Button key={`${v}-${c}`} color={c as any} variant={v as any}>
                                        {c} / {v}
                                    </Button>
                                ))}
                            </div>
                        ))}
                    </div>

                    {/* Icon buttons */}
                    <div className="mt-6 grid grid-cols-2 sm:grid-cols-3 md:grid-cols-4 lg:grid-cols-6 gap-3">
                        <Button isIcon aria-label="Save">
                            <Save className="h-4 w-4" />
                        </Button>
                        <Button isIcon color="primary" variant="soft" aria-label="Refresh">
                            <RefreshCw className="h-4 w-4" />
                        </Button>
                        <Button isIcon color="success" aria-label="Confirm">
                            <Check className="h-4 w-4" />
                        </Button>
                        <Button isIcon color="danger" aria-label="Cancel">
                            <X className="h-4 w-4" />
                        </Button>
                        <Button isIcon color="warning" variant="outline" aria-label="Star">
                            <Star className="h-4 w-4" />
                        </Button>
                        <Button isIcon color="neutral" variant="ghost" aria-label="External">
                            <ExternalLink className="h-4 w-4" />
                        </Button>
                    </div>

                    {/* Polymorphic examples */}
                    <div className="mt-6 grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-3">
                        <Button component="a" href="#docs" color="primary" className="justify-between">
                            As <code className="mx-1">&lt;a&gt;</code> link <ExternalLink className="h-4 w-4" />
                        </Button>
                        <Button component="div" role="button" tabIndex={0} color="success" variant="soft">
                            As <code className="mx-1">&lt;div role="button"&gt;</code>
                        </Button>
                        <Button unstyled className="rounded-2xl border px-4 py-2 hover:bg-white/10">
                            Unstyled + custom className
                        </Button>
                    </div>

                    {/* Loading/disabled showcase */}
                    <div className="mt-6 grid grid-cols-2 sm:grid-cols-3 md:grid-cols-6 gap-3">
                        <Button loading color="primary"><Loader2 className="mr-2 h-4 w-4" />Loading…</Button>
                        <Button disabled color="primary">Disabled</Button>
                        <Button loading isIcon aria-label="Uploading"><Upload className="h-4 w-4" /></Button>
                        <Button isGlow color="primary">Glow</Button>
                        <Button color="warning" variant="outline">Outline</Button>
                        <Button color="neutral" variant="ghost">Ghost</Button>
                    </div>
                </section>

                {/* SPINNER SHOWCASE */}
                <section className="rounded-2xl border border-white/10 bg-white/5 p-4 sm:p-6 shadow-xl backdrop-blur">
                    <h2 className="mb-4 text-lg font-semibold">Spinners</h2>

                    <div className="grid grid-cols-1 sm:grid-cols-2 gap-6">
                        {/* Inline spinner sizes */}
                        <div className="rounded-xl border border-white/10 p-4">
                            <h3 className="mb-3 font-medium">Spinner (default)</h3>
                            <div className="flex items-center gap-4">
                                <Spinner />
                                <Spinner className="h-6 w-6" />
                                <Spinner className="h-8 w-8" />
                                <Spinner className="h-10 w-10" />
                            </div>
                            <p className="mt-3 text-xs text-slate-300">Uses <code>border-2</code> and <code>border-r-transparent</code>. Inherits current color.</p>
                        </div>

                        {/* Ghost variants */}
                        <div className="rounded-xl border border-white/10 p-4">
                            <h3 className="mb-3 font-medium">GhostSpinner variants</h3>
                            <div className="flex items-center gap-6">
                                <div className="grid place-items-center gap-2">
                                    <GhostSpinner />
                                    <span className="text-xs opacity-80">default</span>
                                </div>
                                <div className="grid place-items-center gap-2">
                                    <GhostSpinner variant="soft" />
                                    <span className="text-xs opacity-80">soft</span>
                                </div>
                                <div className="grid place-items-center gap-2">
                                    <GhostSpinner variant="innerDot" className="h-8 w-8" />
                                    <span className="text-xs opacity-80">innerDot</span>
                                </div>
                            </div>
                            <p className="mt-3 text-xs text-slate-300">Great for subtle loading states on ghost/soft buttons.</p>
                        </div>
                    </div>

                    {/* Spinner inside Button */}
                    <div className="mt-6 rounded-xl border border-white/10 p-4">
                        <h3 className="mb-3 font-medium">Spinner + Button combo</h3>
                        <div className="flex flex-wrap items-center gap-3">
                            <Button color="primary" loading>Saving…</Button>
                            <Button color="success"><Spinner className="mr-2" />Processing</Button>
                            <Button color="warning" variant="soft"><GhostSpinner className="mr-2" variant="soft" />Warming up</Button>
                            <Button color="danger" variant="outline"><GhostSpinner className="mr-2" variant="innerDot" />Danger op</Button>
                        </div>
                    </div>
                </section>

                <footer className="mt-10 text-xs text-slate-400">
                    <p>
                        Tip: If some combinations look unstyled, tweak your <code>getButtonClasses()</code> in <code>styles.ts</code> to add mappings for
                        new <code>color</code> / <code>variant</code> pairs. Data attributes <code>data-variant</code> / <code>data-color</code> are set by the Button for CSS targeting.
                    </p>
                </footer>
            </div>
        </div>
    );
}
