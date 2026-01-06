// src/store/themeConfigSlice.tsx

// ... (importlar)

const initialState = {
    // Tarayıcının localStorage'ından 'theme' değerini okur, yoksa varsayılanı kullanır.
    theme: localStorage.getItem('theme') || themeConfig.theme,
    // ... diğer ayarlar
};

const themeConfigSlice = createSlice({
    name: 'auth',
    initialState: initialState,
    reducers: {
        toggleTheme(state, { payload }) {
            payload = payload || state.theme; // 'light', 'dark', veya 'system'
            localStorage.setItem('theme', payload); // Seçimi tarayıcıya kaydeder.
            state.theme = payload;

            if (payload === 'light') {
                state.isDarkMode = false;
            } else if (payload === 'dark') {
                state.isDarkMode = true;
            } else if (payload === 'system') {
                // Sistem tercihini kontrol eder.
                if (window.matchMedia && window.matchMedia('(prefers-color-scheme: dark)').matches) {
                    state.isDarkMode = true;
                } else {
                    state.isDarkMode = false;
                }
            }

            // En önemli kısım: body etiketine 'dark' class'ını ekler veya kaldırır.
            if (state.isDarkMode) {
                document.querySelector('body')?.classList.add('dark');
            } else {
                document.querySelector('body')?.classList.remove('dark');
            }
        },
        // ... diğer reducer'lar
    },
});

export const { toggleTheme, ... } = themeConfigSlice.actions;
export default themeConfigSlice.reducer;