import type { ErrorInfo, ReactNode } from 'react';
import { Component } from 'react';

interface Props {
    children: ReactNode;
}

interface State {
    hasError: boolean;
    error: Error | null;
    errorInfo: ErrorInfo | null;
}

class ErrorBoundary extends Component<Props, State> {
    public state: State = {
        hasError: false,
        error: null,
        errorInfo: null
    };

    public static getDerivedStateFromError(error: Error): State {
        return {
            hasError: true,
            error,
            errorInfo: null
        };
    }

    public componentDidCatch(error: Error, errorInfo: ErrorInfo) {
        console.error('Error Boundary yakaladı:', error, errorInfo);

        this.setState({
            error,
            errorInfo
        });
    }

    private handleReload = () => {
        window.location.reload();
    };

    private handleGoHome = () => {
        window.location.href = '/';
    };

    public render() {
        if (this.state.hasError) {
            return (
                <div className="min-h-screen flex items-center justify-center bg-gray-50 p-4">
                    <div className="max-w-md w-full bg-white rounded-lg shadow-lg p-6 text-center">
                        <div className="mb-4">
                            <div className="mx-auto w-16 h-16 bg-red-100 rounded-full flex items-center justify-center mb-4">
                                <svg className="w-8 h-8 text-red-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-2.5L13.732 4c-.77-.833-1.964-.833-2.732 0L3.732 16.5c-.77.833.192 2.5 1.732 2.5z" />
                                </svg>
                            </div>
                            <h1 className="text-xl font-bold text-gray-900 mb-2">
                                Bir Hata Oluştu
                            </h1>
                            <p className="text-gray-600 mb-4">
                                Sayfa yüklenirken beklenmeyen bir hata oluştu.
                            </p>
                        </div>

                        {/* Development ortamında hata detayları */}
                        {import.meta.env.DEV && this.state.error && (
                            <div className="mb-4 p-3 bg-gray-100 rounded text-left text-xs">
                                <details>
                                    <summary className="cursor-pointer font-medium text-red-600 mb-2">
                                        Hata Detayları (Geliştirici)
                                    </summary>
                                    <div className="text-gray-700">
                                        <p className="font-medium">Hata:</p>
                                        <p className="mb-2 break-words">{this.state.error.message}</p>

                                        {this.state.errorInfo && (
                                            <>
                                                <p className="font-medium">Stack:</p>
                                                <pre className="whitespace-pre-wrap break-words text-xs">
                          {this.state.errorInfo.componentStack}
                        </pre>
                                            </>
                                        )}
                                    </div>
                                </details>
                            </div>
                        )}

                        <div className="space-y-3">
                            <button
                                onClick={this.handleReload}
                                className="w-full px-4 py-2 bg-blue-500 text-white rounded hover:bg-blue-600 transition-colors"
                            >
                                Sayfayı Yenile
                            </button>

                            <button
                                onClick={this.handleGoHome}
                                className="w-full px-4 py-2 bg-gray-500 text-white rounded hover:bg-gray-600 transition-colors"
                            >
                                Ana Sayfaya Git
                            </button>
                        </div>
                    </div>
                </div>
            );
        }

        return this.props.children;
    }
}

export default ErrorBoundary;