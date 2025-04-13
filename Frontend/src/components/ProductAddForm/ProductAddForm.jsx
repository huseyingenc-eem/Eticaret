import React, { useState, useEffect } from 'react';
import axios from 'axios';

const ProductAddForm = ({ onSubmit }) => {
    const [formData, setFormData] = useState({
        name: '',
        price: '',
        stock: '',
        categoryId: ''
    });
    const [categories, setCategories] = useState([]);

    useEffect(() => {
        const fetchCategories = async () => {
            try {
                const response = await axios.get('https://localhost:7053/api/Category');
                setCategories(response.data);
            } catch (err) {
                console.error("Kategori çekilemedi:", err);
            }
        };
        fetchCategories();
    }, []);

    const handleChange = (e) => {
        const { name, value } = e.target;
        setFormData(prev => ({
            ...prev,
            [name]: value
        }));
    };

    const handleSubmit = (e) => {
        e.preventDefault();
        if (formData.name && formData.price && formData.stock && formData.categoryId) {
            onSubmit({
                ...formData,
                price: parseFloat(formData.price),
                stock: parseInt(formData.stock),
                categoryId: parseInt(formData.categoryId)
            });
            // Formu sıfırla
            setFormData({ name: '', price: '', stock: '', categoryId: '' });
        }
    };

    return (
        <form onSubmit={handleSubmit}>
            <div className="mb-3">
                <label className="form-label">Ürün Adı</label>
                <input
                    type="text"
                    className="form-control"
                    name="name"
                    value={formData.name}
                    onChange={handleChange}
                />
            </div>

            <div className="mb-3">
                <label className="form-label">Fiyat</label>
                <input
                    type="number"
                    className="form-control"
                    name="price"
                    value={formData.price}
                    onChange={handleChange}
                />
            </div>

            <div className="mb-3">
                <label className="form-label">Stok</label>
                <input
                    type="number"
                    className="form-control"
                    name="stock"
                    value={formData.stock}
                    onChange={handleChange}
                />
            </div>

            <div className="mb-3">
                <label className="form-label">Kategori</label>
                <select
                    className="form-select"
                    name="categoryId"
                    value={formData.categoryId}
                    onChange={handleChange}
                >
                    <option value="">Kategori Seçiniz</option>
                    {categories.map((category) => (
                        <option key={category.id} value={category.id}>
                            {category.name}
                        </option>
                    ))}
                </select>
            </div>

            <button type="submit" className="btn btn-primary">Ekle</button>
        </form>
    );
};

export default ProductAddForm;