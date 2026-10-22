import React, { useState } from 'react';
import { Navbar } from './components/Navbar';
import { HeroSection } from './components/HeroSection';
import { DestinationModal } from './components/DestinationModal';

export const App: React.FC = () => {
  const [activeDestination, setActiveDestination] = useState<string | null>(null);

  const handleNavigate = (destination: string) => {
    if (destination === 'home') {
      window.scrollTo({ top: 0, behavior: 'smooth' });
      setActiveDestination(null);
      return;
    }
    setActiveDestination(destination);
  };

  const handleCloseModal = () => {
    setActiveDestination(null);
  };

  return (
    <div className="relative min-h-screen bg-white text-[#191919] font-sans antialiased overflow-x-hidden">
      {/* Fixed Transparent Navigation */}
      <Navbar onNavigate={handleNavigate} />

      {/* Main Full-Viewport Hero Experience */}
      <main>
        <HeroSection onNavigate={handleNavigate} />
      </main>

      {/* Accessible Interactive Destination Dialog */}
      <DestinationModal
        destination={activeDestination}
        onClose={handleCloseModal}
      />
    </div>
  );
};

export default App;
