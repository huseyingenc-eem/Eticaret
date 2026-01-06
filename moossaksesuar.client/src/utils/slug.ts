// Türkçe ve aksan dostu slug
export function slugify(input: string) {
    const map: Record<string, string> = { ı:"i", İ:"i", ğ:"g", Ğ:"g", ş:"s", Ş:"s", ö:"o", Ö:"o", ü:"u", Ü:"u", ç:"c", Ç:"c" };
    const replaced = input
        .split("")
        .map((ch) => map[ch] ?? ch)
        .join("")
        .normalize("NFD")
        .replace(/[\u0300-\u036f]/g, "");

    return replaced
        .toLowerCase()
        .replace(/[^a-z0-9]+/g, "-")
        .replace(/(^-+|-+$)/g, "");
}
