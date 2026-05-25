export function PageHeader({
  title,
  description,
  badge,
  action,
}: {
  title: string;
  description?: string;
  badge?: React.ReactNode;
  action?: React.ReactNode;
}) {
  return (
    <div className="mb-8 flex flex-col gap-4 sm:flex-row sm:items-start sm:justify-between animate-fade-in">
      <div>
        {badge && <div className="mb-2">{badge}</div>}
        <h1 className="text-2xl font-bold tracking-tight text-slate-900 sm:text-3xl">{title}</h1>
        {description && <p className="mt-2 max-w-2xl text-sm leading-relaxed text-slate-600">{description}</p>}
      </div>
      {action && <div className="shrink-0">{action}</div>}
    </div>
  );
}
