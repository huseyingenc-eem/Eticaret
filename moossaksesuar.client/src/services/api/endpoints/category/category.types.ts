// src/services/api/endpoints/category/category.types.ts

export type CategoryListItem = {
    id: number;
    name: string;
    description?: string | null;
    parentId: number | null;
    isActive: boolean;
    childCount: number;
};

export type CategoryDetails = {
    id: number;
    name: string;
    parentId: number | null;
    isActive: boolean;
    description?: string | null;
};
export type CategoryTreeNode = {
    id: number;
    name: string;
    parentId: number | null;
    isActive: boolean;
    children: CategoryTreeNode[];
};
export type AncestorNode = { id: number; name: string };

export type SearchResult = {
    id: number;
    name: string;
    path: AncestorNode[]; // Root->...->Parent
};

export type CreateCategoryRequest = {
    name: string;
    parentId: number | null;
    isActive: boolean;
};

export type UpdateCategoryRequest = {
    name: string;
    isActive?: boolean;
    parentId?: number | null;
    description?: string | null;
    displayOrder?: number;
};


export type Paged<T> = { items: T[]; total: number };
