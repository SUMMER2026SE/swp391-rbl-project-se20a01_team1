export function LoadingState({ label = 'Đang tải dữ liệu...' }: { label?: string }) {
  return (
    <div className="flex flex-col items-center justify-center gap-4 py-24 animate-fade-in">
      <div className="h-10 w-10 rounded-full border-[3px] border-brand-200 border-t-brand-600 animate-spin" />
      <p className="text-sm font-medium text-slate-500">{label}</p>
    </div>
  );
}

export function ErrorAlert({ message }: { message: string }) {
  return (
    <div
      role="alert"
      className="rounded-2xl border border-rose-200 bg-rose-50 px-4 py-3 text-sm text-rose-800 animate-fade-in"
    >
      <p className="font-semibold">Đã xảy ra lỗi</p>
      <p className="mt-1 text-rose-700/90 break-words">{message}</p>
    </div>
  );
}

export function EmptyState({ title, description }: { title: string; description?: string }) {
  return (
    <CardEmpty>
      <div className="mx-auto mb-4 flex h-14 w-14 items-center justify-center rounded-2xl bg-slate-100 text-2xl">
        📭
      </div>
      <p className="font-semibold text-slate-800">{title}</p>
      {description && <p className="mt-1 text-sm text-slate-500">{description}</p>}
    </CardEmpty>
  );
}

function CardEmpty({ children }: { children: React.ReactNode }) {
  return (
    <div className="rounded-2xl border border-dashed border-slate-200 bg-white/60 px-6 py-16 text-center animate-fade-in">
      {children}
    </div>
  );
}
