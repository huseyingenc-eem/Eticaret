// src/layouts/admin/slice.ts
import { createSlice } from "@reduxjs/toolkit";

type AdminUiState = {};

const initialState: AdminUiState = {};

const adminUiSlice = createSlice({
    name: "adminUi",
    initialState,
    reducers: {},
});

export default adminUiSlice.reducer;
