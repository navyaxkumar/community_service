import React from 'react';
import { FeatureRow } from './FeatureRow';

interface FeatureInfoPanelProps {
  onNavigate: (destination: string) => void;
}

export const FeatureInfoPanel: React.FC<FeatureInfoPanelProps> = ({ onNavigate }) => {
  return (
    <section
      className="mt-auto w-full max-w-5xl px-4 sm:px-6 z-10"
      aria-label="What DigitalShield Does"
    >
      <div className="bg-white/90 backdrop-blur-sm border border-gray-200 border-b-0 pt-6 sm:pt-8 md:pt-10 lg:pt-12 px-5 sm:px-8 md:px-12 pb-0 shadow-sm rounded-t-xl sm:rounded-t-2xl">
        {/* Row 1: Two Columns */}
        <div className="grid grid-cols-1 md:grid-cols-2 gap-4 sm:gap-6 md:gap-8 lg:gap-16 items-start">
          {/* Left Column: Micro-label & Serif Heading */}
          <div>
            <span className="block text-[11px] uppercase tracking-[0.2em] text-[#191919]/50 font-medium">
              WHAT DOES DIGITALSHIELD DO?
            </span>
            <h2 className="mt-2 sm:mt-2.5 text-2xl sm:text-3xl md:text-4xl font-serif font-normal leading-tight tracking-tight text-[#191919]">
              Awareness that{' '}
              <br className="hidden sm:inline" />
              builds confidence.
            </h2>
          </div>

          {/* Right Column: Clean Supporting Copy */}
          <div className="md:self-end">
            <p className="text-sm md:text-[15px] text-[#191919]/70 leading-relaxed">
              Learn the warning signs of digital fraud, practise identifying
              suspicious messages and links, and build safer online habits through
              guided scenarios and quizzes.
            </p>
          </div>
        </div>

        {/* Divider */}
        <div className="mt-4 sm:mt-6 md:mt-8 h-px bg-gray-200 w-full" role="separator" />

        {/* Row 2: Three Feature Items */}
        <div className="grid grid-cols-1 sm:grid-cols-3 gap-2 sm:gap-3 pt-4 sm:pt-5 pb-5 sm:pb-7">
          <FeatureRow
            number="01"
            label="Recognize"
            subtitle="learn common fraud warning signs"
            destination="learning"
            onNavigate={onNavigate}
          />
          <FeatureRow
            number="02"
            label="Check"
            subtitle="assess suspicious messages and URLs"
            destination="scam-checker"
            onNavigate={onNavigate}
          />
          <FeatureRow
            number="03"
            label="Protect"
            subtitle="practise safer digital habits"
            destination="quizzes"
            onNavigate={onNavigate}
          />
        </div>
      </div>
    </section>
  );
};
