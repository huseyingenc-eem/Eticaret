import { createSlice } from '@reduxjs/toolkit';
import themeConfig from '@/theme.config';

const initialState = {
    theme: localStorage.getItem('theme') || themeConfig.theme,
    isDarkMode: false,
};

const themeConfigSlice = createSlice({
    name: 'themeConfig',
    initialState: initialState,
    reducers: {
        toggleTheme(state, { payload }) {
            payload = payload || state.theme;
            localStorage.setItem('theme', payload);
            state.theme = payload;

            if (payload === 'light') {

                state.isDarkMode = false;
            } else if (payload === 'dark') {

                state.isDarkMode = true;
            } else if (payload === 'system') {
                if (window.matchMedia && window.matchMedia('(prefers-color-scheme: dark)').matches) {
                    state.isDarkMode = true;
                } else {
                    state.isDarkMode = false;
                }
            }
            if (state.isDarkMode) {
                document.body.classList.add('dark', 'black');
            } else {
                document.body.classList.remove('dark', 'black');
            }
        },
    },
});

export const { toggleTheme } = themeConfigSlice.actions;
export default themeConfigSlice.reducer;