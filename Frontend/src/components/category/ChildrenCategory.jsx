import React, { useEffect, useState } from 'react';

import useAsync from '../../hooks/useAsync';
import CategoryServices from '../../services/CategoryServices';


const ChildrenCategory = ({ value }) => {
  const [categories, setCategories] = useState([]);

  const { data } = useAsync(CategoryServices.getAllCategory);
  useEffect(() => {
    if (value) {
      const result = data.filter((parent) =>
        parent.parent.toLowerCase().includes(value.toLowerCase())
      );
      setCategories(result);
    } else {
      setCategories(data);
    }
  }, [data, value]);

  return (
    <>
      {categories.map((parent) => {
        return parent.children.map((child) => (
            <option key={`${parent._id}-${child}`} value={child}>
              {child}
            </option>
        ));
      })}
    </>
  );
};

export default ChildrenCategory;
