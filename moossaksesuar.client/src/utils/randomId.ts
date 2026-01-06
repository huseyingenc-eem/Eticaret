/**
 * Rastgele benzersiz ID üretir
 * @param length - ID uzunluğu (varsayılan: 8)
 * @param prefix - ID başına eklenecek prefix (opsiyonel)
 * @returns Benzersiz ID string'i
 */
export function randomId(length: number = 8, prefix?: string): string {
    const chars = 'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789';
    let result = '';

    for (let i = 0; i < length; i++) {
        result += chars.charAt(Math.floor(Math.random() * chars.length));
    }

    return prefix ? `${prefix}-${result}` : result;
}

/**
 * Timestamp ile benzersiz ID üretir (daha güvenli)
 * @param prefix - ID başına eklenecek prefix (opsiyonel)
 * @returns Timestamp bazlı benzersiz ID
 */
export function timestampId(prefix?: string): string {
    const timestamp = Date.now().toString(36);
    const random = Math.random().toString(36).substring(2, 8);
    const id = `${timestamp}${random}`;

    return prefix ? `${prefix}-${id}` : id;
}

/**
 * UUID v4 benzeri ID üretir
 * @returns UUID formatında ID
 */
export function uuidLike(): string {
    return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function(c) {
        const r = Math.random() * 16 | 0;
        const v = c === 'x' ? r : (r & 0x3 | 0x8);
        return v.toString(16);
    });
}

/**
 * Sayısal ID üretir
 * @param min - Minimum değer (varsayılan: 1000)
 * @param max - Maksimum değer (varsayılan: 9999)
 * @returns Rastgele sayı
 */
export function numericId(min: number = 1000, max: number = 9999): number {
    return Math.floor(Math.random() * (max - min + 1)) + min;
}

// Varsayılan export
export default randomId;