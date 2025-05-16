import axiosInstance from '../axiosInstance.ts';
import type { LoginCommand, RegisterCommand, AccessTokenDto} from '../../types/authTypes.ts'; // Tiplerinizi import edin

export const loginRequest = (credentials: LoginCommand) => {
    return axiosInstance.post<AccessTokenDto>('/Auth/login', credentials); // Yanıt tipini belirtebilirsiniz
};

export const registerRequest = (userData: RegisterCommand) => {
    return axiosInstance.post<AccessTokenDto>('/Auth/register', userData); // Backend'den token dönüyorsa
};

export const getCurrentUserRequest = () => {
    // Backend User DTO'nuz için bir tip oluşturmanız iyi olur
    return axiosInstance.get<any>('/Auth/current');
};

export const logoutRequest = () => {
    // Backend'de bir logout endpoint'i varsa (örneğin token'ı geçersiz kılmak için)
    // return axiosInstance.post('/Auth/logout');
    // Sadece frontend'de token silme işlemi yapılacaksa bu fonksiyon `authService.ts` içinde de olabilir.
    return Promise.resolve(); // Backend'de logout endpoint'i yoksa
};