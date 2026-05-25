export function DetailGrid({ children }: { children: React.ReactNode }) {
  return (
    <dl className="grid gap-4 sm:grid-cols-2">{children}</dl>
  );
}

export function DetailItem({
  label,
  value,
  fullWidth,
}: {
  label: string;
  value?: React.ReactNode;
  fullWidth?: boolean;
}) {
  return (
    <div className={`rounded-xl bg-slate-50/80 px-4 py-3 ring-1 ring-slate-100 ${fullWidth ? 'sm:col-span-2' : ''}`}>
      <dt className="text-xs font-semibold uppercase tracking-wide text-slate-500">{label}</dt>
      <dd className="mt-1 text-sm font-medium text-slate-900">{value ?? '—'}</dd>
    </div>
  );
}
