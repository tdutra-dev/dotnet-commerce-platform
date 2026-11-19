import { useEffect, useMemo, useState } from 'react';
import { api } from './api';
import type { CustomerResponse, OrderResponse, PagedResponse, ProductResponse } from './types';

type Tab = 'customers' | 'products' | 'orders';

export function App(): JSX.Element {
    const [tab, setTab] = useState<Tab>('customers');
    const [customers, setCustomers] = useState<CustomerResponse[]>([]);
    const [products, setProducts] = useState<ProductResponse[]>([]);
    const [orders, setOrders] = useState<PagedResponse<OrderResponse>>({
        items: [],
        page: 1,
        pageSize: 10,
        totalCount: 0
    });
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState<string | null>(null);

    useEffect(() => {
        void loadCustomers();
    }, []);

    const canNextOrders = useMemo(
        () => orders.page * orders.pageSize < orders.totalCount,
        [orders.page, orders.pageSize, orders.totalCount]
    );

    async function loadCustomers(): Promise<void> {
        setLoading(true);
        setError(null);
        try {
            setCustomers(await api.getCustomers());
            setTab('customers');
        } catch (e) {
            setError((e as Error).message);
        } finally {
            setLoading(false);
        }
    }

    async function loadProducts(): Promise<void> {
        setLoading(true);
        setError(null);
        try {
            setProducts(await api.getProducts());
            setTab('products');
        } catch (e) {
            setError((e as Error).message);
        } finally {
            setLoading(false);
        }
    }

    async function loadOrders(page = 1): Promise<void> {
        setLoading(true);
        setError(null);
        try {
            setOrders(await api.getOrders(page, orders.pageSize));
            setTab('orders');
        } catch (e) {
            setError((e as Error).message);
        } finally {
            setLoading(false);
        }
    }

    return (
        <div className="app-shell">
            <header>
                <h1>Commerce React Frontend</h1>
                <p>Frontend React che consuma la Commerce API.</p>
            </header>

            <nav className="tabs">
                <button className={tab === 'customers' ? 'active' : ''} onClick={() => void loadCustomers()}>
                    Customers
                </button>
                <button className={tab === 'products' ? 'active' : ''} onClick={() => void loadProducts()}>
                    Products
                </button>
                <button className={tab === 'orders' ? 'active' : ''} onClick={() => void loadOrders(orders.page)}>
                    Orders
                </button>
            </nav>

            {loading && <p>Loading...</p>}
            {error && <p className="error">{error}</p>}

            {!loading && !error && tab === 'customers' && (
                <table>
                    <thead>
                        <tr>
                            <th>Id</th>
                            <th>Name</th>
                            <th>Email</th>
                            <th>Phone</th>
                            <th>Created</th>
                        </tr>
                    </thead>
                    <tbody>
                        {customers.map((c) => (
                            <tr key={c.id}>
                                <td>{c.id}</td>
                                <td>{c.name}</td>
                                <td>{c.email}</td>
                                <td>{c.phone ?? '-'}</td>
                                <td>{new Date(c.createdAt).toLocaleString()}</td>
                            </tr>
                        ))}
                    </tbody>
                </table>
            )}

            {!loading && !error && tab === 'products' && (
                <table>
                    <thead>
                        <tr>
                            <th>Id</th>
                            <th>Name</th>
                            <th>Sku</th>
                            <th>Price</th>
                            <th>Stock</th>
                        </tr>
                    </thead>
                    <tbody>
                        {products.map((p) => (
                            <tr key={p.id}>
                                <td>{p.id}</td>
                                <td>{p.name}</td>
                                <td>{p.sku}</td>
                                <td>{p.price.toFixed(2)}</td>
                                <td>{p.stock}</td>
                            </tr>
                        ))}
                    </tbody>
                </table>
            )}

            {!loading && !error && tab === 'orders' && (
                <>
                    <table>
                        <thead>
                            <tr>
                                <th>Id</th>
                                <th>Customer</th>
                                <th>Date</th>
                                <th>Status</th>
                                <th>Total</th>
                                <th>Items</th>
                            </tr>
                        </thead>
                        <tbody>
                            {orders.items.map((o) => (
                                <tr key={o.id}>
                                    <td>{o.id}</td>
                                    <td>{o.customerName}</td>
                                    <td>{new Date(o.orderDate).toLocaleString()}</td>
                                    <td>{o.status}</td>
                                    <td>{o.total.toFixed(2)}</td>
                                    <td>{o.items.length}</td>
                                </tr>
                            ))}
                        </tbody>
                    </table>

                    <div className="pager">
                        <button disabled={orders.page <= 1} onClick={() => void loadOrders(orders.page - 1)}>
                            Prev
                        </button>
                        <span>Page {orders.page}</span>
                        <button disabled={!canNextOrders} onClick={() => void loadOrders(orders.page + 1)}>
                            Next
                        </button>
                        <span>Total {orders.totalCount}</span>
                    </div>
                </>
            )}
        </div>
    );
}
