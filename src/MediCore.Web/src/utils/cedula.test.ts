import { describe, expect, it } from 'vitest';
import { formatDominicanCedula, normalizeDominicanCedula, passesDominicanCedulaLuhn } from './cedula';

describe('Dominican cedula helpers', () => {
  it('normalizes and formats identifiers', () => {
    expect(normalizeDominicanCedula('001-1391824-5')).toBe('00113918245');
    expect(formatDominicanCedula('00113918245')).toBe('001-1391824-5');
  });

  it('rejects incomplete identifiers', () => {
    expect(passesDominicanCedulaLuhn('001-1391824')).toBe(false);
  });

  it('evaluates the Luhn checksum deterministically', () => {
    expect(passesDominicanCedulaLuhn('00113918245')).toBe(true);
    expect(passesDominicanCedulaLuhn('00113918244')).toBe(false);
  });
});
