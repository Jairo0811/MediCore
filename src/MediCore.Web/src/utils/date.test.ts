import { describe, expect, it } from 'vitest';
import { toUtc } from './date';

describe('date utilities', () => {
  it('normalizes an offset date-time to UTC', () => {
    expect(toUtc('2026-09-04T11:21:00-04:00')).toBe('2026-09-04T15:21:00.000Z');
  });
});
