import React from 'react';
import { BrowserRouter } from 'react-router-dom';
import { AuthProvider } from './context/AuthContext';
import { AudienceProvider } from './context/AudienceContext';
import { AppRoutes } from './routes/AppRoutes';

export const App: React.FC = () => {
  return (
    <BrowserRouter>
      <AuthProvider>
        <AudienceProvider>
          <AppRoutes />
        </AudienceProvider>
      </AuthProvider>
    </BrowserRouter>
  );
};

export default App;
