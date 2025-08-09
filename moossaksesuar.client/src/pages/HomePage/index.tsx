// src/pages/HomePage/index.tsx
import { useAuth } from '@/hooks/useAuth';
import { Button } from '@/components/ui';

const HomePage = () => {
    const { user, logout } = useAuth();

    return (
        <div className="min-h-screen bg-gray-50">
            {/* Header */}
            <header className="bg-white shadow-sm border-b">
                <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
                    <div className="flex justify-between items-center h-16">
                        <div className="flex items-center">
                            <h1 className="text-xl font-semibold text-gray-900">
                                Moos Aksesuar
                            </h1>
                        </div>

                        <div className="flex items-center space-x-4">
                            <span className="text-sm text-gray-700">
                                Hoş geldin, {user?.id}
                            </span>
                            <Button
                                variant="secondary"
                                size="sm"
                                onClick={logout}
                            >
                                Çıkış Yap
                            </Button>
                        </div>
                    </div>
                </div>
            </header>

            {/* Main Content */}
            <main className="max-w-7xl mx-auto py-6 sm:px-6 lg:px-8">
                <div className="px-4 py-6 sm:px-0">
                    <div className="border-4 border-dashed border-gray-200 rounded-lg h-96 flex items-center justify-center">
                        <div className="text-center">
                            <h2 className="text-2xl font-bold text-gray-900 mb-4">
                                Ana Sayfa
                            </h2>
                            <p className="text-gray-600 mb-6">
                                E-ticaret projesi ana sayfası
                            </p>

                            <div className="grid grid-cols-1 md:grid-cols-3 gap-4 max-w-lg mx-auto">
                                <Button variant="primary" size="md">
                                    Ürünler
                                </Button>
                                <Button variant="secondary" size="md">
                                    Kategoriler
                                </Button>
                                <Button variant="secondary" size="md">
                                    Profil
                                </Button>
                            </div>
                        </div>
                    </div>
                </div>
            </main>

            {/* Footer */}
            <footer className="bg-white border-t">
                <div className="max-w-7xl mx-auto py-4 px-4 sm:px-6 lg:px-8">
                    <p className="text-center text-sm text-gray-500">
                        © 2024 Moos Aksesuar. Tüm hakları saklıdır.
                    </p>
                </div>
            </footer>
        </div>
    );
};

export default HomePage;