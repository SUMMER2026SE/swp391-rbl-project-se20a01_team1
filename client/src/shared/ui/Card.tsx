export function Card({
  children,
  className = '',
  hover = false,
}: {
  children: React.ReactNode;
  className?: string;
  hover?: boolean;
}) {
  return (
    <div
      className={`rounded-2xl border border-slate-200/80 bg-white p-5 shadow-[var(--shadow-card)] transition-all duration-300 ${
        hover ? 'hover:border-brand-200 hover:shadow-[var(--shadow-card-hover)] hover:-translate-y-0.5' : ''
      } ${className}`}
    >
      {children}
    </div>
  );
}
