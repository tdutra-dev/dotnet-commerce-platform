import type { CustomerResponse, PagedResponse, OrderResponse, ProductResponse } from './types';

const API_BASE_URL = import.meta.env.VITE_API_BASE_URL ?? 'http://localhost:8080';

async function getJson<T>(path: string): Promise<T> {
    const response = await fetch(`${API_BASE_URL}${path}`);
    if (!response.ok) {
        throw new Error(`HTTP ${response.status} - ${response.statusText}`);
    }
    return (await response.json()) as T;
}

export const api = {
    getCustomers: () => getJson<CustomerResponse[]>('/api/customers'),
    getProducts: () => getJson<ProductResponse[]>('/api/products'),
    getOrders: (page = 1, pageSize = 10) =>
        getJson<PagedResponse<OrderResponse>>(`/api/orders?page=${page}&pageSize=${pageSize}`)
};
