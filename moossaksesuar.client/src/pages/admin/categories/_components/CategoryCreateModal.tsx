import { useEffect, useState } from "react";
import { categoryApi } from "@/services/api/endpoints/category/category.api";
import type { CreateCategoryRequest } from "@/services/api/endpoints/category/category.types";

export default function CategoryCreateModal({
                                                open,
                                                onClose,
                                                defaultParentId = null,
                                                onSaved,
                                            }: {
    open: boolean;
    onClose: () => void;
    defaultParentId?: number | null;
    onSaved: () => void;
}) {
    const [bulkText, setBulkText] = useState<string>("");
    const [commonIsActive, setCommonIsActive] = useState<boolean>(true);
    const [saving, setSaving] = useState(false);

    useEffect(() => {
        if (open) {
            setBulkText("");
            setCommonIsActive(true);
        }
    }, [open, defaultParentId]);

    if (!open) return null;

    const parseLines = (text: string): string[] => {
        const raw = text
            .split(/\r?\n|,|;/g)
            .map((s) => s.trim())
            .filter(Boolean);
        return Array.from(new Set(raw));
    };

    const createPayloads = (names: string[]): CreateCategoryRequest[] =>
        names.map((name) => ({
            name,
            parentId: defaultParentId ?? null,
            isActive: commonIsActive,
        }));

    const saveInternal = async (closeAfter: boolean) => {
        const names = parseLines(bulkText);
        if (names.length === 0) return;

        const payloads = createPayloads(names);

        setSaving(true);
        try {
            const results = await Promise.allSettled(payloads.map((p) => categoryApi.create(p)));

            const success = results.filter((r) => r.status === "fulfilled").length;
            const fail = results.length - success;

            if (success > 0) onSaved();

            if (closeAfter) {
                onClose();
            } else {
                if (fail > 0) {
                    const failedNames: string[] = [];
                    results.forEach((r, i) => {
                        if (r.status === "rejected") failedNames.push(names[i]);
                    });
                    setBulkText(failedNames.join("\n"));
                } else {
                    setBulkText("");
                }
            }
        } finally {
            setSaving(false);
        }
    };

    const saveAndClose = () => void saveInternal(true);
    const saveAndContinue = () => void saveInternal(false);

    const estimatedCount = parseLines(bulkText).length;

    return (
        <div className="fixed inset-0 z-50 flex items-center justify-center p-4">
            {/* Backdrop */}
            <div
                className="absolute inset-0 bg-black/50 backdrop-blur-sm transition-opacity"
                onClick={onClose}
            />

            {/* Modal */}
            <div className="relative w-full max-w-2xl transform rounded-3xl bg-white shadow-2xl transition-all">
                {/* Header */}
                <div className="relative px-8 py-6 border-b border-gray-100">
                    <div className="flex items-center justify-between">
                        <div>
                            <h2 className="text-xl font-semibold text-gray-900">
                                Toplu Kategori Ekle
                            </h2>
                            {defaultParentId !== null && (
                                <div className="mt-1 flex items-center gap-2 text-sm text-gray-600">
                                    <div className="h-1.5 w-1.5 rounded-full bg-blue-500"></div>
                                    Üst kategori ID:
                                    <span className="inline-flex items-center px-2 py-1 text-xs font-medium bg-blue-50 text-blue-700 rounded-full">
                                        {defaultParentId}
                                    </span>
                                </div>
                            )}
                        </div>

                        {/* Close button */}
                        <button
                            onClick={onClose}
                            className="flex items-center justify-center w-8 h-8 text-gray-400 hover:text-gray-600 hover:bg-gray-100 rounded-full transition-colors"
                            disabled={saving}
                        >
                            <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M6 18L18 6M6 6l12 12" />
                            </svg>
                        </button>
                    </div>
                </div>

                {/* Body */}
                <div className="px-8 py-6 space-y-6">
                    {/* Textarea section */}
                    <div className="space-y-3">
                        <div className="flex items-center justify-between">
                            <label className="text-sm font-medium text-gray-700">
                                Kategori adları
                            </label>
                            {estimatedCount > 0 && (
                                <span className="text-xs text-gray-500 bg-gray-100 px-2 py-1 rounded-full">
                                    {estimatedCount} kategori
                                </span>
                            )}
                        </div>

                        <div className="relative">
                            <textarea
                                value={bulkText}
                                onChange={(e) => setBulkText(e.target.value)}
                                placeholder="Her satıra bir kategori adı yazınız...&#10;Örnek:&#10;Elektronik&#10;Bilgisayar&#10;Telefon"
                                className="w-full h-40 px-4 py-3 text-sm border border-gray-200 rounded-xl resize-y focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent transition-all placeholder-gray-400"
                            />

                            {/* Input helper */}
                            <div className="absolute bottom-3 right-3">
                                <div className="flex items-center gap-1 text-xs text-gray-400">
                                    <svg className="w-3 h-3" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                        <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M13 16h-1v-4h-1m1-4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
                                    </svg>
                                    <span>Satır | Virgül | Noktalı virgül ile ayırın</span>
                                </div>
                            </div>
                        </div>

                        <div className="flex items-start gap-3 p-4 bg-blue-50 rounded-xl border border-blue-100">
                            <div className="flex-shrink-0 w-5 h-5 bg-blue-500 rounded-full flex items-center justify-center mt-0.5">
                                <svg className="w-3 h-3 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M13 16h-1v-4h-1m1-4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
                                </svg>
                            </div>
                            <div className="text-sm text-blue-800">
                                <p className="font-medium mb-1">Toplu ekleme nasıl çalışır?</p>
                                <p>Her satıra bir kategori adı yazın. Virgül (,) veya noktalı virgül (;) ile de ayırabilirsiniz. Üst kategori otomatik olarak {defaultParentId ? `ID ${defaultParentId}` : 'ana kategori'} olarak ayarlanır.</p>
                            </div>
                        </div>
                    </div>

                    {/* Active checkbox */}
                    <div className="flex items-center justify-between p-4 bg-gray-50 rounded-xl">
                        <div className="flex items-center gap-3">
                            <div className="flex-shrink-0">
                                <div className="w-10 h-10 bg-green-100 rounded-full flex items-center justify-center">
                                    <svg className="w-5 h-5 text-green-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                        <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z" />
                                    </svg>
                                </div>
                            </div>
                            <div>
                                <p className="text-sm font-medium text-gray-900">Kategori durumu</p>
                                <p className="text-xs text-gray-600">Kategoriler aktif olarak eklensin mi?</p>
                            </div>
                        </div>

                        <label className="relative inline-flex items-center cursor-pointer">
                            <input
                                type="checkbox"
                                checked={commonIsActive}
                                onChange={(e) => setCommonIsActive(e.target.checked)}
                                className="sr-only peer"
                            />
                            <div className="relative w-11 h-6 bg-gray-200 peer-focus:outline-none peer-focus:ring-4 peer-focus:ring-blue-300 rounded-full peer peer-checked:after:translate-x-full peer-checked:after:border-white after:content-[''] after:absolute after:top-[2px] after:left-[2px] after:bg-white after:border-gray-300 after:border after:rounded-full after:h-5 after:w-5 after:transition-all peer-checked:bg-blue-600"></div>
                        </label>
                    </div>
                </div>

                {/* Footer */}
                <div className="px-8 py-6 bg-gray-50 border-t border-gray-100 rounded-b-3xl">
                    <div className="flex items-center justify-between gap-4">
                        <div className="text-sm text-gray-600">
                            {estimatedCount > 0 && (
                                <span className="font-medium text-gray-900">{estimatedCount}</span>
                            )} kategori eklenecek
                        </div>

                        <div className="flex items-center gap-3">
                            <button
                                className="px-4 py-2 text-sm font-medium text-gray-700 bg-white border border-gray-300 rounded-lg hover:bg-gray-50 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500 transition-colors disabled:opacity-50 disabled:cursor-not-allowed"
                                onClick={onClose}
                                disabled={saving}
                            >
                                İptal
                            </button>

                            <button
                                className="px-4 py-2 text-sm font-medium text-blue-700 bg-blue-50 border border-blue-200 rounded-lg hover:bg-blue-100 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500 transition-colors disabled:opacity-50 disabled:cursor-not-allowed"
                                onClick={saveAndContinue}
                                disabled={saving || !bulkText.trim()}
                            >
                                Kaydet ve devam et
                            </button>

                            <button
                                className="px-6 py-2 text-sm font-medium text-white bg-blue-600 rounded-lg hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500 transition-colors disabled:opacity-50 disabled:cursor-not-allowed flex items-center gap-2"
                                onClick={saveAndClose}
                                disabled={saving || !bulkText.trim()}
                            >
                                {saving ? (
                                    <>
                                        <svg className="animate-spin w-4 h-4" fill="none" viewBox="0 0 24 24">
                                            <circle className="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" strokeWidth="4"></circle>
                                            <path className="opacity-75" fill="currentColor" d="m4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
                                        </svg>
                                        Kaydediliyor...
                                    </>
                                ) : (
                                    <>
                                        <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M5 13l4 4L19 7" />
                                        </svg>
                                        Kaydet
                                    </>
                                )}
                            </button>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    );
}