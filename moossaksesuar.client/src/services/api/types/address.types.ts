export interface MyAddress {
    id: string;
    addressTitle: string;
    country: string;
    city: string;
    district: string;
    addressLine: string;
    isDefaultBilling: boolean;
    isDefaultShipping: boolean;
    createdTime: string;
}

export interface AddressDetail extends MyAddress {
    street: string;
    zipCode?: string;
    updateTime?: string;
}