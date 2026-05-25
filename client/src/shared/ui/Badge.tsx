type Tone = 'default' | 'success' | 'warning' | 'danger' | 'info';

const tones: Record<Tone, string> = {
  default: 'bg-slate-100 text-slate-700 ring-slate-200',
  success: 'bg-emerald-50 text-emerald-800 ring-emerald-200',
  warning: 'bg-amber-50 text-amber-800 ring-amber-200',
  danger: 'bg-rose-50 text-rose-800 ring-rose-200',
  info: 'bg-sky-50 text-sky-800 ring-sky-200',
};

export function Badge({ children, tone = 'default' }: { children: React.ReactNode; tone?: Tone }) {
  return (
    <span
      className={`inline-flex items-center rounded-full px-2.5 py-0.5 text-xs font-semibold ring-1 ring-inset ${tones[tone]}`}
    >
      {children}
    </span>
  );
}

export function statusTone(status: string): Tone {
  const s = status.toLowerCase();
  if (s.includes('approv') || s.includes('active') || s.includes('available') || s.includes('completed'))
    return 'success';
  if (s.includes('pend') || s.includes('review') || s.includes('draft')) return 'warning';
  if (s.includes('reject') || s.includes('ban') || s.includes('lock')) return 'danger';
  return 'default';
}
