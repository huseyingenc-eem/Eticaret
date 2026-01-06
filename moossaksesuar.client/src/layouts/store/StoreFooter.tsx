import Container from "@/layouts/store/_components/Container";
import { Link } from "react-router-dom";
import { useNavigation } from "@/hooks/useNavigation";
import type { FooterColumn, FooterLink } from "@/configs";

export default function StoreFooter() {
    const { footer } = useNavigation();

    return (
        <footer className="border-t bg-white">
            <Container className="py-10">
                <div className="grid grid-cols-2 sm:grid-cols-3 md:grid-cols-4 gap-8 text-sm">
                    {/* Dinamik Sütunlar */}
                    {footer.map((column: FooterColumn) => (
                        <div key={column.title}>
                            <div className="font-semibold mb-3 text-gray-900">{column.title}</div>
                            <ul className="space-y-2 text-black/70">
                                {column.links.map((link: FooterLink) => (
                                    <li key={link.id}>
                                        <Link to={link.to!} className="hover:text-black">
                                            {link.label}
                                        </Link>
                                    </li>
                                ))}
                            </ul>
                        </div>
                    ))}

                    {/* Newsletter Formu (Statik kalabilir veya konfigürasyondan yönetilebilir) */}
                    <div className="col-span-2 md:col-span-1">
                        <div className="font-semibold mb-3">Newsletter</div>
                        <p className="text-xs text-black/60 mb-2">Get the latest news and offers.</p>
                        <form className="flex gap-2">
                            <input
                                type="email"
                                placeholder="Email address"
                                className="w-full rounded-lg border px-3 py-2 text-sm outline-none focus:ring-2 focus:ring-black/10"
                            />
                            <button type="submit" className="rounded-lg bg-black px-4 py-2 text-white text-sm shrink-0">
                                Join
                            </button>
                        </form>
                    </div>
                </div>
                <div className="mt-10 pt-8 border-t text-center text-xs text-black/60">
                    © {new Date().getFullYear()} Mooss. All rights reserved.
                </div>
            </Container>
        </footer>
    );
}
