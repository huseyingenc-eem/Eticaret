// src/components/ui/feedback/ErrorState.tsx
import { AlertTriangle } from "lucide-react";
import React from "react";

type Variant = "block" | "table";
interface Props {
    title?: string;
    message?: string;
    variant?: Variant;
    colSpan?: number;
    onRetry?: () => void;
    action?: React.ReactNode;
    className?: string;
}

export default function ErrorState({
                                       title = "Bir şeyler ters gitti",
                                       message = "Lütfen tekrar deneyin. Sorun devam ederse yöneticinize başvurun.",
                                       variant = "block",
                                       colSpan = 100,
                                       onRetry,
                                       action,
                                       className = ""
                                   }: Props) {
    const content = (
        <div className="flex flex-col items-center text-center gap-3 max-w-md mx-auto">
            <div className="w-16 h-16 rounded-full bg-red-50 flex items-center justify-center">
                <AlertTriangle className="h-8 w-8 text-red-500" />
            </div>
            <div>
                <h3 className="text-base font-semibold text-slate-800 mb-1">{title}</h3>
                <p className="text-sm text-slate-600">{message}</p>
            </div>
            <div className="flex gap-2">
                {onRetry && (
                    <button
                        onClick={onRetry}
                        className="rounded-xl px-3 py-1.5 text-sm font-medium bg-slate-800 text-white"
                    >
                        Tekrar Dene
                    </button>
                )}
                {action}
            </div>
        </div>
    );

    if (variant === "table") {
        return (
            <tbody>
            <tr>
                <td colSpan={colSpan} className="py-12">{content}</td>
            </tr>
            </tbody>
        );
    }

    return <div className={`w-full py-12 ${className}`}>{content}</div>;
}
