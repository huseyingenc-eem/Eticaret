import React from 'react';
import {Link} from "react-router-dom";

function AddFormButton(props) {
    return (
        <Link className={"btn btn-success mb-2"} to={props.url}>Ekle
            <i className={"bi bi-plus-square mx-1"}></i>
        </Link>
    );
}

export default AddFormButton;