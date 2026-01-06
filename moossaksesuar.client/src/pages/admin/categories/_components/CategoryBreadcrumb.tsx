import { Link } from "react-router-dom";
import { buildCategoryPath } from "@/utils/paths";
import type { AncestorNode } from "@/services/api/endpoints/category/category.types";

export default function CategoryBreadcrumb({
                                               path,
                                               current,
                                           }: {
    path: AncestorNode[];
    current: { id: number; name: string } | null;
}) {
    if ((!path || path.length === 0) && !current) return null;

    return (
        <nav className="text-sm text-black/60">
            <ol className="flex flex-wrap items-center gap-2">
                <li>
                    <Link className="hover:underline" to="/admin/categories">Root</Link>
                </li>

                {path?.map((p) => (
                    <li key={p.id} className="flex items-center gap-2">
                        <span>/</span>
                        <Link className="hover:underline" to={buildCategoryPath(p.id, p.name)}>
                            {p.name}
                        </Link>
                    </li>
                ))}

                {current && (
                    <li className="flex items-center gap-2">
                        <span>/</span>
                        <span className="font-medium text-black">{current.name}</span>
                    </li>
                )}
            </ol>
        </nav>
    );
}
