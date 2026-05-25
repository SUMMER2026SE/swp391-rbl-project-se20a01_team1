/** Admin — duyệt khu trọ (Người 5) */
export interface RoomingHouseListResponse {
  items: RoomingHouseListItem[];
  totalCount: number;
  pageSize: number;
  pageNumber: number;
}

export interface RoomingHouseListItem {
  id: string;
  name: string;
  address: string;
  landlordEmail?: string;
  approvalStatus: string;
  createdAt: string;
}

export interface RoomingHouseDetail {
  id: string;
  name: string;
  address: string;
  description?: string;
  approvalStatus: string;
  rooms: RoomInfo[];
}

export interface RoomInfo {
  id: string;
  roomNumber: string;
  price: number;
  capacity: number;
  status: string;
}
