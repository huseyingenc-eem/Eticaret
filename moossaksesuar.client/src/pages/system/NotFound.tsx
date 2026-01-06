import { Link } from "react-router-dom";

export default function NotFound() {
    return (
        <div className="min-h-[60vh] flex items-center justify-center px-6">
            <div className="max-w-md text-center">
                <div className="mb-4 text-6xl">🧭</div>
                <h1 className="text-2xl font-semibold">404 – Sayfa bulunamadı</h1>
                <p className="mt-2 text-black/70">
                    Aradığın sayfa kaldırılmış, adı değiştirilmiş ya da geçici olarak ulaşılamıyor olabilir.
                </p>

                <div className="mt-6 flex items-center justify-center gap-3">
                    <Link to="/" className="rounded-lg bg-black px-4 py-2 text-white text-sm">
                        Ana sayfaya dön
                    </Link>
                    <Link to="/store" className="rounded-lg border px-4 py-2 text-sm">
                        Mağazayı gez
                    </Link>
                </div>
            </div>
        </div>
    );
}
