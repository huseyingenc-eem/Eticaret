// src/pages/admin/ui/Basic/CategoryEditForm.tsx
import React, {useEffect, useState} from "react";
import type {CategoryDetails, UpdateCategoryRequest} from "@/services/api/endpoints/category/category.types";
import {Save, Pencil, X, Tag, Eye, EyeOff, FileText, Info} from "lucide-react";
import {Button, Input} from "@/components/ui";
import toast from "react-hot-toast";

type Props = {
    initialData: CategoryDetails & { parentName?: string | null };
    onSave: (payload: UpdateCategoryRequest) => Promise<void>;
};

export function CategoryEditForm({initialData, onSave}: Props) {
    const [formData, setFormData] = useState({
        name: initialData.name,
        description: initialData.description ?? "",
        isActive: initialData.isActive,
    });
    const [isSaving, setIsSaving] = useState(false);
    const [isEditing, setIsEditing] = useState(false);

    useEffect(() => {
        setFormData({
            name: initialData.name,
            description: initialData.description ?? "",
            isActive: initialData.isActive,
        });
        setIsEditing(false);
    }, [initialData]);

    const hasChanges =
        formData.name !== initialData.name ||
        formData.description !== (initialData.description ?? "") ||
        formData.isActive !== initialData.isActive;

    const handleInputChange = (e: React.ChangeEvent<HTMLInputElement | HTMLTextAreaElement>) => {
        const {name, value} = e.target;
        setFormData((prev) => ({...prev, [name]: value}));
    };

    const handleCancel = () => {
        setFormData({
            name: initialData.name,
            description: initialData.description ?? "",
            isActive: initialData.isActive,
        });
        setIsEditing(false);
    };

    const handleSubmit = async (e: React.FormEvent) => {
        e.preventDefault();
        if (!hasChanges || isSaving) return;

        setIsSaving(true);
        try {
            await onSave({
                name: formData.name,
                description: formData.description,
                isActive: formData.isActive,
                parentId: initialData.parentId,
            });
            toast.success("Kategori bilgileri güncellendi.");
            setIsEditing(false);
        } finally {
            setIsSaving(false);
        }
    };

    return (
        <div className="overflow-hidden rounded-3xl bg-white shadow-xl border border-gray-100">
            {/* Header */}
            <div className="relative bg-gradient-to-r from-blue-50 to-indigo-50 px-6 py-8 border-b border-gray-100">
                <div className="flex flex-col sm:flex-row sm:items-center sm:justify-between gap-4">
                    <div className="flex items-start gap-4">
                        <div className="flex-shrink-0 w-12 h-12 bg-blue-500 rounded-2xl flex items-center justify-center shadow-lg">
                            <Tag className="w-6 h-6 text-white" />
                        </div>
                        <div className="min-w-0 flex-1">
                            <h3 className="text-xl font-bold text-gray-900 tracking-tight">
                                Kategori Bilgileri
                            </h3>
                            <p className="mt-1 text-sm text-gray-600 leading-relaxed">
                                Kategorinin temel özelliklerini görüntüleyin ve düzenleyin
                            </p>

                            {/* Parent Category Badge */}
                            {initialData.parentName && (
                                <div className="mt-3 flex items-center gap-2">
                                    <span className="text-xs text-gray-500">Üst kategori:</span>
                                    <span className="inline-flex items-center gap-1.5 px-3 py-1 text-xs font-medium bg-white text-blue-700 rounded-full border border-blue-200 shadow-sm">
                                        <Tag className="w-3 h-3" />
                                        {initialData.parentName}
                                    </span>
                                </div>
                            )}
                        </div>
                    </div>

                    {/* Action Buttons */}
                    <div className="flex items-center gap-3 flex-shrink-0">
                        {!isEditing ? (
                            <Button
                                color="primary"
                                size="sm"
                                type="button"
                                onClick={() => setIsEditing(true)}
                                className="shadow-lg hover:shadow-xl transition-all duration-200"
                            >
                                <Pencil className="w-4 h-4 mr-2" />
                                Düzenle
                            </Button>
                        ) : (
                            <div className="flex items-center gap-2">
                                <Button
                                    color="danger"
                                    size="sm"
                                    type="button"
                                    onClick={handleCancel}
                                    className="shadow-md hover:shadow-lg transition-shadow"
                                >
                                    <X className="w-4 h-4 mr-2" />
                                    <span className="hidden sm:inline">Vazgeç</span>
                                </Button>
                                <Button
                                    color="primary"
                                    size="sm"
                                    type="submit"
                                    disabled={!hasChanges || isSaving}
                                    loading={isSaving}
                                    onClick={handleSubmit}
                                    className="shadow-lg hover:shadow-xl transition-all duration-200"
                                >
                                    <Save className="w-4 h-4 mr-2" />
                                    Kaydet
                                </Button>
                            </div>
                        )}
                    </div>
                </div>

            </div>

            {/* Body */}
            <form onSubmit={handleSubmit} className="p-6 sm:p-8">
                {!isEditing ? (
                    /* View Mode */
                    <div className="space-y-8">
                        {/* Basic Info Grid */}
                        <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
                            {/* Category Name */}
                            <div className="group">
                                <div className="flex items-center gap-2 mb-3">
                                    <div className="w-8 h-8 bg-blue-100 rounded-lg flex items-center justify-center">
                                        <Tag className="w-4 h-4 text-blue-600" />
                                    </div>
                                    <label className="text-sm font-semibold text-gray-900">Kategori Adı</label>
                                </div>
                                <div className="relative">
                                    <div className="w-full px-4 py-4 bg-gray-50 border border-gray-200 rounded-xl text-sm font-medium text-gray-900 group-hover:bg-gray-100 transition-colors">
                                        {initialData.name}
                                    </div>
                                </div>
                            </div>

                            {/* Status */}
                            <div className="group">
                                <div className="flex items-center gap-2 mb-3">
                                    <div className="w-8 h-8 bg-green-100 rounded-lg flex items-center justify-center">
                                        {initialData.isActive ? (
                                            <Eye className="w-4 h-4 text-green-600" />
                                        ) : (
                                            <EyeOff className="w-4 h-4 text-gray-500" />
                                        )}
                                    </div>
                                    <label className="text-sm font-semibold text-gray-900">Durum</label>
                                </div>
                                <div className={`inline-flex items-center gap-3 px-4 py-3 rounded-xl border-2 ${
                                    initialData.isActive
                                        ? "border-green-200 bg-green-50"
                                        : "border-gray-200 bg-gray-50"
                                }`}>
                                    <div className={`w-3 h-3 rounded-full ${
                                        initialData.isActive ? "bg-green-500 animate-pulse" : "bg-gray-400"
                                    }`} />
                                    <span className={`text-sm font-medium ${
                                        initialData.isActive ? "text-green-800" : "text-gray-600"
                                    }`}>
                                        {initialData.isActive ? "Aktif" : "Pasif"}
                                    </span>
                                </div>
                            </div>
                        </div>

                        {/* Description */}
                        <div className="group">
                            <div className="flex items-center gap-2 mb-3">
                                <div className="w-8 h-8 bg-purple-100 rounded-lg flex items-center justify-center">
                                    <FileText className="w-4 h-4 text-purple-600" />
                                </div>
                                <label className="text-sm font-semibold text-gray-900">Açıklama</label>
                            </div>
                            <div className="relative">
                                <div className="w-full px-4 py-4 bg-gray-50 border border-gray-200 rounded-xl text-sm text-gray-700 min-h-[100px] group-hover:bg-gray-100 transition-colors whitespace-pre-wrap leading-relaxed">
                                    {initialData.description || (
                                        <span className="text-gray-400 italic flex items-center gap-2">
                                            <Info className="w-4 h-4" />
                                            Açıklama henüz eklenmemiş
                                        </span>
                                    )}
                                </div>
                            </div>
                        </div>
                    </div>
                ) : (
                    /* Edit Mode */
                    <div className="space-y-8">
                        {/* Form Grid */}
                        <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
                            {/* Category Name Input */}
                            <div className="space-y-3">
                                <div className="flex items-center gap-2">
                                    <div className="w-8 h-8 bg-blue-100 rounded-lg flex items-center justify-center">
                                        <Tag className="w-4 h-4 text-blue-600" />
                                    </div>
                                    <label htmlFor="name" className="text-sm font-semibold text-gray-900">
                                        Kategori Adı *
                                    </label>
                                </div>
                                <Input
                                    type="text"
                                    name="name"
                                    id="name"
                                    value={formData.name}
                                    onChange={handleInputChange}
                                    placeholder="Örn: Elektronik Ürünler"
                                    className="w-full"
                                    required
                                />
                            </div>

                            {/* Status Toggle */}
                            <div className="space-y-3">
                                <div className="flex items-center gap-2">
                                    <div className="w-8 h-8 bg-green-100 rounded-lg flex items-center justify-center">
                                        <Eye className="w-4 h-4 text-green-600" />
                                    </div>
                                    <label className="text-sm font-semibold text-gray-900">Durum</label>
                                </div>
                                <div className="flex items-center pt-2">
                                    <label className="relative inline-flex items-center cursor-pointer">
                                        <input
                                            type="checkbox"
                                            checked={formData.isActive}
                                            onChange={(e) => setFormData((prev) => ({...prev, isActive: e.target.checked}))}
                                            className="sr-only peer"
                                        />
                                        <div className="relative w-14 h-7 bg-gray-200 peer-focus:outline-none peer-focus:ring-4 peer-focus:ring-blue-300 rounded-full peer peer-checked:after:translate-x-full peer-checked:after:border-white after:content-[''] after:absolute after:top-[2px] after:left-[2px] after:bg-white after:border-gray-300 after:border after:rounded-full after:h-6 after:w-6 after:transition-all peer-checked:bg-blue-600"></div>
                                        <span className="ml-3 text-sm font-medium text-gray-700">
                                            {formData.isActive ? "Aktif" : "Pasif"}
                                        </span>
                                    </label>
                                </div>
                            </div>
                        </div>

                        {/* Description Textarea */}
                        <div className="space-y-3">
                            <div className="flex items-center gap-2">
                                <div className="w-8 h-8 bg-purple-100 rounded-lg flex items-center justify-center">
                                    <FileText className="w-4 h-4 text-purple-600" />
                                </div>
                                <label htmlFor="description" className="text-sm font-semibold text-gray-900">
                                    Açıklama
                                </label>
                            </div>
                            <div className="relative">
                                <textarea
                                    name="description"
                                    id="description"
                                    rows={4}
                                    value={formData.description}
                                    onChange={handleInputChange}
                                    placeholder="Bu kategori hakkında detaylı bir açıklama yazın..."
                                    className="w-full px-4 py-3 border border-gray-200 rounded-xl text-sm placeholder-gray-400 focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent transition-all resize-none min-h-[120px]"
                                />
                                <div className="absolute bottom-3 right-3 flex items-center gap-2 text-xs text-gray-400">
                                    <FileText className="w-3 h-3" />
                                    <span>{formData.description.length}/500</span>
                                </div>
                            </div>
                            <div className="flex items-start gap-2 p-3 bg-blue-50 rounded-lg border border-blue-100">
                                <Info className="w-4 h-4 text-blue-600 mt-0.5 flex-shrink-0" />
                                <p className="text-xs text-blue-800">
                                    <strong>İpucu:</strong> Kategori açıklaması kullanıcıların ürünleri daha kolay bulmasına yardımcı olur. Net ve açıklayıcı bir dil kullanın.
                                </p>
                            </div>
                        </div>

                        {/* Changes Indicator */}
                        {hasChanges && (
                            <div className="flex items-center gap-3 p-4 bg-amber-50 border border-amber-200 rounded-xl">
                                <div className="w-2 h-2 bg-amber-500 rounded-full animate-pulse"></div>
                                <span className="text-sm font-medium text-amber-800">
                                    Kaydedilmemiş değişiklikler var
                                </span>
                            </div>
                        )}
                    </div>
                )}
            </form>
        </div>
    );
}