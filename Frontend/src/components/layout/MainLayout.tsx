import React from 'react';
import Header from './Header';
import Footer from './Footer';
import { Outlet } from 'react-router-dom';

const MainLayout: React.FC = () => {
    return (
        <div className="d-flex flex-column min-vh-100">
            <Header />
            <main className="flex-fill container py-4">
                <Outlet /> {/* Nested route'lar buraya gelir */}
            </main>
            <Footer />
        </div>
    );
};

export default MainLayout;
