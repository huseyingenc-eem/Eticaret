import { lazy } from 'react';

// use lazy for better code splitting
const Dashboard = lazy(() => import('../pages/Dashboard.js'));
const Products = lazy(() => import('../pages/Products.js'));
const ProductDetails = lazy(() => import('../pages/ProductDetails.js'));
const Category = lazy(() => import('../pages/Category.js'));
const Staff = lazy(() => import('../pages/Staff.js'));
const Customers = lazy(() => import('../pages/Customers.js'));
const CustomerOrder = lazy(() => import('../pages/CustomerOrder.js'));
const Orders = lazy(() => import('../pages/Orders.js'));
const OrderInvoice = lazy(() => import('../pages/OrderInvoice.js'));
const Coupons = lazy(() => import('../pages/Coupons.js'));
// const Setting = lazy(() => import("../pages/Setting"));
const Page404 = lazy(() => import('../pages/404.js'));
const EditProfile = lazy(() => import('../pages/EditProfile.js'));

/*
//  * ⚠ These are internal routes!
//  * They will be rendered inside the app, using the default `containers/Layout`.
//  * If you want to add a route to, let's say, a landing page, you should add
//  * it to the `App`'s router, exactly like `Login`, `CreateAccount` and other pages
//  * are routed.
//  *
//  * If you're looking for the links rendered in the SidebarContent, go to
//  * `routes/sidebar.js`
 */

const routes = [
  {
    path: '/dashboard',
    component: Dashboard,
  },
  {
    path: '/products',
    component: Products,
  },
  {
    path: '/product/:id',
    component: ProductDetails,
  },
  // {
  //   path: '/category',
  //   component: Category,
  // },
  {
    path: '/customers',
    component: Customers,
  },
  {
    path: '/customer-order/:id',
    component: CustomerOrder,
  },
  {
    path: '/our-staff',
    component: Staff,
  },
  {
    path: '/orders',
    component: Orders,
  },
  {
    path: '/order/:id',
    component: OrderInvoice,
  },
  // {
  //   path: '/coupons',
  //   component: Coupons,
  // },
  { path: '/setting', component: EditProfile },
  {
    path: '/404',
    component: Page404,
  },
  {
    path: '/edit-profile',
    component: EditProfile,
  },
];

export default routes;
