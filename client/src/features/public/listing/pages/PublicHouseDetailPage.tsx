import { useEffect, useState } from 'react';
import { useParams } from 'react-router-dom';
import { api } from '../../../../shared/api/apiClient';
import { formatVnd } from '../../../../shared/utils/format';
import { Badge } from '../../../../shared/ui/Badge';
import { Card } from '../../../../shared/ui/Card';
import { BackLink } from '../../../../shared/ui/BackLink';
import { EmptyState, ErrorAlert, LoadingState } from '../../../../shared/ui/Feedback';
import type { PublicRoomingHouseDetail } from '../../types/listing.types';

export function PublicHouseDetailPage() {
  const { id } = useParams<{ id: string }>();
  const [detail, setDetail] = useState<PublicRoomingHouseDetail | null>(null);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    if (!id) return;
    api
      .get<PublicRoomingHouseDetail>(`/api/public/rooming-houses/${id}`)
      .then(setDetail)
      .catch((e) => setError(String(e)));
  }, [id]);

  if (error) {
    return (
      <div className="animate-fade-in">
        <BackLink to="/">Quay lại danh sách</BackLink>
        <ErrorAlert message={error} />
      </div>
    );
  }

  if (!detail) return <LoadingState label="Đang tải chi tiết khu trọ..." />;

  return (
    <div className="animate-fade-in space-y-8">
      <BackLink to="/">Quay lại danh sách</BackLink>

      <header className="space-y-3">
        <h1 className="text-3xl font-bold tracking-tight text-slate-900">{detail.name}</h1>
        <p className="flex items-center gap-2 text-slate-600">
          <span className="text-brand-600">📍</span>
          {detail.address}
        </p>
        {detail.description && (
          <p className="max-w-3xl text-sm leading-relaxed text-slate-600">{detail.description}</p>
        )}
      </header>

      {detail.imageUrls.length > 0 && (
        <section>
          <h2 className="mb-3 text-sm font-bold uppercase tracking-wide text-slate-500">Hình ảnh</h2>
          <div className="flex gap-3 overflow-x-auto pb-2 snap-x">
            {detail.imageUrls.map((url) => (
              <img
                key={url}
                src={url}
                alt=""
                className="h-48 w-72 shrink-0 snap-start rounded-2xl object-cover bg-slate-100 shadow-md ring-1 ring-slate-200/80 sm:h-56 sm:w-80"
              />
            ))}
          </div>
        </section>
      )}

      {detail.amenities && detail.amenities.length > 0 && (
        <section>
          <h2 className="mb-3 text-sm font-bold uppercase tracking-wide text-slate-500">Tiện nghi</h2>
          <ul className="flex flex-wrap gap-2">
            {detail.amenities.map((a) => (
              <li
                key={a}
                className="rounded-full bg-white px-4 py-2 text-sm font-medium text-slate-700 shadow-sm ring-1 ring-slate-200"
              >
                {a}
              </li>
            ))}
          </ul>
        </section>
      )}

      <section>
        <div className="mb-4 flex flex-wrap items-end justify-between gap-2">
          <div>
            <h2 className="text-lg font-bold text-slate-900">Phòng còn trống</h2>
            <p className="text-xs text-slate-500">Chỉ hiển thị phòng Available — không gồm Maintenance / Occupied.</p>
          </div>
          <Badge tone="success">{detail.availableRooms.length} phòng</Badge>
        </div>

        {detail.availableRooms.length === 0 ? (
          <EmptyState title="Hiện không có phòng trống" description="Vui lòng quay lại sau hoặc xem khu trọ khác." />
        ) : (
          <ul className="grid gap-4 lg:grid-cols-2">
            {detail.availableRooms.map((r) => (
              <li key={r.id}>
                <Card className="overflow-hidden p-0">
                  <div className="flex flex-col sm:flex-row">
                    {r.imageUrls?.[0] ? (
                      <img
                        src={r.imageUrls[0]}
                        alt={r.roomNumber}
                        className="h-40 w-full object-cover bg-slate-100 sm:h-auto sm:w-40 shrink-0"
                      />
                    ) : (
                      <div className="flex h-32 w-full items-center justify-center bg-slate-100 text-3xl sm:w-36 sm:h-auto">
                        🛏️
                      </div>
                    )}
                    <div className="flex flex-1 flex-col p-5">
                      <div className="flex items-start justify-between gap-2">
                        <h3 className="text-xl font-bold text-slate-900">Phòng {r.roomNumber}</h3>
                        <Badge tone="success">Available</Badge>
                      </div>
                      <p className="mt-1 text-sm text-slate-500">
                        {r.area} m² · tối đa {r.capacity} người
                      </p>
                      <p className="mt-3 text-lg font-bold text-brand-700">
                        {r.priceFromLabel || formatVnd(r.price)}
                      </p>
                      {r.priceTiers.length > 0 && (
                        <ul className="mt-3 space-y-1 border-t border-slate-100 pt-3">
                          {r.priceTiers.map((t) => (
                            <li key={t.occupantCount} className="flex justify-between text-sm text-slate-600">
                              <span>{t.label}</span>
                              <span className="font-medium text-slate-800">{formatVnd(t.monthlyPrice)}</span>
                            </li>
                          ))}
                        </ul>
                      )}
                    </div>
                  </div>
                </Card>
              </li>
            ))}
          </ul>
        )}
      </section>
    </div>
  );
}
