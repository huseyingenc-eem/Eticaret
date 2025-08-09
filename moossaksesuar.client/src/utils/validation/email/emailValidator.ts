// src/utils/validation/email/emailValidator.ts
import type { ValidationResult, EmailValidationOptions } from '../types';

// Genel e-posta formatını kontrol eden Regex.
const EMAIL_REGEX = /^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/;


// const COMMON_PROVIDERS: { [key: string]: string } = {
//     'gmail.com': 'Gmail',
//     'yahoo.com': 'Yahoo',
//     'hotmail.com': 'Hotmail',
//     'outlook.com': 'Outlook',
// };

// E-posta adreslerinde bulunmaması gereken geçersiz desenler
const INVALID_PATTERNS = [
    /\.{2,}/, // Ardışık nokta (örn: user..name@)
    /^\./,    // Başlangıçta nokta (örn: .username@)
    /\.$/,    // Sonda nokta (örn: username.@)
    /@\./,    // '@' sonrası direkt nokta (örn: user@.com)
];

// Tek kullanımlık/geçici e-posta servislerinin domain'leri
const BLACKLISTED_DOMAINS = [
    '10minutemail.com',
    'guerrillamail.com',
    'mailinator.com',
    'tempmail.org'
];

export const validateEmail = (
    email: string,
    options: EmailValidationOptions = {}
): ValidationResult => {
    const {
        required = true,
        domains = [],
        blacklistedDomains = BLACKLISTED_DOMAINS
    } = options;

    // E-posta boş veya null ise kontrol et
    if (!email || email.trim() === '') {
        return {
            isValid: !required, // Gerekli değilse geçerli say
            error: required ? 'E-posta adresi gereklidir' : undefined
        };
    }

    // Boşlukları temizle ve küçük harfe çevir
    const trimmedEmail = email.trim().toLowerCase();

    // Genel e-posta formatını kontrol et
    if (!EMAIL_REGEX.test(trimmedEmail)) {
        return {
            isValid: false,
            error: 'Geçerli bir e-posta adresi girin'
        };
    }

    // Geçersiz format desenlerini kontrol et
    for (const pattern of INVALID_PATTERNS) {
        if (pattern.test(trimmedEmail.split('@')[0])) { // Sadece kullanıcı adını kontrol etmek daha güvenli olabilir
            return {
                isValid: false,
                error: 'E-posta adresi formatı geçersiz'
            };
        }
    }

    // Domain'i al
    const domain = trimmedEmail.split('@')[1];

    // İzin verilen domain listesini kontrol et
    if (domains.length > 0 && !domains.includes(domain)) {
        return {
            isValid: false,
            error: `Sadece ${domains.join(', ')} alan adlarından e-posta kabul edilir`
        };
    }

    // Kara listedeki domain'leri kontrol et
    if (blacklistedDomains.includes(domain)) {
        return {
            isValid: false,
            error: 'Geçici e-posta adresleri kabul edilmez'
        };
    }

    // Türkçe karakter kontrolü
    const turkishChars = /[çğıöşü]/i; // i flagi ile büyük/küçük harf duyarsız hale getirildi
    if (turkishChars.test(trimmedEmail)) {
        return {
            isValid: false,
            error: 'E-posta adresinde Türkçe karakter kullanılamaz'
        };
    }

    // Tüm kontrollerden geçerse
    return { isValid: true };
};