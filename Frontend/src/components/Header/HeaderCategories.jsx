import React, { useEffect, useState } from 'react';
import axios from 'axios';

const HeaderCategories = () => {
    const [categories, setCategories] = useState([]);
    const [activeCategory, setActiveCategory] = useState(null);
    const [activeSubcategory, setActiveSubcategory] = useState(null);

    useEffect(() => {
        const fetchCategories = async () => {
            try {
                const res = await axios.get('https://localhost:7053/api/Category/GetCategoryTree');
                setCategories(res.data);
            } catch (err) {
                console.error("Kategori yüklenemedi:", err);
            }
        };
        fetchCategories();
    }, []);

    return (
        <div className="header-categories border-top py-2 position-relative bg-white">
            <div className="container-fluid d-flex gap-4">
                {categories.map((category) => (
                    <div
                        key={category.id}
                        className="position-relative"
                        onMouseEnter={() => setActiveCategory(category.id)}
                        onMouseLeave={() => {
                            setActiveCategory(null);
                            setActiveSubcategory(null);
                        }}
                    >
                        <span className="fw-semibold text-dark px-2 py-1 cursor-pointer">{category.name}</span>

                        {activeCategory === category.id && category.children?.length > 0 && (
                            <div className="position-absolute top-100 start-0 w-100 bg-white shadow border mt-1 z-10">
                                <div className="d-flex">
                                    {/* Sol: Alt Kategoriler */}
                                    <div className="border-end p-3" style={{ minWidth: "200px" }}>
                                        {category.children.map((sub) => (
                                            <div
                                                key={sub.id}
                                                onMouseEnter={() => setActiveSubcategory(sub.id)}
                                                className={`py-1 px-2 rounded ${activeSubcategory === sub.id ? 'bg-light fw-bold' : 'text-muted'}`}
                                                style={{ cursor: 'pointer' }}
                                            >
                                                {sub.name}
                                            </div>
                                        ))}
                                    </div>

                                    {/* Sağ: Alt-alt içerikler */}
                                    <div className="p-3 flex-grow-1">
                                        {activeSubcategory &&
                                            category.children
                                                .find((sub) => sub.id === activeSubcategory)
                                                ?.children?.map((item) => (
                                                <div key={item.id} className="mb-3">
                                                    <h6 className="fw-bold">{item.name}</h6>
                                                    <ul className="list-unstyled">
                                                        <li className="text-muted small">Öne Çıkan Ürünler</li>
                                                        <li className="text-muted small">Yeni Gelenler</li>
                                                        <li className="text-muted small">İndirimli Ürünler</li>
                                                    </ul>
                                                </div>
                                            ))}
                                    </div>

                                    {/* Banner Alanı */}
                                    <div className="p-3" style={{ width: "220px" }}>
                                        <img
                                            src="https://images.unsplash.com/photo-1498049794561-7780e7231661?w=300"
                                            className="img-fluid rounded mb-3"
                                            alt="promo"
                                        />
                                        <img
                                            src="https://images.unsplash.com/photo-1460925895917-afdab827c52f?w=300"
                                            className="img-fluid rounded"
                                            alt="promo"
                                        />
                                    </div>
                                </div>
                            </div>
                        )}
                    </div>
                ))}
            </div>
        </div>
    );
};

export default HeaderCategories;
