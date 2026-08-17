import type { AuthResponse } from './types';

export const apiBaseUrl = import.meta.env.VITE_API_BASE_URL ?? 'http://localhost:8080';
const sessionKey = 'medicore.session';

export type ApiError = Error & {
  status?: number;
  code?: string;
};

export function readSession(): AuthResponse | null {
  const raw = localStorage.getItem(sessionKey);
  if (!raw) return null;

  try {
    return JSON.parse(raw) as AuthResponse;
  } catch {
    localStorage.removeItem(sessionKey);
    return null;
  }
}

export function saveSession(session: AuthResponse): void {
  localStorage.setItem(sessionKey, JSON.stringify(session));
}

export function clearSession(): void {
  localStorage.removeItem(sessionKey);
}

async function parseError(response: Response): Promise<ApiError> {
  let message = `Solicitud rechazada (${response.status}).`;
  let code: string | undefined;

  try {
    const body = (await response.json()) as { message?: string; error?: string };
    message = body.message ?? message;
    code = body.error;
  } catch {
    // Responses without JSON keep the HTTP fallback message.
  }

  const error = new Error(message) as ApiError;
  error.status = response.status;
  error.code = code;
  return error;
}

async function refreshSession(): Promise<AuthResponse | null> {
  const current = readSession();
  if (!current?.refreshToken) return null;

  const response = await fetch(`${apiBaseUrl}/api/auth/refresh`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ refreshToken: current.refreshToken }),
  });

  if (!response.ok) {
    clearSession();
    return null;
  }

  const renewed = (await response.json()) as AuthResponse;
  saveSession(renewed);
  return renewed;
}

export async function apiRequest<T>(
  path: string,
  init: RequestInit = {},
  retryOnUnauthorized = true,
): Promise<T> {
  const session = readSession();
  const headers = new Headers(init.headers);
  headers.set('Content-Type', 'application/json');
  if (session?.accessToken) headers.set('Authorization', `Bearer ${session.accessToken}`);

  const response = await fetch(`${apiBaseUrl}${path}`, { ...init, headers });

  if (response.status === 401 && retryOnUnauthorized && session?.refreshToken) {
    const renewed = await refreshSession();
    if (renewed) return apiRequest<T>(path, init, false);
  }

  if (!response.ok) throw await parseError(response);
  if (response.status === 204) return undefined as T;
  return (await response.json()) as T;
}

export async function authenticate(
  mode: 'login' | 'bootstrap',
  email: string,
  password: string,
  fullName: string,
): Promise<AuthResponse> {
  const path = mode === 'login' ? '/api/auth/login' : '/api/auth/bootstrap-admin';
  const body = mode === 'login' ? { email, password } : { email, password, fullName };
  const session = await apiRequest<AuthResponse>(path, {
    method: 'POST',
    body: JSON.stringify(body),
  });
  saveSession(session);
  return session;
}
