/** Admin — duyệt KYC (Người 5) */
export interface KYCListResponse {
  items: KYCListItem[];
  totalCount: number;
  pageSize: number;
  pageNumber: number;
}

export interface KYCListItem {
  id: string;
  userId: string;
  userEmail?: string;
  userDisplayName?: string;
  fullName?: string;
  status: string;
  createdAt: string;
}

export interface KYCDetail {
  id: string;
  userId: string;
  userEmail?: string;
  userDisplayName?: string;
  idImageUrl?: string;
  faceImageUrl?: string;
  fullName?: string;
  dateOfBirth?: string;
  idNumber?: string;
  address?: string;
  status: string;
  createdAt: string;
}
