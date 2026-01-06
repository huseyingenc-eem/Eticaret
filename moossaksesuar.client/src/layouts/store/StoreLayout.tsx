import { Outlet } from "react-router-dom";
import StoreHeader from "./StoreHeader";
import StoreFooter from "./StoreFooter";
import Container from "@/layouts/store/_components/Container";

export default function StoreLayout() {
    return (
        <div className="min-h-screen flex flex-col bg-white text-gray-900">
            <StoreHeader />
            <main className="flex-1">
                <Container className="py-6">
                    <Outlet />
                </Container>
            </main>
            <StoreFooter />
        </div>
    );
}
