// src/hooks/useAddress.ts
import { useState, useEffect } from 'react';
import { addressApi, type Address } from '@/services/api/endpoints/addressApi';

export const useAddress = () => {
    const [addresses, setAddresses] = useState<Address[]>([]);
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState<string | null>(null);

    const fetchAddresses = async () => {
        try {
            setLoading(true);
            setError(null);
            const data = await addressApi.getMyAddresses();
            setAddresses(data);
        } catch (err) {
            setError('Adresler yüklenemedi');
            throw err;
        } finally {
            setLoading(false);
        }
    };

    const addAddress = async (addressData: Omit<Address, 'id' | 'createdTime'>) => {
        try {
            setLoading(true);
            const newAddress = await addressApi.createAddress(addressData);
            setAddresses(prev => [...prev, newAddress]);
            return newAddress;
        } catch (error) {
            throw error;
        } finally {
            setLoading(false);
        }
    };

    const updateAddress = async (id: string, addressData: Partial<Address>) => {
        try {
            setLoading(true);
            const updatedAddress = await addressApi.updateAddress(id, addressData);
            setAddresses(prev =>
                prev.map(addr => addr.id === id ? updatedAddress : addr)
            );
            return updatedAddress;
        } catch (error) {
            throw error;
        } finally {
            setLoading(false);
        }
    };

    const deleteAddress = async (id: string) => {
        try {
            setLoading(true);
            await addressApi.deleteAddress(id);
            setAddresses(prev => prev.filter(addr => addr.id !== id));
        } catch (error) {
            throw error;
        } finally {
            setLoading(false);
        }
    };

    useEffect(() => {
        if (localStorage.getItem('token')) {
            fetchAddresses();
        }
    }, []);

    return {
        addresses,
        loading,
        error,
        fetchAddresses,
        addAddress,
        updateAddress,
        deleteAddress
    };
};