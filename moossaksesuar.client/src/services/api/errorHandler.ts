import toast from 'react-hot-toast';
import type { AxiosError } from 'axios';
import type { ApiError } from './baseApi';

export const handleApiError = (error: AxiosError<ApiError>) => {
    if (!error.response) {
        // Network hatası
        toast.error('Bağlantı hatası! İnternet bağlantınızı kontrol edin.');
        return;
    }

    const { status, data } = error.response;

    // Özel durumlar
    if (status === 401) {
        handleAuthError();
        return;
    }

    if (status === 403) {
        toast.error('Bu işlem için yetkiniz bulunmuyor.');
        return;
    }

    // Backend'den gelen userFriendlyMessage öncelikli
    if (data?.userFriendlyMessage) {
        toast.error(data.userFriendlyMessage);
        return;
    }

    // Validation hataları (errors object'i)
    if (data?.errors && Object.keys(data.errors).length > 0) {
        // İlk validation hatasını göster
        const firstFieldErrors = Object.values(data.errors)[0];
        if (Array.isArray(firstFieldErrors) && firstFieldErrors.length > 0) {
            toast.error(firstFieldErrors[0]);
        }
        return;
    }

    // Son çare olarak title, detail veya genel mesaj
    const message = data?.title || data?.detail || 'Beklenmeyen bir hata oluştu.';
    toast.error(message);

    // Development ortamında detaylı log
    if (import.meta.env.DEV) {
        console.group('🚨 API Error Details');
        console.error('Status:', status);
        console.error('Error Code:', data?.errorCode);
        console.error('Data:', data);
        if (data?.developerDetail) {
            console.error('Developer Detail:', data.developerDetail);
        }
        console.groupEnd();
    }
};

const handleAuthError = () => {
    toast.error('Oturumunuz sonlanmıştır. Lütfen tekrar giriş yapın.');

    // Token'ları temizle
    localStorage.removeItem('token');
    localStorage.removeItem('refreshToken');

    // 2 saniye sonra login sayfasına yönlendir
    setTimeout(() => {
        window.location.href = '/auth/login';
    }, 2000);
};