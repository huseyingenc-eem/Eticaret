import React, { memo, useCallback } from "react";
import { FileX } from "lucide-react";
import { type ColumnDef } from "./types";

function getCellValue<T>(row: T, col: ColumnDef<T>): any {
    if (typeof col.accessor === "function") return col.accessor(row);
    if (typeof col.accessor === "string") return (row as any)[col.accessor];
    return (row as any)[col.id];
}

/* ---------------- types ---------------- */
interface RowProps<T> {
    row: T;
    rowIndex: number;
    columns: ColumnDef<T>[];
    selectableRows?: boolean;
    isSelected: boolean;
    toggle: (key: React.Key) => void;

    /** tam-satır tıklama (clickColumnId verildiyse devre dışı kalır) */
    onRowClick?: (row: T) => void;

    /** sadece bu sütunda tıklama */
    clickColumnId?: string;
    onCellClick?: (row: T) => void;

    rowKey: React.Key;
    variant: "default" | "modern" | "minimal";
    density: "compact" | "normal" | "comfortable";
}

interface Props<T> {
    rows: T[];
    columns: ColumnDef<T>[];
    selectableRows?: boolean;
    selected: Set<React.Key>;
    setSelected: (s: Set<React.Key>) => void;

    /** tam-satır tıklama (clickColumnId verildiyse devre dışı kalır) */
    onRowClick?: (row: T) => void;

    /** sadece belirli sütunda tıklama */
    clickColumnId?: string;
    onCellClick?: (row: T) => void;

    rowId?: (row: T, index: number) => React.Key;
    emptyStateTitle?: string;
    emptyStateDescription?: string;
    variant?: "default" | "modern" | "minimal";
    density?: "compact" | "normal" | "comfortable";
}

/* -------------- Row (generic-safe memo) -------------- */
const Row = memo(<T,>(props: RowProps<T>) => {
    const {
        row,
        columns,
        selectableRows,
        isSelected,
        toggle,
        onRowClick,
        clickColumnId,
        onCellClick,
        rowKey,
        variant,
        density,
    } = props;

    const variantStyles = {
        default: {
            row: "border-b border-slate-100 hover:bg-slate-50 transition-colors duration-200",
            selectedRow: "bg-blue-50 hover:bg-blue-100 border-blue-200",
            cell: "text-slate-700",
        },
        modern: {
            row: "border-b border-slate-100 hover:bg-gradient-to-r hover:from-slate-50 hover:to-white transition-all duration-300",
            selectedRow: "bg-gradient-to-r from-blue-50 to-indigo-50 hover:from-blue-100 hover:to-indigo-100 border-blue-200",
            cell: "text-slate-700",
        },
        minimal: {
            row: "border-b border-slate-50 hover:bg-slate-25 transition-colors duration-150",
            selectedRow: "bg-slate-100",
            cell: "text-slate-600",
        },
    } as const;

    const densityStyles = {
        compact: "px-3 py-2",
        normal: "px-4 py-3",
        comfortable: "px-6 py-4",
    } as const;

    const currentVariant = variantStyles[variant];
    const cellPadding = densityStyles[density];

    // clickColumnId verildiyse satırın tamamı tıklanabilir olmasın
    const rowClickable = !!onRowClick && !clickColumnId;

    const rowClasses = [
        "group",
        rowClickable ? "cursor-pointer" : "cursor-default",
        isSelected ? currentVariant.selectedRow : currentVariant.row,
        rowClickable ? "focus:outline-none focus:ring-2 focus:ring-blue-500/20" : "",
    ]
        .filter(Boolean)
        .join(" ");

    return (
        <tr
            className={rowClasses}
            aria-selected={isSelected}
            tabIndex={rowClickable ? 0 : -1}
            onClick={() => rowClickable && onRowClick?.(row)}
            onKeyDown={(e) => {
                if (!rowClickable) return;
                if (e.key === "Enter" || e.key === " ") {
                    e.preventDefault();
                    onRowClick?.(row);
                }
            }}
        >
            {selectableRows && (
                <td className={`sticky left-0 z-10 bg-inherit border-r border-slate-100 ${cellPadding} w-12`}>
                    <div className="flex items-center justify-center">
                        <input
                            type="checkbox"
                            checked={isSelected}
                            onClick={(e) => e.stopPropagation()}
                            onChange={() => toggle(rowKey)}
                            className="
                h-4 w-4 rounded border-2 border-slate-300 text-blue-600
                focus:ring-2 focus:ring-blue-500/20 transition-colors duration-200
              "
                            aria-label="Satırı seç"
                        />
                    </div>
                </td>
            )}

            {columns.map((col) => {
                const rawValue = getCellValue(row, col);

                const stickyCls = col.sticky
                    ? `sticky ${col.sticky === "left" ? "left-0" : "right-0"} z-10 bg-inherit border-r border-slate-100`
                    : "";

                const widthCls =
                    col.width || col.minWidth || col.maxWidth
                        ? `${col.width || ""} ${col.minWidth ? `min-w-[${col.minWidth}]` : ""} ${col.maxWidth ? `max-w-[${col.maxWidth}]` : ""}`
                        : "";

                const isClickCell = !!clickColumnId && col.id === clickColumnId;

                const cellClasses = [
                    cellPadding,
                    currentVariant.cell,
                    col.className || "",
                    widthCls,
                    stickyCls,
                    "whitespace-nowrap overflow-hidden text-ellipsis",
                    isClickCell ? "text-blue-600 hover:text-blue-700" : "",
                ]
                    .filter(Boolean)
                    .join(" ");

                const content =
                    col.cell ? (
                        col.cell(rawValue, row)
                    ) : (
                        <span className={rawValue == null ? "text-slate-400 italic" : ""}>{String(rawValue ?? "—")}</span>
                    );

                const handleCellClick: React.MouseEventHandler = (e) => {
                    e.stopPropagation();
                    onCellClick?.(row);
                };
                const handleCellKeyDown: React.KeyboardEventHandler = (e) => {
                    if (e.key === "Enter" || e.key === " ") {
                        e.preventDefault();
                        onCellClick?.(row);
                    }
                };

                return (
                    <td key={col.id} className={cellClasses} title={String(rawValue ?? "")}>
                        {isClickCell ? (
                            <div
                                role="button"
                                tabIndex={0}
                                className="inline-flex items-center gap-1 cursor-pointer select-none focus:outline-none focus:ring-2 focus:ring-blue-500/20 rounded px-1 -mx-1"
                                onClick={handleCellClick}
                                onKeyDown={handleCellKeyDown}
                            >
                                {content}
                            </div>
                        ) : (
                            content
                        )}
                    </td>
                );
            })}
        </tr>
    );
}) as <T,>(p: RowProps<T>) => React.JSX.Element; // <-- generic-safe memo cast

