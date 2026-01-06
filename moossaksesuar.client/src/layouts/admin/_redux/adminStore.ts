import { configureStore } from "@reduxjs/toolkit";
import adminUiReducer from "./slice.ts"; // Admin arayüzü (sidebar vb.) durumları için
import themeConfigReducer from "@/layouts/admin/theme/themeConfigSlice.tsx";

export const adminStore = configureStore({
    reducer: {
        adminUi: adminUiReducer,
        themeConfig: themeConfigReducer,
    },
});

export type AdminRootState = ReturnType<typeof adminStore.getState>;
export type AdminDispatch = typeof adminStore.dispatch;