// src/components/ui/Table/_components/ErrorState.tsx
import { AlertCircle } from "lucide-react";

interface Props {
    error: string;
}

export default function ErrorState({ error }: Props) {
    return (
        <div className="flex items-center justify-center py-16">
            <div className="flex flex-col items-center space-y-3 text-center max-w-md">
                <div className="w-16 h-16 bg-red-100 rounded-full flex items-center justify-center">
                    <AlertCircle className="h-8 w-8 text-red-600" />
                </div>
                <div>
                    <h3 className="text-base font-medium text-slate-800 mb-1">Bir hata oluştu</h3>
                    <p className="text-sm text-slate-500">{error}</p>
                </div>
            </div>
        </div>
    );
}