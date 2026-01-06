// src/configs/navigation/admin/sidebar.config.ts
import type { NavLink } from '@/configs/types';

export const sidebarNav: NavLink[] = [
    { id: 'dash', label: 'Dashboard', to: '/admin', icon: 'LayoutDashboard', roles: ['Admin','Manager'] },
    {
        id: 'orders', label: 'Siparişler', icon: 'ShoppingCart', roles: ['Admin','Manager','Support'],
        children: [
            {  id: 'orders_all',
                label: 'Tüm Siparişler',
                to: '/admin/orders',
                children: [
                    { id: 'orders_draft', label: 'Taslaklar', to: '/admin/orders?status=draft' },
                    { id: 'orders_open',  label: 'Açık Siparişler', to: '/admin/orders?status=open' },
                    { id: 'orders_closed',label: 'Kapananlar', to: '/admin/orders?status=closed' },
                ],
            },

            { id: 'orders_pending', label: 'Bekleyenler',           to: '/admin/orders?status=pending' },
            { id: 'orders_returns', label: 'İade/İptaller',         to: '/admin/orders/returns' },
            { id: 'orders_new',     label: 'Yeni Sipariş Oluştur',  to: '/admin/orders/new' },
        ],
    },

    {
        id: 'products', label: 'Ürünler', icon: 'Package', roles: ['Admin','Manager','Editor'],
        children: [
            { id: 'products_all', label: 'Tüm Ürünler',          to: '/admin/products' },
            { id: 'products_new', label: 'Yeni Ürün',            to: '/admin/products/new' },
            { id: 'collections',  label: 'Koleksiyonlar',        to: '/admin/collections' },
            { id: 'categories',   label: 'Kategoriler',          to: '/admin/categories' },
            { id: 'brands',       label: 'Markalar',             to: '/admin/catalog/brands' },
            { id: 'attributes',   label: 'Özellikler / Varyant', to: '/admin/catalog/attributes' },
        ],
    },

    {
        id: 'inventory', label: 'Stok & Depo', icon: 'Boxes', roles: ['Admin','Manager'],
        children: [
            { id: 'inv_levels',    label: 'Stok Düzeyleri',   to: '/admin/inventory' },
            { id: 'inv_moves',     label: 'Stok Hareketleri', to: '/admin/inventory/movements' },
            { id: 'warehouses',    label: 'Depolar',          to: '/admin/warehouses' },
        ],
    },

    {
        id: 'discounts', label: 'İndirimler', icon: 'BadgePercent', roles: ['Admin','Manager','Editor'],
        children: [
            { id: 'coupons',    label: 'Kuponlar',     to: '/admin/discounts/coupons' },
            { id: 'campaigns',  label: 'Kampanyalar',  to: '/admin/discounts/campaigns' },
        ],
    },

    {
        id: 'customers', label: 'Müşteriler', icon: 'Users', roles: ['Admin','Manager','Support'],
        children: [
            { id: 'customers_all', label: 'Tüm Müşteriler', to: '/admin/customers' },
            { id: 'segments',      label: 'Segmentler',     to: '/admin/customers/segments' },
            { id: 'reviews',       label: 'Yorumlar',       to: '/admin/reviews' },
        ],
    },
    {
        id: 'suppliers', label: 'Tedarikçiler', icon: 'supplier', roles: ['Admin','Manager','Support'],
        children: [
            { id: 'suppliers_all', label: 'Tüm Tedarikçiler', to: '/admin/suppliers' },
            { id: 'segments',      label: 'Segmentler',     to: '/admin/customers/segments' },
            { id: 'reviews',       label: 'Yorumlar',       to: '/admin/reviews' },
        ],
    },
    {
        id: 'marketing', label: 'Pazarlama', icon: 'Megaphone', roles: ['Admin','Manager','Editor'],
        children: [
            { id: 'email',   label: 'E-posta Kampanyaları', to: '/admin/marketing/email' },
            { id: 'sms',     label: 'SMS Kampanyaları',     to: '/admin/marketing/sms' },
            { id: 'banners', label: 'Banner / Slider',      to: '/admin/marketing/banners' },
        ],
    },

    {
        id: 'analytics', label: 'Raporlar', icon: 'BarChart3', roles: ['Admin','Manager'],
        children: [
            { id: 'sales',    label: 'Satış Raporu',        to: '/admin/analytics/sales' },
            { id: 'products', label: 'Ürün Performansı',    to: '/admin/analytics/products' },
            { id: 'cust',     label: 'Müşteri Raporu',      to: '/admin/analytics/customers' },
        ],
    },

    {
        id: 'content', label: 'İçerik', icon: 'FileText', roles: ['Admin','Editor'],
        children: [
            { id: 'pages',  label: 'Sayfalar',            to: '/admin/content/pages' },
            { id: 'blog',   label: 'Blog',                to: '/admin/content/blog' },
            { id: 'menus',  label: 'Menüler',             to: '/admin/content/menus' },
            { id: 'media',  label: 'Medya Kütüphanesi',   to: '/admin/media' },
        ],
    },

    {
        id: 'shipping', label: 'Kargo & Teslimat', icon: 'Truck', roles: ['Admin','Manager'],
        children: [
            { id: 'zones',   label: 'Bölgeler',  to: '/admin/shipping/zones' },
            { id: 'methods', label: 'Yöntemler', to: '/admin/shipping/methods' },
            { id: 'rates',   label: 'Ücretler',  to: '/admin/shipping/rates' },
        ],
    },

    {
        id: 'payments', label: 'Ödemeler', icon: 'CreditCard', roles: ['Admin','Manager'],
        children: [
            { id: 'providers',   label: 'Sağlayıcılar', to: '/admin/payments/providers' },
            { id: 'transactions',label: 'İşlemler',     to: '/admin/payments/transactions' },
            { id: 'refunds',     label: 'İadeler',      to: '/admin/payments/refunds' },
        ],
    },

    {
        id: 'taxes', label: 'Vergi', icon: 'Percent', roles: ['Admin'],
        children: [
            { id: 'tax_rates', label: 'Vergi Oranları', to: '/admin/taxes/rates' },
            { id: 'tax_rules', label: 'Kurallar',       to: '/admin/taxes/rules' },
        ],
    },

    {
        id: 'settings', label: 'Ayarlar', icon: 'Settings', roles: ['Admin'],
        children: [
            { id: 'store',   label: 'Mağaza Bilgileri', to: '/admin/settings/store' },
            { id: 'staff',   label: 'Kullanıcılar',     to: '/admin/users' },
            { id: 'roles',   label: 'Roller',           to: '/admin/roles' },
            { id: 'integr',  label: 'Entegrasyonlar',   to: '/admin/integrations' },
        ],
    },

    { id: 'support', label: 'Destek Talepleri', to: '/admin/support/tickets', icon: 'Ticket', roles: ['Admin','Support'] },
    { id: 'docs',    label: 'Dokümanlar',       to: 'https://docs.myapp.com', external: true, icon: 'Book' },

    {
        id: 'ui', label: 'Uİ', icon: '', roles: ['Admin'],
        children: [
            { id: 'button',   label: 'Button', to: '/admin/ui/buttons' },
            { id: 'timeline',   label: 'Timeline',     to: '/admin/ui/timeline' },
            { id: 'roles',   label: 'Roller',           to: '/admin/roles' },
            { id: 'integr',  label: 'Entegrasyonlar',   to: '/admin/integrations' },
        ],
    },
];
