import { StrictMode } from 'react';
import { createRoot } from 'react-dom/client';
import App from './App';
import './styles.css';
import './ui-enhancements.css';
import './branding.css';
import './form-controls.css';
import './ui-responsive.css';
import './accessibility.css';

createRoot(document.getElementById('root')!).render(
  <StrictMode>
    <App />
  </StrictMode>,
);
