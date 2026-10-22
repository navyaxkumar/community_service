import React from 'react';
import { Link } from 'react-router-dom';
import { ArrowRight } from 'lucide-react';
import { HeroSection } from '../components/HeroSection';

export const HomePage: React.FC = () => {
  return (
    <div className="space-y-24 pb-20">
      {/* 1. Full-screen Cinematic Hero */}
      <HeroSection />

      {/* 2. Platform Mission / Concise Explanation */}
      <section className="max-w-5xl mx-auto px-6 sm:px-10">
        <div className="grid grid-cols-1 md:grid-cols-12 gap-8 items-start">
          <div className="md:col-span-5">
            <span className="text-xs uppercase tracking-widest text-[#191919]/50 font-semibold block mb-2">
              Our Mission
            </span>
            <h2 className="text-3xl sm:text-4xl font-serif text-[#191919] leading-tight">
              Protecting people from digital deception before harm happens.
            </h2>
          </div>
          <div className="md:col-span-7 space-y-4 text-sm md:text-base text-[#191919]/70 leading-relaxed">
            <p>
              Digital scams have evolved from obvious typos to sophisticated impersonations of banks, delivery carriers, and government agencies. Anyone can encounter an urgent message demanding immediate payment or confidential credentials.
            </p>
            <p>
              DigitalShield combines instant automated scam analysis with practical, scenario-driven learning modules and quizzes. Whether you are an elderly internet user, a busy professional, or a student, our platform builds lasting digital safety instincts.
            </p>
          </div>
        </div>
      </section>

      {/* 3. How It Works (Step-by-Step) */}
      <section className="max-w-5xl mx-auto px-6 sm:px-10">
        <div className="border-t border-gray-200 pt-16">
          <div className="text-center max-w-2xl mx-auto mb-14">
            <span className="text-xs uppercase tracking-widest text-[#191919]/50 font-semibold block mb-2">
              Three-Step Protection
            </span>
            <h2 className="text-3xl sm:text-4xl font-serif text-[#191919]">
              How DigitalShield Works
            </h2>
            <p className="text-sm text-[#191919]/65 mt-2">
              Simple, accessible, and structured to guide you through suspicious interactions.
            </p>
          </div>

          <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
            <div className="bg-[#F4F3F3] p-8 rounded-xl border border-gray-100 flex flex-col justify-between">
              <div>
                <span className="text-xs font-mono font-semibold text-[#191919]/40 block mb-3">
                  STEP 01
                </span>
                <h3 className="text-lg font-serif text-[#191919] mb-2">
                  Inspect & Check
                </h3>
                <p className="text-xs text-[#191919]/70 leading-relaxed">
                  Paste any unexpected text message, email snippet, or suspicious URL into the Scam Checker to evaluate risk indicators against verified threat patterns.
                </p>
              </div>
              <div className="mt-6 pt-4 border-t border-gray-200/50">
                <Link
                  to="/scam-checker"
                  className="text-xs font-medium text-[#191919] inline-flex items-center gap-1.5 hover:underline"
                >
                  Try Scam Checker <ArrowRight className="w-3.5 h-3.5" />
                </Link>
              </div>
            </div>

            <div className="bg-[#F4F3F3] p-8 rounded-xl border border-gray-100 flex flex-col justify-between">
              <div>
                <span className="text-xs font-mono font-semibold text-[#191919]/40 block mb-3">
                  STEP 02
                </span>
                <h3 className="text-lg font-serif text-[#191919] mb-2">
                  Learn the Patterns
                </h3>
                <p className="text-xs text-[#191919]/70 leading-relaxed">
                  Browse structured modules covering phishing, emergency wire fraud, fake delivery SMS, and executive impersonation with real-world examples.
                </p>
              </div>
              <div className="mt-6 pt-4 border-t border-gray-200/50">
                <Link
                  to="/learn"
                  className="text-xs font-medium text-[#191919] inline-flex items-center gap-1.5 hover:underline"
                >
                  Browse Modules <ArrowRight className="w-3.5 h-3.5" />
                </Link>
              </div>
            </div>

            <div className="bg-[#F4F3F3] p-8 rounded-xl border border-gray-100 flex flex-col justify-between">
              <div>
                <span className="text-xs font-mono font-semibold text-[#191919]/40 block mb-3">
                  STEP 03
                </span>
                <h3 className="text-lg font-serif text-[#191919] mb-2">
                  Test Your Instincts
                </h3>
                <p className="text-xs text-[#191919]/70 leading-relaxed">
                  Take realistic scenario quizzes, track your awareness progress on your personal dashboard, and earn verified safety badges.
                </p>
              </div>
              <div className="mt-6 pt-4 border-t border-gray-200/50">
                <Link
                  to="/quizzes"
                  className="text-xs font-medium text-[#191919] inline-flex items-center gap-1.5 hover:underline"
                >
                  Take a Quiz <ArrowRight className="w-3.5 h-3.5" />
                </Link>
              </div>
            </div>
          </div>
        </div>
      </section>

      {/* 4. Supported Fraud Categories */}
      <section className="max-w-5xl mx-auto px-6 sm:px-10">
        <div className="border-t border-gray-200 pt-16">
          <div className="flex flex-col md:flex-row md:items-end justify-between mb-10 gap-4">
            <div>
              <span className="text-xs uppercase tracking-widest text-[#191919]/50 font-semibold block mb-2">
                Supported Threat Vectors
              </span>
              <h2 className="text-3xl sm:text-4xl font-serif text-[#191919]">
                Common Scams We Address
              </h2>
            </div>
            <Link
              to="/learn"
              className="text-xs font-medium text-[#191919] inline-flex items-center gap-1 hover:underline shrink-0"
            >
              Explore all categories <ArrowRight className="w-3.5 h-3.5" />
            </Link>
          </div>

          <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
            <div className="p-6 bg-white border border-gray-200 rounded-xl hover:border-gray-400 transition-colors">
              <span className="text-[11px] font-mono uppercase text-[#191919]/50 block mb-2">
                Category 01
              </span>
              <h3 className="text-base font-semibold text-[#191919] mb-2">
                Phishing & Suspicious Links
              </h3>
              <p className="text-xs text-[#191919]/70 leading-relaxed">
                Spoofed bank portals, deceptive lookalike domains, shortened URLs masking credential theft forms, and fake security warnings.
              </p>
            </div>

            <div className="p-6 bg-white border border-gray-200 rounded-xl hover:border-gray-400 transition-colors">
              <span className="text-[11px] font-mono uppercase text-[#191919]/50 block mb-2">
                Category 02
              </span>
              <h3 className="text-base font-semibold text-[#191919] mb-2">
                Impersonation & Social Engineering
              </h3>
              <p className="text-xs text-[#191919]/70 leading-relaxed">
                Fake tech support phone calls, law enforcement identity threats, urgent supervisor requests, and family emergency payment appeals.
              </p>
            </div>

            <div className="p-6 bg-white border border-gray-200 rounded-xl hover:border-gray-400 transition-colors">
              <span className="text-[11px] font-mono uppercase text-[#191919]/50 block mb-2">
                Category 03
              </span>
              <h3 className="text-base font-semibold text-[#191919] mb-2">
                OTP & Digital Payment Fraud
              </h3>
              <p className="text-xs text-[#191919]/70 leading-relaxed">
                Deceptive UPI QR codes disguised as refunds, fake payment gateway confirmations, and social-engineering attempts to harvest one-time passcodes.
              </p>
            </div>

            <div className="p-6 bg-white border border-gray-200 rounded-xl hover:border-gray-400 transition-colors">
              <span className="text-[11px] font-mono uppercase text-[#191919]/50 block mb-2">
                Category 04
              </span>
              <h3 className="text-base font-semibold text-[#191919] mb-2">
                Employment & Marketplace Traps
              </h3>
              <p className="text-xs text-[#191919]/70 leading-relaxed">
                Work-from-home task schemes demanding upfront deposits, unverified buyer payment bypasses, and fake invoice wire scams.
              </p>
            </div>
          </div>
        </div>
      </section>

      {/* 5. Practical Digital Safety Guidance */}
      <section className="max-w-5xl mx-auto px-6 sm:px-10">
        <div className="bg-[#F4F3F3] border border-gray-200 rounded-2xl p-8 sm:p-12">
          <div className="max-w-2xl mb-8">
            <span className="text-xs uppercase tracking-widest text-[#191919]/50 font-semibold block mb-2">
              Everyday Rules
            </span>
            <h2 className="text-2xl sm:text-3xl font-serif text-[#191919]">
              Four Golden Rules of Digital Safety
            </h2>
            <p className="text-xs sm:text-sm text-[#191919]/70 mt-2">
              Remember these foundational habits whenever an unexpected digital interaction occurs.
            </p>
          </div>

          <div className="grid grid-cols-1 sm:grid-cols-2 gap-6 text-xs text-[#191919]/80">
            <div className="space-y-1">
              <strong className="block text-sm font-semibold text-[#191919]">
                1. Pause on Artificial Urgency
              </strong>
              <p className="leading-relaxed">
                Scammers rely on panic to bypass critical thinking. If a message gives you minutes to act, stop and verify independently.
              </p>
            </div>
            <div className="space-y-1">
              <strong className="block text-sm font-semibold text-[#191919]">
                2. Never Share Passcodes or OTPs
              </strong>
              <p className="leading-relaxed">
                Legitimate banks, government officials, and delivery services will never ask for your one-time passwords or security PINs.
              </p>
            </div>
            <div className="space-y-1">
              <strong className="block text-sm font-semibold text-[#191919]">
                3. Inspect Sender Addresses Directly
              </strong>
              <p className="leading-relaxed">
                Display names can be faked. Look at the true sending email address and check domain spelling before clicking any embedded link.
              </p>
            </div>
            <div className="space-y-1">
              <strong className="block text-sm font-semibold text-[#191919]">
                4. Verify Through Known Official Channels
              </strong>
              <p className="leading-relaxed">
                Never call numbers sent in suspicious text messages. Look up the verified phone number on the official back of your card or official website.
              </p>
            </div>
          </div>
        </div>
      </section>

      {/* 6. Final Call to Action */}
      <section className="max-w-5xl mx-auto px-6 sm:px-10 text-center">
        <div className="border border-gray-200 bg-white rounded-2xl py-14 px-6 sm:px-12 shadow-sm">
          <h2 className="text-3xl sm:text-4xl font-serif text-[#191919] max-w-xl mx-auto leading-tight">
            Ready to build safer digital habits for yourself and your family?
          </h2>
          <p className="text-sm text-[#191919]/70 max-w-md mx-auto mt-3 mb-8 leading-relaxed">
            Start by analyzing suspicious messages with our Scam Checker or jump directly into our interactive fraud awareness modules.
          </p>
          <div className="flex flex-col sm:flex-row items-center justify-center gap-3">
            <Link
              to="/scam-checker"
              className="w-full sm:w-auto bg-[#191919] text-white px-7 py-3 rounded-full text-xs sm:text-sm font-medium hover:bg-black/85 transition-colors shadow-sm"
            >
              Check a Message Now
            </Link>
            <Link
              to="/register"
              className="w-full sm:w-auto px-6 py-3 rounded-full text-xs sm:text-sm font-medium text-[#191919] border border-gray-300 hover:bg-gray-50 transition-colors"
            >
              Create Free Account
            </Link>
          </div>
        </div>
      </section>
    </div>
  );
};
