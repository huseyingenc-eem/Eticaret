import React from 'react';
import './ModalFormDesign.css';

const ModalForm = ({ show, handleClose, title, children }) => {
    if (!show) return null;

    return (
        <div className="modal-overlay">
            <div className="modal-container">
                <div className="modal-header">
                    <h5>{title}</h5>
                    <button className="close-btn" onClick={handleClose}>X</button>
                </div>
                <div className="modal-body">
                    {children}
                </div>
            </div>
        </div>
    );
};

export default ModalForm;