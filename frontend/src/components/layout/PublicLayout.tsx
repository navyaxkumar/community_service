import React from 'react';
import { Outlet, useLocation } from 'react-router-dom';
import { Navbar } from '../common/Navbar';
import { Footer } from '../common/Footer';

export const PublicLayout: React.FC = () => {
  const location = useLocation();
  const isHome = location.pathname === '/';

  return (
    <div className="min-h-screen flex flex-col bg-white text-[#191919] font-sans antialiased selection:bg-[#191919] selection:text-white">
      <Navbar transparentOnHero={isHome} />
      <main className="flex-grow">
        <Outlet />
      </main>
      <Footer />
    </div>
  );
};
