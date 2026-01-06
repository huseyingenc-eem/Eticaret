import { Link, useNavigate, useLocation } from "react-router-dom";

export default function Forbidden() {
    const nav = useNavigate();
    const location = useLocation();
    const from = (location.state as any)?.from?.pathname;

    return (
        <div className="min-h-[60vh] flex items-center justify-center px-6">
            <div className="max-w-md text-center">
                <div className="mb-4 text-6xl">🚫</div>
                <h1 className="text-2xl font-semibold">403 – Yetkin yok</h1>
                <p className="mt-2 text-black/70">
                    Bu sayfayı görüntülemek için gerekli role sahip değilsin.
                </p>

                <div className="mt-6 flex items-center justify-center gap-3">
                    <button
                        onClick={() => nav(-1)}
                        className="rounded-lg border px-4 py-2 text-sm"
                    >
                        Geri dön
                    </button>
                    {from ? (
                        <button
                            onClick={() => nav(from, { replace: true })}
                            className="rounded-lg bg-black px-4 py-2 text-white text-sm"
                        >
                            Geldiğim sayfaya git
                        </button>
                    ) : (
                        <Link
                            to="/"
                            className="rounded-lg bg-black px-4 py-2 text-white text-sm"
                        >
                            Ana sayfa
                        </Link>
                    )}
                </div>
                <p className="mt-3 text-xs text-black/50">
                    Eğer bu bir hata ise yöneticiyle iletişime geç.
                </p>
            </div>
        </div>
    );
}
