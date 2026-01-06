import { useState, useEffect } from 'react';

interface ImportErrorHandlerProps {
    children: React.ReactNode;
}

const ImportErrorHandler = ({ children }: ImportErrorHandlerProps) => {
    const [hasImportError, setHasImportError] = useState(false);
    const [errorDetails, setErrorDetails] = useState<string>('');

    useEffect(() => {
        const handleUnhandledRejection = (event: PromiseRejectionEvent) => {
            const error = event.reason;

            // Import hatalarını yakala
            if (error?.message?.includes('Failed to resolve import') ||
                error?.message?.includes('Does the file exist?')) {
                setHasImportError(true);
                setErrorDetails(error.message);
                event.preventDefault(); // Console'a düşmesin
            }
        };

        window.addEventListener('unhandledrejection', handleUnhandledRejection);

        return () => {
            window.removeEventListener('unhandledrejection', handleUnhandledRejection);
        };
    }, []);

    if (hasImportError) {
        return (
            <div className="min-h-screen flex items-center justify-center bg-gray-50 p-4">
                <div className="max-w-lg w-full bg-white rounded-lg shadow-lg p-6">
                    <div className="text-center mb-6">
                        <div className="mx-auto w-16 h-16 bg-yellow-100 rounded-full flex items-center justify-center mb-4">
                            <svg className="w-8 h-8 text-yellow-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 8v4m0 4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
                            </svg>
                        </div>

                        <h1 className="text-xl font-bold text-gray-900 mb-2">
                            Dosya Bulunamadı
                        </h1>

                        <p className="text-gray-600 mb-4">
                            İmport edilen bir dosya bulunamıyor.
                        </p>
                    </div>

                    {/* Hata detayı */}
                    <div className="mb-6 p-4 bg-yellow-50 rounded border border-yellow-200">
                        <h3 className="font-medium text-yellow-800 mb-2">Hata Detayı:</h3>
                        <p className="text-sm text-yellow-700 break-words">
                            {errorDetails}
                        </p>
                    </div>

                    {/* Çözüm önerileri */}
                    <div className="mb-6 p-4 bg-blue-50 rounded border border-blue-200">
                        <h3 className="font-medium text-blue-800 mb-2">Çözüm Önerileri:</h3>
                        <ul className="text-sm text-blue-700 space-y-1">
                            <li>• Dosya yolunu kontrol edin</li>
                            <li>• Dosyanın mevcut olduğundan emin olun</li>
                            <li>• Import syntax'ını kontrol edin</li>
                            <li>• Dosya uzantısını kontrol edin (.ts, .tsx, .js)</li>
                        </ul>
                    </div>

                    <div className="space-y-3">
                        <button
                            onClick={() => window.location.reload()}
                            className="w-full px-4 py-2 bg-blue-500 text-white rounded hover:bg-blue-600 transition-colors"
                        >
                            Sayfayı Yenile
                        </button>

                        <button
                            onClick={() => window.location.href = '/'}
                            className="w-full px-4 py-2 bg-gray-500 text-white rounded hover:bg-gray-600 transition-colors"
                        >
                            Ana Sayfaya Git
                        </button>
                    </div>
                </div>
            </div>
        );
    }

    return <>{children}</>;
};

export default ImportErrorHandler;