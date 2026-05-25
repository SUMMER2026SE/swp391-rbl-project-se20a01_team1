import { useEffect, useState } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import { api } from '../../../../shared/api/apiClient';
import { formatVnd } from '../../../../shared/utils/format';
import { Badge, statusTone } from '../../../../shared/ui/Badge';
import { Card } from '../../../../shared/ui/Card';
import { BackLink } from '../../../../shared/ui/BackLink';
import { ApprovalPanel } from '../../../../shared/ui/ApprovalPanel';
import { ErrorAlert, LoadingState } from '../../../../shared/ui/Feedback';
import type { RoomingHouseDetail } from '../../types';

export function RoomingHouseDetailPage() {
  const { id } = useParams();
  const navigate = useNavigate();
  const [house, setHouse] = useState<RoomingHouseDetail | null>(null);
  const [reason, setReason] = useState('');
  const [error, setError] = useState<string | null>(null);
  const [submitting, setSubmitting] = useState(false);

  useEffect(() => {
    if (!id) return;
    api.get<RoomingHouseDetail>(`/api/admin/rooming-houses/${id}`).then(setHouse).catch((e) => setError(String(e)));
  }, [id]);

  const approve = async () => {
    if (!id) return;
    setSubmitting(true);
    setError(null);
    try {
      await api.post(`/api/admin/rooming-houses/${id}/approve`);
      navigate('/admin/rooming-houses');
    } catch (e) {
      setError(String(e));
      setSubmitting(false);
    }
  };

  const reject = async () => {
    if (!id || !reason.trim()) {
      setError('Vui lòng nhập lý do từ chối');
      return;
    }
    setSubmitting(true);
    setError(null);
    try {
      await api.post(`/api/admin/rooming-houses/${id}/reject`, { rejectedReason: reason });
      navigate('/admin/rooming-houses');
    } catch (e) {
      setError(String(e));
      setSubmitting(false);
    }
  };

  if (!house && !error) return <LoadingState label="Đang tải chi tiết khu trọ..." />;

  return (
    <div className="animate-fade-in">
      <BackLink to="/admin/rooming-houses">Quay lại danh sách</BackLink>

      {error && <div className="mb-6"><ErrorAlert message={error} /></div>}

      {house && (
        <div className="grid gap-8 lg:grid-cols-[1fr_340px]">
          <div className="space-y-6">
            <div>
              <div className="flex flex-wrap items-center gap-3">
                <h1 className="text-2xl font-bold text-slate-900">{house.name}</h1>
                <Badge tone={statusTone(house.approvalStatus)}>{house.approvalStatus}</Badge>
              </div>
              <p className="mt-2 text-slate-600">{house.address}</p>
              {house.description && (
                <p className="mt-3 rounded-xl bg-slate-50 px-4 py-3 text-sm text-slate-600 ring-1 ring-slate-100">
                  {house.description}
                </p>
              )}
            </div>

            <Card>
              <h2 className="mb-4 text-sm font-bold uppercase tracking-wide text-slate-500">
                Danh sách phòng ({house.rooms?.length ?? 0})
              </h2>
              {!house.rooms?.length ? (
                <p className="text-sm text-slate-500">Chưa có phòng nào.</p>
              ) : (
                <ul className="divide-y divide-slate-100">
                  {house.rooms.map((r) => (
                    <li key={r.id} className="flex flex-wrap items-center justify-between gap-2 py-3 first:pt-0 last:pb-0">
                      <div>
                        <span className="font-bold text-slate-900">Phòng {r.roomNumber}</span>
                        <span className="mx-2 text-slate-300">·</span>
                        <span className="text-sm text-slate-600">tối đa {r.capacity} người</span>
                      </div>
                      <div className="flex items-center gap-3">
                        <span className="font-semibold text-brand-700">{formatVnd(r.price)}</span>
                        <Badge tone={statusTone(r.status)}>{r.status}</Badge>
                      </div>
                    </li>
                  ))}
                </ul>
              )}
            </Card>
          </div>

          <ApprovalPanel
            reason={reason}
            onReasonChange={setReason}
            onApprove={approve}
            onReject={reject}
            loading={submitting}
          />
        </div>
      )}
    </div>
  );
}