/* -------------- Body (generic-safe memo) -------------- */
function DataTableBodyComponentInner<T>({
                                            rows,
                                            columns,
                                            selectableRows,
                                            selected,
                                            setSelected,
                                            onRowClick,
                                            clickColumnId,
                                            onCellClick,
                                            rowId,
                                            emptyStateTitle = "Veri bulunamadı",
                                            emptyStateDescription = "Lütfen filtreleri kontrol edin veya yeni veri ekleyin.",
                                            variant = "default",
                                            density = "normal",
                                        }: Props<T>) {
    const toggle = useCallback(
        (key: React.Key) => {
            setSelected: React.Dispatch<React.SetStateAction<Set<React.Key>>>;
            setSelected((prev: Set<React.Key>) => {
                const next = new Set(prev);
                next.has(key) ? next.delete(key) : next.add(key);
                return next;
            });
        },
        [setSelected]
    );

    return (
        <tbody className="bg-white divide-y divide-slate-100">
        {rows.map((row, rowIndex) => {
            const key: React.Key = rowId ? rowId(row, rowIndex) : rowIndex;
            const isSelected = selected.has(key);

            return (
                <Row
                    key={key}
                    row={row}
                    rowIndex={rowIndex}
                    columns={columns}
                    selectableRows={selectableRows}
                    isSelected={isSelected}
                    toggle={toggle}
                    onRowClick={onRowClick}
                    clickColumnId={clickColumnId}
                    onCellClick={onCellClick}
                    rowKey={key}
                    variant={variant}
                    density={density}
                />
            );
        })}

        {rows.length === 0 && (
            <tr>
                <td colSpan={(selectableRows ? 1 : 0) + columns.length} className="px-6 py-16">
                    <div className="flex flex-col items-center justify-center text-center space-y-3">
                        <div className="w-16 h-16 bg-slate-100 rounded-full flex items-center justify-center">
                            <FileX className="w-8 h-8 text-slate-400" />
                        </div>
                        <div>
                            <h3 className="text-base font-medium text-slate-600 mb-1">{emptyStateTitle}</h3>
                            <p className="text-sm text-slate-400">{emptyStateDescription}</p>
                        </div>
                    </div>
                </td>
            </tr>
        )}
        </tbody>
    );
}

const DataTableBodyComponent = memo(DataTableBodyComponentInner) as <T,>(p: Props<T>) => React.JSX.Element;
export default DataTableBodyComponent;
