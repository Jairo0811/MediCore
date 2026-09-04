import { fireEvent, render, screen } from '@testing-library/react';
import { afterEach, beforeEach, describe, expect, it, vi } from 'vitest';
import App from './App';

vi.mock('./api', () => ({
  apiBaseUrl: 'http://localhost:8080',
  authenticate: vi.fn(),
  clearSession: vi.fn(),
  readSession: vi.fn(() => null),
}));

describe('App authentication shell', () => {
  beforeEach(() => {
    localStorage.clear();
    vi.stubGlobal('fetch', vi.fn().mockResolvedValue({ ok: true }));
  });

  afterEach(() => {
    vi.unstubAllGlobals();
  });

  it('exposes an accessible login form and skip link', () => {
    render(<App />);

    expect(screen.getByRole('link', { name: 'Saltar al contenido principal' })).toHaveAttribute('href', '#main-content');
    expect(screen.getByRole('main')).toHaveAttribute('id', 'main-content');
    expect(screen.getByRole('heading', { name: 'Acceso seguro' })).toBeInTheDocument();
    expect(screen.getByLabelText('Correo electrónico')).toHaveAttribute('autocomplete', 'email');
    expect(screen.getByLabelText('Contraseña')).toHaveAttribute('autocomplete', 'current-password');
    expect(screen.getByRole('button', { name: 'Iniciar sesión' })).toBeEnabled();
  });

  it('switches to the initial administrator form without submitting', () => {
    render(<App />);

    fireEvent.click(screen.getByRole('button', { name: 'Configurar primer administrador' }));

    expect(screen.getByRole('heading', { name: 'Administrador inicial' })).toBeInTheDocument();
    expect(screen.getByLabelText('Nombre completo')).toHaveAttribute('autocomplete', 'name');
    expect(screen.getByLabelText('Contraseña')).toHaveAttribute('autocomplete', 'new-password');
  });
});
