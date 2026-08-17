export function formatDate(value: string): string {
  return new Intl.DateTimeFormat('es-DO', {
    dateStyle: 'medium',
    timeStyle: 'short',
  }).format(new Date(value));
}

export function toUtc(localValue: string): string {
  return new Date(localValue).toISOString();
}
