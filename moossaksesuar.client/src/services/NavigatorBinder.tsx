// src/services/NavigatorBinder.tsx
import { useEffect } from "react";
import { useNavigate } from "react-router-dom";
import { setNavigator } from "./navigation";

export default function NavigatorBinder() {
    const navigate = useNavigate();
    useEffect(() => setNavigator(navigate), [navigate]);
    return null;
}
