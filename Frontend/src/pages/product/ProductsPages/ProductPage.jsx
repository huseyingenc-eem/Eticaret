import React, { useEffect, useState } from 'react';
import { useParams } from 'react-router-dom';
import axios from "axios";
import ProductCard from "../../../components/ProductCard/ProductCard.jsx";
import ModalForm from "../../../components/ModalForm/ModalForm.jsx";
import ProductAddForm from "../../../components/ProductAddForm/ProductAddForm.jsx";
import AddFormButton from "../../../components/Butons/AddButton/AddFormButton.jsx";

function ProductsPage() {
    const { categoryRange } = useParams();
    const [data, setData] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);
    const [showModal, setShowModal] = useState(false);

    // Ürünleri getir
    const fetchData = async () => {
        setLoading(true);
        try {
            let response;
            if (categoryRange?.includes('-')) {
                response = await axios.get('https://localhost:7053/api/Products');
            } else {
                response = await axios.get(`https://localhost:7053/api/Products/getallbycategory?categoryId=${categoryRange}`);
            }
            setData(response.data);
            setLoading(false);
        } catch (err) {
            setError(err.message);
            setLoading(false);
        }
    };

    useEffect(() => {
        fetchData();
    }, [categoryRange]);

    // Ürün ekle ve backend'e gönder
    const handleProductAdd = async (product) => {
        try {
            const response = await axios.post('https://localhost:7053/api/Products', product, {
                headers: {
                    'Content-Type': 'application/json'
                }
            });
            // API'den gelen ürünü direkt listeye ekle
            setData(prev => [...prev, response.data]);
            setShowModal(false);
        } catch (err) {
            console.error("Ürün eklenemedi:", err);
        }
    };

    if (loading) return <div className="text-center mt-4">Yükleniyor...</div>;
    if (error) return <div className="text-center mt-4 text-danger">Hata: {error}</div>;

    return (
        <>
            <div className="container">
                <h2 className="my-4 text-center">Ürünler</h2>
                <button className="btn btn-success mb-4" onClick={() => setShowModal(true)}>Ürün Ekle</button>
                <AddFormButton url={"https://localhost:7053/api/Products"} />
                <div className="row g-4 justify-content-center">
                    {data.map((product) => (
                        <div key={product.id} className="col-12 col-sm-6 col-md-4">
                            <ProductCard product={product} categoryRange={categoryRange} />
                        </div>
                    ))}
                    {data.length === 0 && <div className="text-center">Ürün bulunamadı.</div>}
                </div>
            </div>

            {/* Modal */}
            <ModalForm show={showModal} handleClose={() => setShowModal(false)} title="Yeni Ürün Ekle">
                <ProductAddForm onSubmit={handleProductAdd} />
            </ModalForm>

        </>
    );
}

export default ProductsPage;
