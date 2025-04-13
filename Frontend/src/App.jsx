import './App.css';
import { Routes, Route } from 'react-router-dom';
import Header from "./components/Header/Header.jsx";
import ProductsPage from "./pages/product/ProductsPages/ProductPage.jsx";
import HomePage from "./pages/HomePage.jsx";
import ProductDetails from "./pages/product/ProductDetails/ProductDetails.jsx";

function App() {
    return (
        <div>
            <Header />
            <main>
                <Routes>
                    <Route path="/" element={<HomePage />} />
                    <Route path="/" element={<HomePage />} />
                    <Route path="/category/:categoryRange" element={<ProductsPage />} />
                    <Route path="/category/:categoryRange/product/:productId" element={<ProductDetails />} />
                </Routes>
            </main>
        </div>
    );
}

export default App;