// src/pages/admin/Dashboard.tsx
export default function Dashboard() {
    return (
        <div className="space-y-6">
            {/* Başlık */}
            <div className="flex items-center justify-between">
                <h1 className="text-2xl font-semibold">Dashboard</h1>
                <div className="text-sm text-black/60">Son güncelleme: {new Date().toLocaleString()}</div>
            </div>

            {/* KPI Kartları */}
            <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-4">
                <div className="rounded-2xl border bg-white p-4">
                    <div className="text-sm text-black/60">Toplam Sipariş</div>
                    <div className="mt-2 text-2xl font-semibold">1,284</div>
                    <div className="mt-1 text-xs text-emerald-600">+4.2% bu hafta</div>
                </div>
                <div className="rounded-2xl border bg-white p-4">
                    <div className="text-sm text-black/60">Ciro</div>
                    <div className="mt-2 text-2xl font-semibold">₺ 382,450</div>
                    <div className="mt-1 text-xs text-emerald-600">+2.1% bu hafta</div>
                </div>
                <div className="rounded-2xl border bg-white p-4">
                    <div className="text-sm text-black/60">Aktif Kullanıcı</div>
                    <div className="mt-2 text-2xl font-semibold">5,904</div>
                    <div className="mt-1 text-xs text-rose-600">-1.3% bu hafta</div>
                </div>
                <div className="rounded-2xl border bg-white p-4">
                    <div className="text-sm text-black/60">İade Oranı</div>
                    <div className="mt-2 text-2xl font-semibold">2.4%</div>
                    <div className="mt-1 text-xs text-black/50">stabil</div>
                </div>
            </div>

            {/* Son Siparişler */}
            <div className="rounded-2xl border bg-white">
                <div className="px-4 py-3 border-b">
                    <h2 className="font-semibold">Son Siparişler</h2>
                </div>
                <div className="overflow-x-auto">
                    <table className="w-full text-sm">
                        <thead>
                        <tr className="text-left text-black/60">
                            <th className="px-4 py-3">#</th>
                            <th className="px-4 py-3">Müşteri</th>
                            <th className="px-4 py-3">Tutar</th>
                            <th className="px-4 py-3">Durum</th>
                            <th className="px-4 py-3">Tarih</th>
                        </tr>
                        </thead>
                        <tbody>
                        {[
                            { id: "ORD-20341", user: "Ayşe K.", total: "₺1.250", status: "Hazırlanıyor", date: "10.08.2025 13:22" },
                            { id: "ORD-20340", user: "Mehmet T.", total: "₺489", status: "Kargolandı",   date: "10.08.2025 12:58" },
                            { id: "ORD-20339", user: "Hüseyin G.", total: "₺2.900", status: "Tamamlandı", date: "10.08.2025 12:32" },
                        ].map((o) => (
                            <tr key={o.id} className="border-t">
                                <td className="px-4 py-3 font-medium">{o.id}</td>
                                <td className="px-4 py-3">{o.user}</td>
                                <td className="px-4 py-3">{o.total}</td>
                                <td className="px-4 py-3">
                    <span className="inline-flex items-center rounded-full border px-2 py-0.5 text-xs">
                      {o.status}
                    </span>
                                </td>
                                <td className="px-4 py-3 text-black/60">{o.date}</td>
                            </tr>
                        ))}
                        </tbody>
                    </table>
                </div>
                <div className="px-4 py-3 border-t text-right">
                    <button className="rounded-lg border px-3 py-1.5 text-sm">Tümünü Gör</button>
                </div>
            </div>
        </div>
    );
}
