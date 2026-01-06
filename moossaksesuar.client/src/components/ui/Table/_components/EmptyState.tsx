// src/components/ui/Table/_components/EmptyState.tsx
import { FileX } from "lucide-react";

interface Props {
    title?: string;
    description?: string;
}

export default function EmptyState({
                                       title = "Veri bulunamadı",
                                       description = "Henüz hiç veri yok."
                                   }: Props) {
    return (
        <tbody>
        <tr>
            <td colSpan={100} className="py-16">
                <div className="flex flex-col items-center space-y-3 text-center max-w-md mx-auto">
                    <div className="w-16 h-16 bg-slate-100 rounded-full flex items-center justify-center">
                        <FileX className="h-8 w-8 text-slate-400" />
                    </div>
                    <div>
                        <h3 className="text-base font-medium text-slate-800 mb-1">{title}</h3>
                        <p className="text-sm text-slate-500">{description}</p>
                    </div>
                </div>
            </td>
        </tr>
        </tbody>
    );
}