import { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import { api } from '../../../../shared/api/apiClient';
import { formatDate } from '../../../../shared/utils/format';
import { Badge, statusTone } from '../../../../shared/ui/Badge';
import { Card } from '../../../../shared/ui/Card';
import { EmptyState, ErrorAlert, LoadingState } from '../../../../shared/ui/Feedback';
import { PageHeader } from '../../../../shared/ui/PageHeader';
import type { RoomingHouseListResponse } from '../../types';

export function RoomingHousePendingListPage() {
  const [data, setData] = useState<RoomingHouseListResponse | null>(null);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    api
      .get<RoomingHouseListResponse>('/api/admin/rooming-houses/pending?pageNumber=1&pageSize=20')
      .then(setData)
      .catch((e) => setError(String(e)));
  }, []);

  if (error) {
    return (
      <div className="animate-fade-in">
        <PageHeader title="Khu trọ chờ duyệt" description="Khu trọ mới đăng ký chờ admin phê duyệt." />
        <ErrorAlert message={error} />
      </div>
    );
  }

  if (!data) return <LoadingState label="Đang tải danh sách khu trọ..." />;

  return (
    <div className="animate-fade-in">
      <PageHeader
        title="Khu trọ chờ duyệt"
        description="Duyệt khu trọ mới — khi duyệt lần đầu, chủ trọ có thể được cấp role Landlord."
        badge={<Badge tone="warning">{data.totalCount} khu trọ</Badge>}
      />

      {data.items.length === 0 ? (
        <EmptyState title="Không có khu trọ chờ duyệt" description="Tất cả hồ sơ đã được xử lý." />
      ) : (
        <ul className="space-y-3">
          {data.items.map((h) => (
            <li key={h.id}>
              <Link to={`/admin/rooming-houses/${h.id}`} className="block">
                <Card hover className="flex flex-col gap-3 sm:flex-row sm:items-center sm:justify-between">
                  <div className="flex items-start gap-4">
                    <div className="flex h-12 w-12 shrink-0 items-center justify-center rounded-xl bg-indigo-50 text-xl ring-1 ring-indigo-100">
                      🏢
                    </div>
                    <div>
                      <p className="font-bold text-slate-900">{h.name}</p>
                      <p className="text-sm text-slate-500">{h.address}</p>
                      {h.landlordEmail && (
                        <p className="mt-1 text-xs text-slate-400">Chủ trọ: {h.landlordEmail}</p>
                      )}
                      <p className="mt-1 text-xs text-slate-400">Đăng {formatDate(h.createdAt)}</p>
                    </div>
                  </div>
                  <div className="flex items-center gap-3 sm:flex-col sm:items-end">
                    <Badge tone={statusTone(h.approvalStatus)}>{h.approvalStatus}</Badge>
                    <span className="text-sm font-semibold text-brand-600">Xem & duyệt →</span>
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
