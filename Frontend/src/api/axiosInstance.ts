// src/api/axiosInstance.ts
import axios from 'axios';

// .env dosyasından veya doğrudan buraya backend URL'nizi yazın
// launchSettings.json'dan: "https://localhost:7053" veya "http://localhost:5031"
// Ortam değişkeni kullanmak daha iyidir: process.env.REACT_APP_API_URL veya import.meta.env.VITE_API_URL
const API_BASE_URL = import.meta.env.VITE_API_URL || 'https://localhost:7053/api';

const axiosInstance = axios.create({
    baseURL: API_BASE_URL,
    headers: {
        'Content-Type': 'application/json',
    },
    // timeout: 10000, // İsteğe bağlı: istek zaman aşımı (milisaniye)
});

// --- İSTEK (REQUEST) INTERCEPTOR ---
// Her API isteği gönderilmeden önce çalışır.
// Genellikle Authorization token'ını header'a eklemek için kullanılır.
axiosInstance.interceptors.request.use(
    (config) => {
        const token = localStorage.getItem('token'); // Token'ı localStorage'dan (veya state'ten) al
        if (token && config.headers) {
            config.headers.Authorization = `Bearer ${token}`;
        }
        return config;
    },
    (error) => {
        // İstek hatası durumunda bir şeyler yapın
        return Promise.reject(error);
    }
);

// --- YANIT (RESPONSE) INTERCEPTOR ---
// API'den bir yanıt alındığında çalışır.
// Genel hata yönetimi (örneğin 401 Unauthorized) veya veri dönüşümü için kullanılabilir.
axiosInstance.interceptors.response.use(
    (response) => {
        // Başarılı yanıtlar için herhangi bir işlem yapılmasına gerek yok, doğrudan döndürülür.
        return response;
    },
    (error) => {
        // HTTP hata durumlarını burada merkezi olarak yönetebilirsiniz.
        if (error.response) {
            // Sunucudan bir yanıt geldi ancak durum kodu 2xx aralığında değil
            console.error('API Error Response:', error.response.data);
            console.error('Status:', error.response.status);
            console.error('Headers:', error.response.headers);

            if (error.response.status === 401) {
                // Token geçersiz veya süresi dolmuş olabilir.
                // Kullanıcıyı login sayfasına yönlendirme, token'ı temizleme gibi işlemler yapılabilir.
                // Örneğin:
                // localStorage.removeItem('token');
                // window.location.href = '/login'; // Veya React Router ile yönlendirme
                console.error('Yetkisiz istek! Token geçersiz veya süresi dolmuş olabilir.');
            }
            // Hatanın kendisini veya backend'den gelen hata detayını fırlat
            return Promise.reject(error.response.data || error.message || 'Bir API hatası oluştu.');
        } else if (error.request) {
            // İstek yapıldı ancak yanıt alınamadı (network hatası vb.)
            console.error('API Network Error:', error.request);
            return Promise.reject('Sunucuya ulaşılamadı. Lütfen internet bağlantınızı kontrol edin.');
        } else {
            // İsteği ayarlarken bir şeyler ters gitti
            console.error('API Request Setup Error:', error.message);
            return Promise.reject(error.message || 'İstek oluşturulurken bir hata oluştu.');
        }
    }
);

export default axiosInstance;