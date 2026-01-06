export const toComparable = (v: any) => {
    if (v == null) return "";
    if (v instanceof Date) return v.getTime();
    if (typeof v === "number") return v;
    return String(v).toLowerCase();
};

export const inNumberRange = (value: any, range?: { min?: number; max?: number }) => {
    if (value == null || range == null) return true;
    const n = Number(value);
    if (Number.isNaN(n)) return false;
    if (range.min != null && n < range.min) return false;
    if (range.max != null && n > range.max) return false;
    return true;
};

export const inDateRange = (value: any, range?: { from?: string; to?: string }) => {
    if (!range) return true;
    const t = value instanceof Date ? value.getTime() : new Date(value).getTime();
    if (Number.isNaN(t)) return false;
    const from = range.from ? new Date(range.from).getTime() : undefined;
    const to = range.to ? new Date(range.to).getTime() : undefined;
    if (from != null && t < from) return false;
    if (to != null && t > to) return false;
    return true;
};

export const formatCurrency = (value: number, currency = 'TRY') => {
    return new Intl.NumberFormat('tr-TR', {
        style: 'currency',
        currency,
        minimumFractionDigits: 2,
        maximumFractionDigits: 2,
    }).format(value);
};

export const formatDate = (value: string | Date) => {
    return new Intl.DateTimeFormat('tr-TR', {
        day: '2-digit',
        month: '2-digit',
        year: 'numeric',
    }).format(new Date(value));
};

export const debounce = <T extends (...args: any[]) => any>(
    func: T,
    wait: number
): T => {
    let timeout: NodeJS.Timeout;
    return ((...args: any[]) => {
        clearTimeout(timeout);
        timeout = setTimeout(() => func(...args), wait);
    }) as T;
};