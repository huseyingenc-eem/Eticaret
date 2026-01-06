// src/components/ui/feedback/Empty.tsx
import { FileX } from "lucide-react";
import React from "react";

type Variant = "block" | "table";
interface Props {
    title?: string;
    description?: string;
    icon?: React.ReactNode;
    /** table içinde kullanacaksan: "table" seç ve colSpan ver */
    variant?: Variant;
    colSpan?: number;
    className?: string;
    action?: React.ReactNode;
}

export default function Empty({
                                  title = "Veri bulunamadı",
                                  description = "Henüz hiç veri yok.",
                                  icon = <FileX className="h-8 w-8 text-slate-400" />,
                                  variant = "block",
                                  colSpan = 100,
                                  className = "",
                                  action
                              }: Props) {
    const content = (
        <div className="flex flex-col items-center space-y-3 text-center max-w-md mx-auto">
            <div className="w-16 h-16 bg-slate-100 rounded-full flex items-center justify-center">
                {icon}
            </div>
            <div>
                <h3 className="text-base font-medium text-slate-800 mb-1">{title}</h3>
                <p className="text-sm text-slate-500">{description}</p>
            </div>
            {action ? <div className="mt-1">{action}</div> : null}
        </div>
    );

    if (variant === "table") {
        return (
            <tbody>
            <tr>
                <td colSpan={colSpan} className="py-16">{content}</td>
            </tr>
            </tbody>
        );
    }

    // block
    return (
        <div className={`w-full py-16 ${className}`}>
            {content}
        </div>
    );
}
