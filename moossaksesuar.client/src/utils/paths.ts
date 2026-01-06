import { slugify } from "./slug";

// /admin/categories/:id-:slug
export function buildCategoryPath(id: number, name?: string, opt?: { edit?: boolean }) {
    const slug = name ? slugify(name) : "category";
    const path = `/admin/categories/${id}-${slug}`;
    return opt?.edit ? `${path}?edit=1` : path;
}

// "1-elektronik" → 1, "8" → 8
export function parseIdFromIdSlug(idSlug?: string): number {
    if (!idSlug) return NaN;
    const m = idSlug.match(/^(\d+)/);
    return m ? Number(m[1]) : NaN;
}
