// src/utils/validation/email/emailUtils.ts
export const isValidEmailDomain = (email: string): boolean => {
    const domain = email.split('@')[1];
    if (!domain) return false;

    if (!domain.includes('.')) return false;

    if (domain.length < 3 || domain.length > 255) return false;

    return true;
};

export const getEmailProvider = (email: string): string | null => {
    const domain = email.split('@')[1]?.toLowerCase();

    const providers: Record<string, string> = {
        'icloud.com': 'iCloud',
        'yandex.com': 'Yandex',
        'mynet.com': 'Mynet'
    };

    return providers[domain] || null;
};