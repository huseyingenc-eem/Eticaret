export default {
    darkMode: ['class', '.black'],
    content: ['./index.html', './src/**/*.{ts,tsx,js,jsx}'],
    theme: {
        extend: {
            colors: {
                border:     'var(--border)',
                input:      'var(--input)',
                ring:       'var(--ring)',
                background: 'var(--background)',
                foreground: 'var(--foreground)',
                card:      { DEFAULT: 'var(--card)',     foreground: 'var(--card-foreground)' },
                popover:   { DEFAULT: 'var(--popover)',  foreground: 'var(--popover-foreground)' },
                muted:     { DEFAULT: 'var(--muted)',    foreground: 'var(--muted-foreground)' },
                accent:    { DEFAULT: 'var(--accent)',   foreground: 'var(--accent-foreground)' },
                primary:   { DEFAULT: 'var(--primary)',  foreground: 'var(--primary-foreground)' },
            },
        },
    },
    plugins: [],
}
