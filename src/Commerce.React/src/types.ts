export type OrderStatus = 'Pending' | 'Paid' | 'Shipped' | 'Cancelled';

export type CustomerResponse = {
    id: number;
    name: string;
    email: string;
    phone?: string;
    createdAt: string;
};

export type ProductResponse = {
    id: number;
    name: string;
    sku: string;
    price: number;
    stock: number;
};

export type OrderItemResponse = {
    productId: number;
    productName: string;
    quantity: number;
    unitPrice: number;
    lineTotal: number;
};

export type OrderResponse = {
    id: number;
    customerId: number;
    customerName: string;
    orderDate: string;
    status: OrderStatus;
    total: number;
    items: OrderItemResponse[];
};

export type PagedResponse<T> = {
    items: T[];
    page: number;
    pageSize: number;
    totalCount: number;
};
