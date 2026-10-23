import React from 'react';
import { useAudience } from '../../context/AudienceContext';
import { AUDIENCE_SEGMENTS, AudienceSegment } from '../../types/audience';
import {
  Globe,
  Smile,
  Gamepad2,
  Sparkles,
  GraduationCap,
  Shield,
  HeartHandshake,
  Briefcase,
  VolumeX,
  Eye,
  Type,
} from 'lucide-react';

interface AudienceSelectorProps {
  onSelect?: (segment: AudienceSegment) => void;
  showAccessibilityToolbar?: boolean;
  className?: string;
}

export const AudienceSelector: React.FC<AudienceSelectorProps> = ({
  onSelect,
  showAccessibilityToolbar = true,
  className = '',
}) => {
  const {
    audience,
    setAudience,
    isHighContrast,
    toggleHighContrast,
    fontSize,
    setFontSize,
    isSpeaking,
    stopReadAloud,
    currentMeta,
  } = useAudience();

  const handleSelect = (segment: AudienceSegment) => {
    setAudience(segment);
    if (onSelect) {
      onSelect(segment);
    }
  };

  const getIcon = (iconName: string, classNameStr: string) => {
    switch (iconName) {
      case 'Smile':
        return <Smile className={classNameStr} />;
      case 'Gamepad2':
        return <Gamepad2 className={classNameStr} />;
      case 'Sparkles':
        return <Sparkles className={classNameStr} />;
      case 'GraduationCap':
        return <GraduationCap className={classNameStr} />;
      case 'HeartHandshake':
        return <HeartHandshake className={classNameStr} />;
      case 'Briefcase':
        return <Briefcase className={classNameStr} />;
      case 'Shield':
        return <Shield className={classNameStr} />;
      default:
        return <Globe className={classNameStr} />;
    }
  };

  return (
    <div className={`space-y-4 ${className}`}>
      {/* Header & Accessibility Bar */}
      <div className="flex flex-col sm:flex-row sm:items-center justify-between gap-3 pb-2 border-b border-gray-200">
        <div>
          <span className="text-[10px] uppercase tracking-wider font-semibold text-[#191919]/60 block">
            Adaptive Learning Persona
          </span>
          <p className="text-xs text-[#191919]/75 mt-0.5">
            Tailoring explanations, examples, and reading levels for{' '}
            <strong className="text-[#191919]">{currentMeta.label}</strong> ({currentMeta.ageRange}).
          </p>
        </div>

        {showAccessibilityToolbar && (
          <div className="flex items-center space-x-2 self-start sm:self-auto">
            {/* Read Aloud Stop Control */}
            {isSpeaking && (
              <button
                type="button"
                onClick={stopReadAloud}
                className="inline-flex items-center gap-1 px-2.5 py-1 text-xs font-semibold bg-rose-100 text-rose-800 rounded-lg hover:bg-rose-200 transition-colors animate-pulse"
                title="Stop audio reading"
              >
                <VolumeX className="w-3.5 h-3.5" />
                <span>Stop Audio</span>
              </button>
            )}

            {/* High Contrast Toggle */}
            <button
              type="button"
              onClick={toggleHighContrast}
              aria-pressed={isHighContrast}
              className={`inline-flex items-center gap-1 px-2.5 py-1 text-xs font-semibold rounded-lg border transition-all ${
                isHighContrast
                  ? 'bg-black text-white border-black ring-2 ring-black/20'
                  : 'bg-white text-gray-700 border-gray-200 hover:bg-gray-100'
              }`}
              title="Toggle High-Contrast Senior Accessibility Mode"
            >
              <Eye className="w-3.5 h-3.5" />
              <span>{isHighContrast ? 'High Contrast: On' : 'High Contrast'}</span>
            </button>

            {/* Font Size Adjuster */}
            <div className="inline-flex items-center border border-gray-200 rounded-lg bg-white overflow-hidden text-xs">
              <span className="px-2 py-1 text-gray-400 border-r border-gray-100 flex items-center">
                <Type className="w-3 h-3" />
              </span>
              <button
                type="button"
                onClick={() => setFontSize('normal')}
                className={`px-2 py-1 ${fontSize === 'normal' ? 'bg-[#191919] text-white font-semibold' : 'text-gray-600 hover:bg-gray-100'}`}
                title="Normal text size"
              >
                A
              </button>
              <button
                type="button"
                onClick={() => setFontSize('large')}
                className={`px-2 py-1 ${fontSize === 'large' ? 'bg-[#191919] text-white font-semibold' : 'text-gray-600 hover:bg-gray-100'}`}
                title="Large text size"
              >
                A+
              </button>
              <button
                type="button"
                onClick={() => setFontSize('extra-large')}
                className={`px-2 py-1 ${fontSize === 'extra-large' ? 'bg-[#191919] text-white font-semibold' : 'text-gray-600 hover:bg-gray-100'}`}
                title="Extra large text size"
              >
                A++
              </button>
            </div>
          </div>
        )}
      </div>

      {/* Persona Pill Grid */}
      <div className="grid grid-cols-2 sm:grid-cols-4 lg:grid-cols-8 gap-2">
        {AUDIENCE_SEGMENTS.map((seg) => {
          const isSelected = audience === seg.id;
          return (
            <button
              key={seg.id}
              type="button"
              onClick={() => handleSelect(seg.id)}
              aria-pressed={isSelected}
              className={`p-2.5 rounded-xl border text-left transition-all flex flex-col justify-between focus:outline-none focus:ring-2 focus:ring-[#191919] ${
                isSelected
                  ? 'border-[#191919] bg-[#191919] text-white shadow-sm ring-1 ring-[#191919]'
                  : 'border-gray-200 bg-white text-[#191919]/80 hover:border-gray-400 hover:bg-gray-50'
              }`}
            >
              <div className="flex items-center justify-between mb-1.5">
                <div
                  className={`w-6 h-6 rounded-md flex items-center justify-center ${
                    isSelected ? 'bg-white/20 text-white' : 'bg-gray-100 text-[#191919]'
                  }`}
                >
                  {getIcon(seg.iconName, 'w-3.5 h-3.5')}
                </div>
                <span
                  className={`text-[9px] font-mono px-1 py-0.5 rounded ${
                    isSelected ? 'bg-white/20 text-white' : 'bg-gray-100 text-gray-600'
                  }`}
                >
                  {seg.ageRange}
                </span>
              </div>
              <div className="font-semibold text-xs leading-tight line-clamp-1">
                {seg.label}
              </div>
            </button>
          );
        })}
      </div>

      {/* Audience Mode Banner */}
      <div
        className={`p-3.5 rounded-xl border flex items-start gap-3 transition-colors ${currentMeta.themeColor}`}
      >
        <div className="mt-0.5 shrink-0">
          {getIcon(currentMeta.iconName, 'w-4 h-4')}
        </div>
        <div className="text-xs leading-relaxed">
          <strong className="font-semibold">{currentMeta.label} ({currentMeta.ageRange}):</strong>{' '}
          {currentMeta.tagline}
          {audience === 'EarlyLearners' && (
            <span className="block mt-1 font-medium text-emerald-900">
              💡 Tip: Click the "Listen Aloud" button on any lesson card to hear the audio narration.
            </span>
          )}
          {audience === 'Seniors' && (
            <span className="block mt-1 font-medium text-amber-950">
              💡 High-contrast view and enlarged typography are automatically enabled for maximum legibility.
            </span>
          )}
          {audience === 'Professionals' && (
            <span className="block mt-1 font-medium text-slate-900">
              💡 Lessons feature corporate threat indicators, MITRE references, and enterprise escalation procedures.
            </span>
          )}
        </div>
      </div>
    </div>
  );
};
