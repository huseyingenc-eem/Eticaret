import requests from './httpService.js';


const API_BASE_URL = '/Category'; // Backend API'nizin temel URL'si

const CategoryServices = {
  getAllCategory() {
    return requests.get(`${API_BASE_URL}/GetCategoryTree`);
  },

  getCategoryById(id) {
    return requests.get(`/category/${id}`);
  },

  addCategory(body) {
    return requests.post('/category/add', body);
  },

  updateCategory(id, body) {
    return requests.put(`/category/${id}`, body);
  },

  updateStatus(id, body) {
    return requests.put(`/category/status/${id}`, body);
  },

  deleteCategory(id, body) {
    return requests.patch(`/category/${id}`, body);
  },
};

export default CategoryServices;
