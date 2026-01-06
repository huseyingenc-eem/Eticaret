import { memo, useMemo } from "react";
import { ChevronLeft, ChevronRight, ChevronsLeft, ChevronsRight } from "lucide-react";

interface Props {
    page: number;
    pageSize: number;
    total: number;
    setPage: (p: number) => void;
    setPageSize: (n: number) => void;
    showRange?: boolean;
    className?: string;
}

function DataTablePaginationComponent({
                                          page,
                                          pageSize,
                                          total,
                                          setPage,
                                          setPageSize,
                                          showRange = true,
                                          className = "",
                                      }: Props) {
    const totalPages = Math.max(1, Math.ceil(total / pageSize));
    const canPrev = page > 1;
    const canNext = page < totalPages;

    const start = total === 0 ? 0 : (page - 1) * pageSize + 1;
    const end = Math.min(page * pageSize, total);

    // Dinamik sayfa büyüklüğü seçenekleri
    const pageSizeOptions = useMemo(() => {
        const baseOptions = [5, 10, 20, 30, 50, 100];
        return baseOptions.filter(size => size <= total * 2); // Toplam kaydın 2 katına kadar
    }, [total]);

    const buttonClass = (disabled: boolean) => `
        inline-flex items-center justify-center px-3 py-1.5 text-sm font-medium
        border border-slate-300 rounded-md transition-colors duration-200
        ${disabled
        ? "bg-slate-100 text-slate-400 cursor-not-allowed"
        : "bg-white text-slate-700 hover:bg-slate-50 hover:border-slate-400"
    }
    `;

    return (
        <div className={`
            flex items-center justify-between gap-4 px-6 py-4 border-t border-slate-200 bg-white
            ${className}
        `}>
            <div className="flex items-center gap-4 text-sm text-slate-600">
                <div className="flex items-center gap-2">
                    <span>Sayfa büyüklüğü:</span>
                    <select
                        value={pageSize}
                        onChange={(e) => {
                            const newSize = Number(e.target.value);
                            setPageSize(newSize);
                            setPage(1);
                        }}
                        className="
                            border border-slate-300 rounded-md px-2 py-1 text-sm
                            focus:border-blue-500 focus:ring-2 focus:ring-blue-500/20
                            transition-all duration-200 outline-none
                        "
                    >
                        {pageSizeOptions.map((size) => (
                            <option key={size} value={size}>
                                {size}
                            </option>
                        ))}
                    </select>
                </div>

                {showRange && total > 0 && (
                    <div className="hidden sm:block">
                        <span>
                            <strong>{start}</strong>-<strong>{end}</strong> / <strong>{total}</strong> kayıt
                        </span>
                    </div>
                )}
            </div>

            <div className="flex items-center gap-4">
                <div className="text-sm text-slate-600">
                    Sayfa <strong>{page}</strong> / <strong>{totalPages}</strong>
                </div>

                <div className="flex items-center gap-1">
                    <button
                        className={buttonClass(!canPrev)}
                        disabled={!canPrev}
                        onClick={() => canPrev && setPage(1)}
                        title="İlk sayfa"
                    >
                        <ChevronsLeft className="h-4 w-4" />
                    </button>

                    <button
                        className={buttonClass(!canPrev)}
                        disabled={!canPrev}
                        onClick={() => canPrev && setPage(page - 1)}
                        title="Önceki sayfa"
                    >
                        <ChevronLeft className="h-4 w-4" />
                    </button>

                    <button
                        className={buttonClass(!canNext)}
                        disabled={!canNext}
                        onClick={() => canNext && setPage(page + 1)}
                        title="Sonraki sayfa"
                    >
                        <ChevronRight className="h-4 w-4" />
                    </button>

                    <button
                        className={buttonClass(!canNext)}
                        disabled={!canNext}
                        onClick={() => canNext && setPage(totalPages)}
                        title="Son sayfa"
                    >
                        <ChevronsRight className="h-4 w-4" />
                    </button>
                </div>
            </div>
        </div>
    );
}

export default memo(DataTablePaginationComponent) as typeof DataTablePaginationComponent;