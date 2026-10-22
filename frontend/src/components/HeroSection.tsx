import React from 'react';
import { BoomerangVideoBg } from './BoomerangVideoBg';
import { FeatureInfoPanel } from './FeatureInfoPanel';

interface HeroSectionProps {
  onNavigate: (destination: string) => void;
}

export const HeroSection: React.FC<HeroSectionProps> = ({ onNavigate }) => {
  return (
    <section
      id="home"
      className="relative flex flex-col items-center min-h-screen lg:h-screen lg:overflow-hidden justify-between"
      aria-label="DigitalShield Hero"
    >
      {/* Cinematic Boomerang Video Background at z-0 */}
      <BoomerangVideoBg />

      {/* Hero Typography & Primary Actions at z-10 */}
      <div className="relative z-10 flex flex-col items-center text-center pt-20 sm:pt-24 md:pt-28 lg:pt-28 px-4 sm:px-6 max-w-4xl mx-auto">
        {/* Eyebrow */}
        <span className="text-[11px] sm:text-xs uppercase tracking-[0.2em] font-medium text-[#191919]/75 mb-2.5 sm:mb-3">
          YOUR EVERYDAY DIGITAL SAFETY COMPANION
        </span>

        {/* Main Headline */}
        <h1 className="font-serif text-4xl sm:text-5xl md:text-6xl lg:text-7xl xl:text-8xl leading-[1.08] tracking-tighter text-[#191919] font-normal">
          Spot the Scam.
          <br />
          Stay One Step Ahead.
        </h1>

        {/* Supporting Paragraph */}
        <p className="max-w-sm sm:max-w-md mt-3.5 sm:mt-5 text-sm md:text-base text-[#191919]/70 leading-relaxed">
          Learn to recognize online fraud, check suspicious messages, and navigate
          the digital world with confidence.
        </p>

        {/* CTAs */}
        <div className="mt-5 sm:mt-7 flex flex-col sm:flex-row items-center gap-3 sm:gap-4">
          <button
            type="button"
            onClick={() => onNavigate('scam-checker')}
            className="w-full sm:w-auto bg-[#191919] text-white px-7 sm:px-8 py-3 sm:py-3.5 rounded-full text-sm font-medium hover:bg-black/85 active:scale-98 transition-all duration-200 shadow-sm focus:outline-none focus:ring-2 focus:ring-[#191919]/30"
          >
            Check for Scams
          </button>

          <button
            type="button"
            onClick={() => onNavigate('learning')}
            className="w-full sm:w-auto px-6 py-3 rounded-full text-sm font-medium text-[#191919]/70 hover:text-[#191919] hover:bg-black/5 transition-colors duration-200 focus:outline-none focus-visible:underline"
          >
            Explore Learning
          </button>
        </div>
      </div>

      {/* Bottom-anchored Information Panel */}
      <FeatureInfoPanel onNavigate={onNavigate} />
    </section>
  );
};
