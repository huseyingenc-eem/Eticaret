import {lazy} from 'react';

// Lazy-loaded page components
const Dashboard = lazy(() => import('../pages/Dashboard.jsx'));
const Products = lazy(() => import('../pages/Products.jsx'));
const ProductDetails = lazy(() => import('../pages/ProductDetails.js'));
const Category = lazy(() => import('../pages/Category.jsx'));
const Staff = lazy(() => import('../pages/Staff.js'));
const Customers = lazy(() => import('../pages/Customers.jsx'));
const CustomerOrder = lazy(() => import('../pages/CustomerOrder.js'));
const Orders = lazy(() => import('../pages/Orders.js'));
const OrderInvoice = lazy(() => import('../pages/OrderInvoice.js'));
const Coupons = lazy(() => import('../pages/Coupons.jsx'));
const Page404 = lazy(() => import('../pages/404.jsx'));
const EditProfile = lazy(() => import('../pages/EditProfile.jsx'));

const routes = [
    {path: '/dashboard', component: Dashboard},
    {path: '/products', component: Products},
    {path: '/product/:id', component: ProductDetails},
    {path: '/customers', component: Customers},
    {path: '/customer-order/:id', component: CustomerOrder},
    {path: '/our-staff', component: Staff},
    {path: '/orders', component: Orders},
    {path: '/order/:id', component: OrderInvoice},
    {path: '/setting', component: EditProfile},
    {path: '/edit-profile', component: EditProfile},
    {path: '/404', component: Page404},
    {path: '/category', component: Category}, // enable later if needed
    {path: '/coupons', component: Coupons},  // enable later if needed
];

export default routes;
