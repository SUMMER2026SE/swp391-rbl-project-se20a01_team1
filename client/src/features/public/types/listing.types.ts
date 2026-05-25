/** Public — danh sách khu trọ công khai (Người 5) */
export interface PublicRoomingHouse {
  id: string;
  name: string;
  address: string;
  description?: string;
  imageUrls?: string[];
  availableRoomCount: number;
  minRoomPrice: number;
  maxRoomPrice: number;
  priceFromLabel: string;
}

export interface PublicRoomingHouseDetail {
  id: string;
  name: string;
  address: string;
  description?: string;
  amenities?: string[];
  imageUrls: string[];
  availableRooms: PublicRoom[];
  createdAt: string;
}

export interface PublicRoom {
  id: string;
  roomNumber: string;
  price: number;
  area: number;
  capacity: number;
  priceFromLabel: string;
  priceTiers: { occupantCount: number; monthlyPrice: number; label: string }[];
  imageUrls?: string[];
}
