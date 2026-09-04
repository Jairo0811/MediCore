import { render, screen } from '@testing-library/react';
import { describe, expect, it } from 'vitest';
import ErrorMessage from './ErrorMessage';

describe('ErrorMessage', () => {
  it('renders an accessible alert when a message exists', () => {
    render(<ErrorMessage message="No fue posible completar la operación." />);

    expect(screen.getByRole('alert')).toHaveTextContent('No fue posible completar la operación.');
  });

  it('renders nothing when there is no message', () => {
    const { container } = render(<ErrorMessage message={null} />);

    expect(container).toBeEmptyDOMElement();
  });
});
