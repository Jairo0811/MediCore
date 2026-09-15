export function normalizeDominicanCedula(value: string): string {
  return value.replace(/\D/g, '').slice(0, 11);
}

export function formatDominicanCedula(value: string): string {
  const digits = normalizeDominicanCedula(value);
  if (digits.length <= 3) return digits;
  if (digits.length <= 10) return `${digits.slice(0, 3)}-${digits.slice(3)}`;
  return `${digits.slice(0, 3)}-${digits.slice(3, 10)}-${digits.slice(10)}`;
}

/**
 * Fast client-side prevalidation using the same Luhn pattern referenced by
 * OGTIC Cuenta Única Registry. The backend remains authoritative because it
 * also supports SHA-256 exception hashes for legitimate legacy identifiers.
 */
export function passesDominicanCedulaLuhn(value: string): boolean {
  const cedula = normalizeDominicanCedula(value);
  if (cedula.length !== 11) return false;

  const digits = cedula.split('').reverse().map(Number);
  const checkDigit = digits.shift();
  if (checkDigit === undefined) return false;

  const sum = digits.reduce((total, digit, index) => {
    if (index % 2 !== 0) return total + digit;
    const doubled = digit * 2;
    return total + (doubled > 9 ? doubled - 9 : doubled);
  }, checkDigit);

  return sum % 10 === 0;
}
