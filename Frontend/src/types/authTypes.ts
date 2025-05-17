// src/types/authTypes.ts

// ETicaret.Application/Features/Authentication/Command/Register/RegisterCommand.cs'e karşılık gelir
export interface RegisterCommand {
    firstName: string;
    lastName: string;
    email: string;
    city?: string; // Backend'de nullable ise '?' ekleyin
    password: string;
}

// ETicaret.Application/Features/Authentication/Command/Login/LoginCommand.cs'e karşılık gelir
export interface LoginCommand {
    email: string; // Backend'iniz email ile giriş yapıyorsa
    password: string;
    // Eğer backend username ile de giriş kabul ediyorsa:
    // userName?: string;
    // emailOrUserName?: string; // Veya tek bir alan
}

// ETicaret.Application/Services/JwtServices/AccessTokenDto.cs'e karşılık gelir
export interface AccessTokenDto {
    token: string;
    tokenExpiration: string; // Genellikle ISO 8601 formatında bir tarih string'i
}

// Opsiyonel: Backend'deki User entity'nizin frontend'de kullanacağınız temel alanları için bir tip
// ETicaret.Presentation/Controllers/AuthController.cs GetCurrentUser endpoint'inden dönen yapıya göre
export interface CurrentUser {
    id: string;
    // userName: string; // Eğer dönüyorsa
    // email: string; // Eğer dönüyorsa
    roles: string[];
    // firstName, lastName gibi ek bilgiler de eklenebilir
}