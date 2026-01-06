import { useState } from "react";
import {
    Plus,
    Folder,
    TrendingUp,
    RefreshCw,
} from "lucide-react";
import { Button } from "@/components/ui";
import CategoryTable from "./_components/CategoryTable";

// Mock data - gerçek projenizde API'den gelecek
const mockStats = {
    total: 156,
    active: 134,
    inactive: 22,
    withProducts: 89,
    trending: [
        { name: "Elektronik", growth: "+12%" },
        { name: "Moda", growth: "+8%" },
        { name: "Ev & Yaşam", growth: "+5%" }
    ]
};

export default function CategoriesIndexPage() {

    const [isRefreshing, setIsRefreshing] = useState(false);

    const handleRefresh = async () => {
        setIsRefreshing(true);
        // API refresh logic here
        setTimeout(() => setIsRefreshing(false), 1000);
    };

    return (
        <div className="min-h-screen bg-gray-50">
            {/* Page Header */}
            <div className="bg-white border-b border-gray-200 sticky top-0 z-10">
                <div className="px-6 py-8">
                    <div className="flex flex-col lg:flex-row lg:items-center lg:justify-between gap-6">
                        {/* Title & Description */}
                        <div className="flex items-start gap-4">
                            <div className="w-12 h-12 bg-gradient-to-br from-blue-500 to-purple-600 rounded-2xl flex items-center justify-center shadow-lg">
                                <Folder className="w-6 h-6 text-white" />
                            </div>
                            <div className="min-w-0 flex-1">
                                <h1 className="text-2xl font-bold text-gray-900 tracking-tight">
                                    Kategori Yönetimi
                                </h1>
                                <p className="mt-1 text-sm text-gray-600 max-w-2xl">
                                    Ürün kategorilerinizi düzenleyin, hiyerarşi oluşturun ve performanslarını takip edin
                                </p>
                            </div>
                        </div>

                        {/* Quick Actions */}
                        <div className="flex items-center gap-3 flex-shrink-0">
                            <Button color="secondary" size="sm" onClick={handleRefresh} disabled={isRefreshing}>
                                <RefreshCw className={`w-4 h-4 mr-2 ${isRefreshing ? 'animate-spin' : ''}`} />
                                Yenile
                            </Button>
                            <Button color="primary" size="sm">
                                <Plus className="w-4 h-4 mr-2" />
                                Kategori Ekle
                            </Button>
                        </div>
                    </div>
                </div>
            </div>

            <div className="p-6 space-y-6">
                {/* Trending Categories */}
                <div className="bg-white rounded-2xl p-6 shadow-sm border border-gray-100">
                    <div className="flex items-center justify-between mb-6">
                        <div className="flex items-center gap-3">
                            <div className="w-10 h-10 bg-gradient-to-br from-green-400 to-blue-500 rounded-xl flex items-center justify-center">
                                <TrendingUp className="w-5 h-5 text-white" />
                            </div>
                            <div>
                                <h3 className="text-lg font-semibold text-gray-900">Trend Kategoriler</h3>
                                <p className="text-sm text-gray-500">Son 30 günde en çok büyüyen kategoriler</p>
                            </div>
                        </div>
                    </div>

                    <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
                        {mockStats.trending.map((trend, index) => (
                            <div key={index} className="flex items-center justify-between p-4 bg-gray-50 rounded-xl border border-gray-100 hover:bg-gray-100 transition-colors">
                                <div className="flex items-center gap-3">
                                    <div className="w-8 h-8 bg-white rounded-lg flex items-center justify-center shadow-sm">
                                        <span className="text-sm font-bold text-gray-700">{index + 1}</span>
                                    </div>
                                    <span className="font-medium text-gray-900">{trend.name}</span>
                                </div>
                                <span className="text-green-600 font-semibold text-sm">{trend.growth}</span>
                            </div>
                        ))}
                    </div>
                </div>

                {/* Table/Grid Content */}
                <div className="bg-white rounded-2xl shadow-sm border border-gray-100 overflow-hidden">
                    <CategoryTable />
                </div>
            </div>
        </div>
    );
}