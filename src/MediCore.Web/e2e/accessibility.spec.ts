import AxeBuilder from '@axe-core/playwright';
import { expect, test } from '@playwright/test';

test.beforeEach(async ({ page }) => {
  await page.route('**/api/health/live', async (route) => {
    await route.fulfill({ status: 200, contentType: 'application/json', body: '{}' });
  });
});

test('login shell is keyboard-ready and has no serious WCAG violations', async ({ page }) => {
  await page.goto('/');

  await expect(page.getByRole('heading', { name: 'Acceso seguro' })).toBeVisible();
  await page.keyboard.press('Tab');
  await expect(page.getByRole('link', { name: 'Saltar al contenido principal' })).toBeFocused();

  const results = await new AxeBuilder({ page })
    .withTags(['wcag2a', 'wcag2aa', 'wcag21a', 'wcag21aa'])
    .analyze();

  const blockingViolations = results.violations.filter(
    (violation) => violation.impact === 'critical' || violation.impact === 'serious',
  );

  expect(blockingViolations).toEqual([]);
});
