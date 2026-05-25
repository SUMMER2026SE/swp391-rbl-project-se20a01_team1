import { useEffect, useState } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import { api } from '../../../../shared/api/apiClient';
import { Badge, statusTone } from '../../../../shared/ui/Badge';
import { Card } from '../../../../shared/ui/Card';
import { BackLink } from '../../../../shared/ui/BackLink';
import { ApprovalPanel } from '../../../../shared/ui/ApprovalPanel';
import { DetailGrid, DetailItem } from '../../../../shared/ui/DetailGrid';
import { ErrorAlert, LoadingState } from '../../../../shared/ui/Feedback';
import type { KYCDetail } from '../../types';

export function KycDetailPage() {
  const { kycId } = useParams();
  const navigate = useNavigate();
  const [kyc, setKyc] = useState<KYCDetail | null>(null);
  const [reason, setReason] = useState('');
  const [error, setError] = useState<string | null>(null);
  const [submitting, setSubmitting] = useState(false);

  useEffect(() => {
    if (!kycId) return;
    api.get<KYCDetail>(`/api/admin/kyc/${kycId}`).then(setKyc).catch((e) => setError(String(e)));
  }, [kycId]);

  const approve = async () => {
    if (!kycId) return;
    setSubmitting(true);
    setError(null);
    try {
      await api.post(`/api/admin/kyc/${kycId}/approve`);
      navigate('/admin/kyc');
    } catch (e) {
      setError(String(e));
      setSubmitting(false);
    }
  };

  const reject = async () => {
    if (!kycId || !reason.trim()) {
      setError('Vui lòng nhập lý do từ chối');
      return;
    }
    setSubmitting(true);
    setError(null);
    try {
      await api.post(`/api/admin/kyc/${kycId}/reject`, { rejectedReason: reason });
      navigate('/admin/kyc');
    } catch (e) {
      setError(String(e));
      setSubmitting(false);
    }
  };

  if (!kyc && !error) return <LoadingState label="Đang tải chi tiết KYC..." />;

  return (
    <div className="animate-fade-in">
      <BackLink to="/admin/kyc">Quay lại danh sách KYC</BackLink>

      {error && <div className="mb-6"><ErrorAlert message={error} /></div>}

      {kyc && (
        <div className="grid gap-8 lg:grid-cols-[1fr_340px]">
          <div className="space-y-6">
            <div className="flex flex-wrap items-center gap-3">
              <h1 className="text-2xl font-bold text-slate-900">Chi tiết KYC</h1>
              <Badge tone={statusTone(kyc.status)}>{kyc.status}</Badge>
            </div>

            <Card>
              <h2 className="mb-4 text-sm font-bold uppercase tracking-wide text-slate-500">
                Thông tin người nộp
              </h2>
              <DetailGrid>
                <DetailItem label="Họ tên" value={kyc.fullName} />
                <DetailItem label="CCCD / CMND" value={kyc.idNumber} />
                <DetailItem label="Email" value={kyc.userEmail} />
                <DetailItem label="Tên hiển thị" value={kyc.userDisplayName} />
                <DetailItem label="Ngày sinh" value={kyc.dateOfBirth} />
                <DetailItem label="Địa chỉ" value={kyc.address} fullWidth />
              </DetailGrid>
            </Card>

            {(kyc.idImageUrl || kyc.faceImageUrl) && (
              <Card>
                <h2 className="mb-4 text-sm font-bold uppercase tracking-wide text-slate-500">
                  Ảnh đính kèm (Signed URL)
                </h2>
                <div className="grid gap-4 sm:grid-cols-2">
                  {kyc.idImageUrl && (
                    <DocumentLink label="Ảnh giấy tờ (CCCD)" href={kyc.idImageUrl} />
                  )}
                  {kyc.faceImageUrl && (
                    <DocumentLink label="Ảnh khuôn mặt" href={kyc.faceImageUrl} />
                  )}
                </div>
              </Card>
            )}
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

function DocumentLink({ label, href }: { label: string; href: string }) {
  return (
    <a
      href={href}
      target="_blank"
      rel="noreferrer"
      className="flex items-center gap-3 rounded-xl border border-slate-200 bg-slate-50 px-4 py-4 text-sm font-semibold text-brand-700 transition hover:border-brand-300 hover:bg-brand-50"
    >
      <span className="flex h-10 w-10 items-center justify-center rounded-lg bg-white text-lg shadow-sm">📄</span>
      {label}
      <span className="ml-auto text-xs text-slate-400">Mở tab mới</span>
    </a>
  );
}
