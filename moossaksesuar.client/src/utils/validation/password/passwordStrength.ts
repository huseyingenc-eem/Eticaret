export const getPasswordStrength = (password: string): {
    score: number;
    level: 'Çok Zayıf' | 'Zayıf' | 'Orta' | 'Güçlü' | 'Çok Güçlü';
    suggestions: string[];
} => {
    let score = 0;
    const suggestions: string[] = [];

    if (password.length >= 8) score += 1;
    else suggestions.push('En az 8 karakter kullanın');

    if (password.length >= 12) score += 1;
    else if (password.length >= 8) suggestions.push('12+ karakter daha güvenli');

    if (/[a-z]/.test(password)) score += 1;
    else suggestions.push('Küçük harf ekleyin');

    if (/[A-Z]/.test(password)) score += 1;
    else suggestions.push('Büyük harf ekleyin');

    if (/\d/.test(password)) score += 1;
    else suggestions.push('Rakam ekleyin');

    if (/[!@#$%^&*()_+\-=\[\]{};':"\\|,.<>\/?]/.test(password)) score += 1;
    else suggestions.push('Özel karakter ekleyin');

    if (password.length >= 16) score += 1;
    if (/[^\w\s]/.test(password)) score += 1;

    const levels = ['Çok Zayıf', 'Zayıf', 'Orta', 'Güçlü', 'Çok Güçlü'] as const;
    const level = levels[Math.min(Math.floor(score / 2), 4)];

    return { score, level, suggestions };
};