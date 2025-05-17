import axios from 'axios';
import Cookies from 'js-cookie';

console.log('API BASE URL:', process.env.REACT_APP_API_BASE_URL); // TEST

const instance = axios.create({
  baseURL: process.env.REACT_APP_API_BASE_URL,
  timeout: 5000,
  headers: {
    Accept: 'application/json',
    'Content-Type': 'application/json',
  },
});

instance.interceptors.request.use((config) => {
  const adminInfo = Cookies.get('adminInfo')
      ? JSON.parse(Cookies.get('adminInfo'))
      : null;

  if (adminInfo?.token) {
    config.headers.authorization = `Bearer ${adminInfo.token}`;
  }

  return config;
});

const responseBody = (response) => response.data;

const requests = {
  get: (url, config) => instance.get(url, config).then(responseBody),
  post: (url, body) => instance.post(url, body).then(responseBody),
  put: (url, body) => instance.put(url, body).then(responseBody),
  patch: (url, body) => instance.patch(url, body).then(responseBody),
  delete: (url) => instance.delete(url).then(responseBody),
};

export default requests;
