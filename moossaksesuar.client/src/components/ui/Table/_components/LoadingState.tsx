// src/components/ui/Table/_components/LoadingState.tsx
import { Loader2 } from "lucide-react";

export default function LoadingState() {
    return (
        <div className="flex items-center justify-center py-16">
            <div className="flex flex-col items-center space-y-3">
                <Loader2 className="h-8 w-8 animate-spin text-blue-600" />
                <p className="text-sm text-slate-500">Veriler yükleniyor...</p>
            </div>
        </div>
    );
}