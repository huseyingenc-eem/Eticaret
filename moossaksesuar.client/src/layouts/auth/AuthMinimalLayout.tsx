import { Outlet, useNavigate } from "react-router-dom";

export default function AuthMinimalLayout() {
    const nav = useNavigate();

    const goBack = () => {
        if (window.history.length > 1) nav(-1);
        else nav("/", { replace: true });
    };

    return (
        <div className="min-h-screen bg-gradient-to-br from-blue-50 to-indigo-100 flex items-center justify-center p-4">
            <div className="relative w-full max-w-md">
                {/* Üst mini bar */}
                <div className="absolute -top-10 left-0">
                    <button
                        onClick={goBack}
                        className="inline-flex items-center gap-2 text-sm text-blue-700 hover:text-blue-900"
                    >
                        <span className="inline-block h-5 w-5 rounded-full border border-blue-600" />
                        Geri dön
                    </button>
                </div>

                {/* İçerik kutusu */}
                <div className="bg-white rounded-2xl shadow-sm overflow-hidden">
                    <Outlet />
                </div>

                {/* Alt mini footer */}
                <div className="mt-6 text-center text-sm text-gray-600">
                    © {new Date().getFullYear()} Şirket Adı Maass Aksesuar. Tüm hakları saklıdır.
                </div>
            </div>
        </div>
    );
}
