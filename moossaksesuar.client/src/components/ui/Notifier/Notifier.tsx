import { useEffect } from "react";
import { Toaster, ToastBar, toast, useToasterStore } from "react-hot-toast";
import type { AppError } from "@/services/api/http";
import {
    CheckCircle2,
    XCircle,
    Info,
    Loader2,
    X as CloseIcon,
} from "lucide-react";

const TOAST_LIMIT = 3;

type Variant = "success" | "error" | "info" | "loading";

function variantOf(type: string): Variant {
    if (type === "success") return "success";
    if (type === "error") return "error";
    if (type === "loading") return "loading";
    return "info";
}

function classes(v: Variant) {
    switch (v) {
        case "success":
            return {
                card: "bg-emerald-50/90 border-emerald-200",
                icon: "text-emerald-600",
                accent: "bg-emerald-500/80",
            };
        case "error":
            return {
                card: "bg-rose-50/90 border-rose-200",
                icon: "text-rose-600",
                accent: "bg-rose-500/80",
            };
        case "info": // turuncu/amber
            return {
                card: "bg-amber-50/90 border-amber-200",
                icon: "text-amber-600",
                accent: "bg-amber-500/80",
            };
        case "loading":
            return {
                card: "bg-sky-50/90 border-sky-200",
                icon: "text-sky-600",
                accent: "bg-sky-500/80",
            };
    }
}

export default function Notifier() {
    // Maks 3 toast
    const { toasts } = useToasterStore();
    useEffect(() => {
        toasts
            .filter((t) => t.visible)
            .filter((_, i) => i >= TOAST_LIMIT)
            .forEach((t) => toast.dismiss(t.id));
    }, [toasts]);

    // HTTP error event'leri
    useEffect(() => {
        const onHttpError = (e: Event) => {
            const { detail } = e as CustomEvent<AppError>;
            if (!detail) return;
            if (detail.kind === "network") {
                toast.error(detail.message || "Ağ hatası");
            } else if (detail.kind === "problem") {
                toast.error(detail.message || detail.title || "İşlem başarısız");
            }
        };
        window.addEventListener("app:error", onHttpError as EventListener);
        return () => window.removeEventListener("app:error", onHttpError as EventListener);
    }, []);

    return (
        <Toaster
            position="top-right"
            gutter={10}
            containerStyle={{ top: 14, right: 14 }}
            toastOptions={{
                duration: 3500,
                // root'u şeffaf bırakıyoruz; kart stilini içeride veriyoruz
                className:
                    "group pointer-events-auto !p-0 !bg-transparent !shadow-none !border-0",
            }}
        >
            {(t) => {
                const v = variantOf(t.type);
                const c = classes(v);

                return (
                    <ToastBar toast={t} style={{ padding: 0 }}>
                        {() => (
                            <div
                                className={`relative flex items-center gap-3 min-w-[300px] max-w-[380px] rounded-xl border px-3.5 py-3 shadow-lg backdrop-blur-sm ${c.card}`}
                            >
                                {/* sol accent bar */}
                                <div className={`self-stretch w-1 rounded-l-xl ${c.accent}`} />

                                {/* ikon (dikey ortalı) */}
                                <div className="shrink-0 flex items-center justify-center ml-1">
                                    {v === "success" ? (
                                        <CheckCircle2 size={20} className={c.icon} />
                                    ) : v === "error" ? (
                                        <XCircle size={20} className={c.icon} />
                                    ) : v === "loading" ? (
                                        <Loader2 size={20} className={`${c.icon} animate-spin`} />
                                    ) : (
                                        <Info size={20} className={c.icon} />
                                    )}
                                </div>

                                {/* metin */}
                                <div className="pr-8">
                                    <div className="text-sm font-medium text-gray-900">
                                        {t.message as React.ReactNode}
                                    </div>
                                </div>

                                {/* hover'da görünen kapat butonu */}
                                <button
                                    aria-label="Kapat"
                                    onClick={() => toast.dismiss(t.id)}
                                    className="absolute right-2 top-2 opacity-0 group-hover:opacity-100 transition-opacity p-1 rounded hover:bg-black/5"
                                >
                                    <CloseIcon size={16} className="text-gray-500" />
                                </button>
                            </div>
                        )}
                    </ToastBar>
                );
            }}
        </Toaster>
    );
}
