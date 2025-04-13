import React from 'react';
import { Link } from 'react-router-dom';
import './HeaderStyle.css';

const HeaderMainBar = () => (
    <div className="header-wrapper">
        <div className="top-link">
            <div className="top-link-container">
                <Link to="/orders">Siparişlerim</Link>
                <Link to="/campaigns">Kampanyalar</Link>
                <Link to="/support">Müşteri Hizmetleri</Link>
            </div>
        </div>

        <div className="header-mainbar">
            {/* Logo */}
            <div className="logo-area">
                <Link to="/">
                    <span className="logo-text">Eticaret</span>
                </Link>
            </div>

            {/* Search */}
            <div className="search-box">
                <span className="search-icon">
                    <i className="bi bi-search"></i>
                </span>
                <input type="text" placeholder="Ürün, kategori veya marka ara" />
            </div>

            {/* Login / Cart */}
            <div className="account-cart">
                <button className="login-button">
                    <i className="bi bi-person"></i>
                    <span>
                        <div className="login-bold">Giriş Yap</div>
                        <small>veya üye ol</small>
                    </span>
                </button>
                <button className="cart-button">
                    <i className="bi bi-cart"></i>
                    <span className="cart-label">Sepetim</span>
                    <span className="cart-badge">0</span>
                </button>
            </div>
        </div>
    </div>
);

export default HeaderMainBar;