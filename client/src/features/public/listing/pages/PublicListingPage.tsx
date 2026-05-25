import { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import { api } from '../../../../shared/api/apiClient';
import { Badge } from '../../../../shared/ui/Badge';
import { Card } from '../../../../shared/ui/Card';
import { EmptyState, ErrorAlert, LoadingState } from '../../../../shared/ui/Feedback';
import { PageHeader } from '../../../../shared/ui/PageHeader';
import type { PublicRoomingHouse } from '../../types/listing.types';

export function PublicListingPage() {
  const [list, setList] = useState<PublicRoomingHouse[] | null>(null);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    api
      .get<PublicRoomingHouse[]>('/api/public/rooming-houses?pageNumber=1&pageSize=20')
      .then(setList)
      .catch((e) => setError(String(e)));
  }, []);

  if (error) {
    return (
      <div className="animate-fade-in">
        <PageHeader title="Tìm khu trọ" description="Danh sách khu trọ đã duyệt, hiển thị công khai." />
        <ErrorAlert message={error} />
      </div>
    );
  }

  if (!list) return <LoadingState label="Đang tải danh sách khu trọ..." />;

  return (
    <div className="animate-fade-in">
      <PageHeader
        title="Tìm khu trọ"
        description="Chỉ hiển thị khu trọ đã duyệt (Approved), đang hiển thị (Visible) và còn phòng trống (Available)."
        badge={<Badge tone="info">{list.length} khu trọ</Badge>}
      />

      {list.length === 0 ? (
        <EmptyState
          title="Chưa có khu trọ công khai"
          description="Khi admin duyệt khu trọ, danh sách sẽ xuất hiện tại đây."
        />
      ) : (
        <ul className="grid gap-6 sm:grid-cols-2 lg:grid-cols-3">
          {list.map((h) => (
            <li key={h.id}>
              <Link to={`/houses/${h.id}`} className="group block h-full">
                <Card hover className="flex h-full flex-col overflow-hidden p-0">
                  <div className="relative aspect-[16/10] bg-gradient-to-br from-slate-100 to-slate-200">
                    {h.imageUrls?.[0] ? (
                      <img
                        src={h.imageUrls[0]}
                        alt={h.name}
                        className="h-full w-full object-cover"
                      />
                    ) : (
                      <div className="flex h-full items-center justify-center text-4xl opacity-40">🏠</div>
                    )}
                    {h.availableRoomCount > 0 && (
                      <span className="absolute right-3 top-3 rounded-full bg-white/95 px-2.5 py-1 text-xs font-bold text-emerald-700 shadow-sm ring-1 ring-emerald-100">
                        {h.availableRoomCount} phòng trống
                      </span>
                    )}
                  </div>
                  <div className="flex flex-1 flex-col p-5">
                    <h2 className="font-bold text-slate-900 line-clamp-1">{h.name}</h2>
                    <p className="mt-1 flex items-start gap-1.5 text-sm text-slate-500 line-clamp-2">
                      <LocationIcon />
                      {h.address}
                    </p>
                    <p className="mt-4 text-lg font-bold text-brand-700">
                      {h.priceFromLabel || `Từ ${h.minRoomPrice.toLocaleString('vi-VN')}đ`}
                    </p>
                    <span className="mt-4 inline-flex items-center gap-1 text-sm font-semibold text-brand-600">
                      Xem chi tiết
                      <ArrowIcon />
                    </span>
                  </div>
                </Card>
              </Link>
            </li>
          ))}
        </ul>
      )}
    </div>
  );
}

function LocationIcon() {
  return (
    <svg className="mt-0.5 h-4 w-4 shrink-0 text-slate-400" fill="none" viewBox="0 0 24 24" stroke="currentColor" strokeWidth={2}>
      <path strokeLinecap="round" strokeLinejoin="round" d="M17.657 16.657L13.414 20.9a1.998 1.998 0 01-2.827 0l-4.244-4.243a8 8 0 1111.314 0z" />
      <path strokeLinecap="round" strokeLinejoin="round" d="M15 11a3 3 0 11-6 0 3 3 0 016 0z" />
    </svg>
  );
}

function ArrowIcon() {
  return (
    <svg className="h-4 w-4 transition group-hover:translate-x-0.5" fill="none" viewBox="0 0 24 24" stroke="currentColor" strokeWidth={2}>
      <path strokeLinecap="round" strokeLinejoin="round" d="M9 5l7 7-7 7" />
    </svg>
  );
}
