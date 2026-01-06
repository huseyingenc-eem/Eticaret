import React from "react";

/* ------------------------- utils ------------------------- */
function cn(...v: Array<string | false | undefined | null>) {
    return v.filter(Boolean).join(" ");
}

/* -------------------------- types ------------------------ */
export type TimelineStatus = "complete" | "current" | "upcoming" | "error" | "info";

export type TimelineItem = {
    id: React.Key;
    title: React.ReactNode;
    description?: React.ReactNode;
    /** e.g. "12 minutes ago" or a localized date string */
    timestamp?: React.ReactNode;
    /** visual status -> maps to dot/line colors */
    status?: TimelineStatus;
};

export type TimelineProps = {
    items: TimelineItem[];
    /** vertical | horizontal | auto (sm=vertical, md+=horizontal) */
    orientation?: "vertical" | "horizontal" | "auto";
    /** left | right | alternate (vertical only) */
    align?: "left" | "right" | "alternate";
    /** left gutter (px) where the spine & dots live */
    gutterPx?: number;
    className?: string;
};

/* ------------------- visual tokens ----------------------- */
const statusColor: Record<TimelineStatus, { dot: string; line: string }> = {
    complete: { dot: "bg-emerald-600 border-emerald-600", line: "bg-emerald-100" },
    current:  { dot: "bg-blue-600 border-blue-600",       line: "bg-blue-100" },
    upcoming: { dot: "bg-slate-300 border-slate-300",     line: "bg-slate-200" },
    error:    { dot: "bg-red-600 border-red-600",         line: "bg-red-100" },
    info:     { dot: "bg-sky-600 border-sky-600",         line: "bg-sky-100" },
};

/* ----------------------- component ----------------------- */
export default function Timeline({
                                     items,
                                     orientation = "auto",
                                     align = "left",
                                     gutterPx = 24,
                                     className,
                                 }: TimelineProps) {
    const rootOrientation = orientation === "auto" ? "md:horiz" : orientation === "horizontal" ? "horiz" : "vert";

    return (
        <div className={cn("w-full", className)}>
            {/* VERTICAL */}
            <div className={cn(rootOrientation === "horiz" ? "hidden md:block" : "block")}>{
                /* vertical list */
                (
                    <ol className={cn("relative", align === "alternate" ? "mx-6" : "ml-8")} role="list" aria-orientation="vertical">
                        {/* spine */}
                        <span
                            aria-hidden
                            className={cn(
                                "absolute top-0 h-full w-px bg-slate-200",
                                align === "alternate" ? "left-1/2 -translate-x-1/2" : "left-0"
                            )}
                            style={align === "alternate" ? undefined : { transform: `translateX(-${gutterPx / 2}px)` }}
                        />

                        {items.map((item, idx) => {
                            const isLast = idx === items.length - 1;
                            const clr = statusColor[item.status ?? "upcoming"];

                            return (
                                <li
                                    key={item.id}
                                    className={cn(
                                        "relative mb-8 last:mb-0",
                                        align === "alternate"
                                            ? idx % 2 === 0
                                                ? "md:pr-[52%]"
                                                : "md:pl-[52%]"
                                            : align === "right"
                                                ? "text-right mr-8"
                                                : ""
                                    )}
                                >
                                    {/* dot */}
                                    <span
                                        aria-hidden
                                        className={cn(
                                            "absolute -left-8 mt-1.5 h-2.5 w-2.5 rounded-full border-2",
                                            clr.dot,
                                            align === "alternate" && "left-1/2 -translate-x-[7px] md:-translate-x-[7px]"
                                        )}
                                        style={align === "alternate" ? undefined : { transform: `translateX(-${gutterPx - 8}px)` }}
                                    />

                                    {/* connector */}
                                    {!isLast && (
                                        <span
                                            aria-hidden
                                            className={cn(
                                                "absolute -left-8 top-4 w-px",
                                                clr.line,
                                                align === "alternate" && "left-1/2 -translate-x-px"
                                            )}
                                            style={
                                                align === "alternate"
                                                    ? { height: "calc(100% - 1rem)" }
                                                    : { transform: `translateX(-${gutterPx - 8}px)`, height: "calc(100% - 1rem)" }
                                            }
                                        />
                                    )}

                                    {/* content */}
                                    <div className="space-y-1">
                                        <h3 className="font-semibold leading-none text-slate-800">{item.title}</h3>
                                        {item.timestamp && (
                                            <p className="text-xs text-slate-500">{item.timestamp}</p>
                                        )}
                                        {item.description && (
                                            <p className="text-sm leading-6 text-slate-600">{item.description}</p>
                                        )}
                                    </div>
                                </li>
                            );
                        })}
                    </ol>
                )}
            </div>

            {/* HORIZONTAL (md+) */}
            <div className={cn(rootOrientation === "horiz" ? "block" : "md:hidden")}>{
                /* horizontal scroller */
                (
                    <div role="list" aria-orientation="horizontal" className="relative">
                        {/* spine */}
                        <div aria-hidden className="absolute left-0 right-0 top-4 h-px bg-slate-200" />

                        <div className="grid auto-cols-[minmax(220px,1fr)] grid-flow-col gap-6 overflow-x-auto pt-6 pb-1">
                            {items.map((item) => {
                                const clr = statusColor[item.status ?? "upcoming"];
                                return (
                                    <div key={item.id} className="relative">
                                        {/* dot */}
                                        <span aria-hidden className={cn("absolute -top-3 left-0 h-2.5 w-2.5 rounded-full border-2", clr.dot)} />

                                        {/* content */}
                                        <div className="space-y-1">
                                            <h3 className="font-semibold leading-none text-slate-800">{item.title}</h3>
                                            {item.timestamp && <p className="text-xs text-slate-500">{item.timestamp}</p>}
                                            {item.description && <p className="text-sm leading-6 text-slate-600">{item.description}</p>}
                                        </div>
                                    </div>
                                );
                            })}
                        </div>
                    </div>
                )}
            </div>
        </div>
    );
}