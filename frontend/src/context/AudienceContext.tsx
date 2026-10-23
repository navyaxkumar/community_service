import React, { createContext, useContext, useEffect, useState } from 'react';
import { AudienceSegment, AUDIENCE_SEGMENTS, getAudienceMeta, AudienceOption } from '../types/audience';

interface AudienceContextType {
  audience: AudienceSegment;
  setAudience: (segment: AudienceSegment) => void;
  isHighContrast: boolean;
  toggleHighContrast: () => void;
  fontSize: 'normal' | 'large' | 'extra-large';
  setFontSize: (size: 'normal' | 'large' | 'extra-large') => void;
  isSpeaking: boolean;
  readAloud: (text: string) => void;
  stopReadAloud: () => void;
  currentMeta: AudienceOption;
}

const AudienceContext = createContext<AudienceContextType | undefined>(undefined);

export const AudienceProvider: React.FC<{ children: React.ReactNode }> = ({ children }) => {
  const [audience, setAudienceState] = useState<AudienceSegment>(() => {
    try {
      const stored = localStorage.getItem('ds_audience');
      if (stored && AUDIENCE_SEGMENTS.some((s) => s.id === stored)) {
        return stored as AudienceSegment;
      }
    } catch {
      // Fallback
    }
    return 'All';
  });

  const [isHighContrast, setIsHighContrast] = useState<boolean>(() => {
    try {
      const stored = localStorage.getItem('ds_high_contrast');
      return stored ? JSON.parse(stored) : false;
    } catch {
      return false;
    }
  });

  const [fontSize, setFontSizeState] = useState<'normal' | 'large' | 'extra-large'>(() => {
    try {
      const stored = localStorage.getItem('ds_font_size');
      if (stored === 'normal' || stored === 'large' || stored === 'extra-large') {
        return stored;
      }
    } catch {
      // Fallback
    }
    return 'normal';
  });

  const [isSpeaking, setIsSpeaking] = useState<boolean>(false);

  // Automatically adapt contrast and font size when switching to/from Seniors
  const setAudience = (segment: AudienceSegment) => {
    setAudienceState(segment);
    try {
      localStorage.setItem('ds_audience', segment);
    } catch {
      // Ignore storage errors
    }

    if (segment === 'Seniors') {
      setIsHighContrast(true);
      setFontSizeState('large');
    }
  };

  const toggleHighContrast = () => {
    setIsHighContrast((prev) => {
      const next = !prev;
      try {
        localStorage.setItem('ds_high_contrast', JSON.stringify(next));
      } catch {
        // Ignore
      }
      return next;
    });
  };

  const setFontSize = (size: 'normal' | 'large' | 'extra-large') => {
    setFontSizeState(size);
    try {
      localStorage.setItem('ds_font_size', size);
    } catch {
      // Ignore
    }
  };

  const stopReadAloud = () => {
    if (typeof window !== 'undefined' && 'speechSynthesis' in window) {
      window.speechSynthesis.cancel();
      setIsSpeaking(false);
    }
  };

  const readAloud = (text: string) => {
    if (typeof window === 'undefined' || !('speechSynthesis' in window)) {
      return;
    }

    window.speechSynthesis.cancel();
    if (!text || !text.trim()) {
      setIsSpeaking(false);
      return;
    }

    const cleanText = text
      .replace(/<!--.*?-->/gs, '')
      .replace(/\[Audience:.*?\]/g, '')
      .replace(/[#*_`]/g, '')
      .trim();

    const utterance = new SpeechSynthesisUtterance(cleanText);
    utterance.rate = audience === 'EarlyLearners' || audience === 'Seniors' ? 0.9 : 1.0;
    utterance.pitch = audience === 'EarlyLearners' ? 1.1 : 1.0;

    utterance.onstart = () => setIsSpeaking(true);
    utterance.onend = () => setIsSpeaking(false);
    utterance.onerror = () => setIsSpeaking(false);

    window.speechSynthesis.speak(utterance);
  };

  useEffect(() => {
    return () => {
      stopReadAloud();
    };
  }, []);

  const currentMeta = getAudienceMeta(audience);

  return (
    <AudienceContext.Provider
      value={{
        audience,
        setAudience,
        isHighContrast,
        toggleHighContrast,
        fontSize,
        setFontSize,
        isSpeaking,
        readAloud,
        stopReadAloud,
        currentMeta,
      }}
    >
      {children}
    </AudienceContext.Provider>
  );
};

export const useAudience = (): AudienceContextType => {
  const context = useContext(AudienceContext);
  if (!context) {
    throw new Error('useAudience must be used within an AudienceProvider');
  }
  return context;
};
