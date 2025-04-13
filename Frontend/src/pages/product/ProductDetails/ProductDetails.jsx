import React, { useEffect, useState } from 'react';
import { useParams } from 'react-router-dom';
import './ProductDetails.css';

function ProductDetails() {
    const { categoryId, productId } = useParams();
    const [product, setProduct] = useState(null);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);

    useEffect(() => {
        const idToFetch = productId || categoryId; // Fallback in case only one param is present
        fetch(`https://localhost:7053/api/Products/getbyid?id=${productId || idToFetch}`)
            .then(res => {
                if (!res.ok) throw new Error('Veri çekilemedi');
                return res.json();
            })
            .then(data => {
                setProduct(data);
                setLoading(false);
            })
            .catch(err => {
                setError(err.message);
                setLoading(false);
            });
    }, [productId, categoryId]);

    if (loading) return <div>Yükleniyor...</div>;
    if (error) return <div className="text-danger">Hata: {error}</div>;

    return (
        <div className="product-details-container">
            <h2>{product.name}</h2>
            <p className="product-price">Fiyat: {product.price} ₺</p>
            <p className="product-stock">Stok: {product.stock} adet</p>
            <p>Kategori ID: {product.categoryID}</p>
        </div>
    );
}

export default ProductDetails;
