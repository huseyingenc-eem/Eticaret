import React from 'react';
import HeaderMainBar from './HeaderMainBar';
import HeaderCategories from './HeaderCategories';

const Header = () => {
    return (
        <div className="header shadow-sm border-bottom bg-white">
            <div className="px-xxl-5">
                <HeaderMainBar />
            </div>
            <HeaderCategories />
        </div>
    );
};

export default Header;