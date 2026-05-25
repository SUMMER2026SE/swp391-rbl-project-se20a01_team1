/**
 * Tập trung khai báo route — tách khỏi App.tsx khi dự án lớn hơn.
 * Hiện route vẫn khai báo trong app/App.tsx.
 */
export const routePaths = {
  public: {
    listing: '/',
    houseDetail: '/houses/:id',
  },
  admin: {
    kycList: '/admin/kyc',
    kycDetail: '/admin/kyc/:kycId',
    roomingHouseList: '/admin/rooming-houses',
    roomingHouseDetail: '/admin/rooming-houses/:id',
  },
} as const;
