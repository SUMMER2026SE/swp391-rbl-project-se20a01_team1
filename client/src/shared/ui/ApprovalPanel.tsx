import { Button } from './Button';

interface ApprovalPanelProps {
  reason: string;
  onReasonChange: (value: string) => void;
  onApprove: () => void;
  onReject: () => void;
  loading?: boolean;
  rejectHint?: string;
}

export function ApprovalPanel({
  reason,
  onReasonChange,
  onApprove,
  onReject,
  loading,
  rejectHint = 'Lý do từ chối (bắt buộc khi từ chối)',
}: ApprovalPanelProps) {
  return (
    <div className="rounded-2xl border border-slate-200 bg-gradient-to-br from-slate-50 to-white p-6 shadow-sm">
      <h3 className="text-sm font-bold text-slate-900">Quyết định duyệt</h3>
      <p className="mt-1 text-xs text-slate-500">Xác nhận hồ sơ hợp lệ trước khi duyệt.</p>

      <div className="mt-5 flex flex-wrap gap-3">
        <Button variant="success" size="lg" onClick={onApprove} disabled={loading}>
          <CheckIcon />
          Duyệt
        </Button>
      </div>

      <div className="mt-6 border-t border-slate-200 pt-6">
        <label htmlFor="reject-reason" className="block text-xs font-semibold text-slate-600">
          {rejectHint}
        </label>
        <textarea
          id="reject-reason"
          rows={3}
          className="mt-2 w-full resize-none rounded-xl border border-slate-200 bg-white px-4 py-3 text-sm text-slate-900 shadow-sm transition placeholder:text-slate-400 focus:border-rose-300 focus:outline-none focus:ring-2 focus:ring-rose-500/20"
          placeholder="Nhập lý do cụ thể..."
          value={reason}
          onChange={(e) => onReasonChange(e.target.value)}
        />
        <Button variant="danger" className="mt-3" onClick={onReject} disabled={loading}>
          Từ chối
        </Button>
      </div>
    </div>
  );
}

function CheckIcon() {
  return (
    <svg className="h-4 w-4" fill="none" viewBox="0 0 24 24" stroke="currentColor" strokeWidth={2.5}>
      <path strokeLinecap="round" strokeLinejoin="round" d="M5 13l4 4L19 7" />
    </svg>
  );
}
