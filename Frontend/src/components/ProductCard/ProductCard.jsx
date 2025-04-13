import React from 'react';
import { Link } from 'react-router-dom';

const ProductCard = ({ product, categoryRange }) => {
    return (
        <div className="card" style={{ width: '18rem' }}>
            {/* Ürün Görseli */}
            <img src="https://via.placeholder.com/286x180.png?text=Product+Image" className="card-img-top" alt={product.name} />

            <div className="card-body">
                {/* Ürün İsmi */}
                <h5 className="card-title">{product.name}</h5>

                {/* Ürün Fiyatı */}
                <p className="card-text">Fiyat: {product.price} ₺</p>

                {/* Ürün Stok */}
                <p className="card-text">Stok: {product.stock}</p>

                {/* Ürün Detay Linki */}
                <Link to={`/category/${categoryRange}/product/${product.id}`} className="btn btn-primary">Detaya Git</Link>
            </div>
        </div>
    );
};

export default ProductCard;