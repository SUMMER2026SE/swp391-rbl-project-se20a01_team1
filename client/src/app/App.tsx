import { BrowserRouter, Route, Routes } from 'react-router-dom';
import { AppShell } from '../shared/layout/AppShell';
import { KycPendingListPage } from '../features/admin/kyc/pages/KycPendingListPage';
import { KycDetailPage } from '../features/admin/kyc/pages/KycDetailPage';
import { RoomingHousePendingListPage } from '../features/admin/rooming-house/pages/RoomingHousePendingListPage';
import { RoomingHouseDetailPage } from '../features/admin/rooming-house/pages/RoomingHouseDetailPage';
import { PublicListingPage } from '../features/public/listing/pages/PublicListingPage';
import { PublicHouseDetailPage } from '../features/public/listing/pages/PublicHouseDetailPage';

export default function App() {
  return (
    <BrowserRouter>
      <Routes>
        <Route element={<AppShell />}>
          <Route path="/" element={<PublicListingPage />} />
          <Route path="/houses/:id" element={<PublicHouseDetailPage />} />
          <Route path="/admin/kyc" element={<KycPendingListPage />} />
          <Route path="/admin/kyc/:kycId" element={<KycDetailPage />} />
          <Route path="/admin/rooming-houses" element={<RoomingHousePendingListPage />} />
          <Route path="/admin/rooming-houses/:id" element={<RoomingHouseDetailPage />} />
        </Route>
      </Routes>
    </BrowserRouter>
  );
}
