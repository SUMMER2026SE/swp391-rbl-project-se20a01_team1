import { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import { api } from '../../../../shared/api/apiClient';
import { formatDate } from '../../../../shared/utils/format';
import { Badge, statusTone } from '../../../../shared/ui/Badge';
import { Card } from '../../../../shared/ui/Card';
import { EmptyState, ErrorAlert, LoadingState } from '../../../../shared/ui/Feedback';
import { PageHeader } from '../../../../shared/ui/PageHeader';
import type { KYCListResponse } from '../../types';

export function KycPendingListPage() {
  const [data, setData] = useState<KYCListResponse | null>(null);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    api
      .get<KYCListResponse>('/api/admin/kyc/pending?pageNumber=1&pageSize=20')
      .then(setData)
      .catch((e) => setError(String(e)));
  }, []);

  if (error) {
    return (
      <div className="animate-fade-in">
        <PageHeader title="KYC chờ duyệt" description="Hồ sơ xác minh danh tính đang chờ admin xử lý." />
        <ErrorAlert message={error} />
      </div>
    );
  }

  if (!data) return <LoadingState label="Đang tải danh sách KYC..." />;

  return (
    <div className="animate-fade-in">
      <PageHeader
        title="KYC chờ duyệt"
        description="Xem và duyệt hồ sơ KYC của người dùng (Tenant / Landlord)."
        badge={<Badge tone="warning">{data.totalCount} hồ sơ</Badge>}
      />

      {data.items.length === 0 ? (
        <EmptyState title="Không có KYC chờ duyệt" description="Tất cả hồ sơ đã được xử lý." />
      ) : (
        <ul className="space-y-3">
          {data.items.map((k) => (
            <li key={k.id}>
              <Link to={`/admin/kyc/${k.id}`} className="block">
                <Card hover className="flex flex-col gap-3 sm:flex-row sm:items-center sm:justify-between">
                  <div className="flex items-start gap-4">
                    <div className="flex h-12 w-12 shrink-0 items-center justify-center rounded-xl bg-amber-50 text-lg font-bold text-amber-700 ring-1 ring-amber-100">
                      {(k.fullName ?? k.userEmail ?? '?').charAt(0).toUpperCase()}
                    </div>
                    <div>
                      <p className="font-bold text-slate-900">
                        {k.fullName ?? k.userDisplayName ?? 'Chưa có tên'}
                      </p>
                      <p className="text-sm text-slate-500">{k.userEmail}</p>
                      <p className="mt-1 text-xs text-slate-400">Gửi lúc {formatDate(k.createdAt)}</p>
                    </div>
                  </div>
                  <div className="flex items-center gap-3 sm:flex-col sm:items-end">
                    <Badge tone={statusTone(k.status)}>{k.status}</Badge>
                    <span className="text-sm font-semibold text-brand-600">Xem chi tiết →</span>
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
