import { describe, expect, it } from 'vitest';
import { formatDominicanCedula, normalizeDominicanCedula, passesDominicanCedulaLuhn } from './cedula';

describe('Dominican cedula helpers', () => {
  it('normalizes and formats identifiers', () => {
    expect(normalizeDominicanCedula('123-4567890-3')).toBe('12345678903');
    expect(formatDominicanCedula('12345678903')).toBe('123-4567890-3');
  });

  it('rejects incomplete identifiers', () => {
    expect(passesDominicanCedulaLuhn('123-4567890')).toBe(false);
  });

  it('evaluates the Luhn checksum deterministically', () => {
    expect(passesDominicanCedulaLuhn('12345678903')).toBe(true);
    expect(passesDominicanCedulaLuhn('12345678904')).toBe(false);
  });
});
