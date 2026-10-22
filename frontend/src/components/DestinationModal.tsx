import React, { useEffect } from 'react';

interface DestinationModalProps {
  destination: string | null;
  onClose: () => void;
}

export const DestinationModal: React.FC<DestinationModalProps> = ({
  destination,
  onClose,
}) => {
  // Close on Escape key
  useEffect(() => {
    const handleKeyDown = (e: KeyboardEvent) => {
      if (e.key === 'Escape') {
        onClose();
      }
    };
    if (destination) {
      window.addEventListener('keydown', handleKeyDown);
    }
    return () => window.removeEventListener('keydown', handleKeyDown);
  }, [destination, onClose]);

  if (!destination || destination === 'home') return null;

  return (
    <div
      className="fixed inset-0 z-50 flex items-center justify-center p-4 sm:p-6 bg-black/40 backdrop-blur-sm"
      role="dialog"
      aria-modal="true"
      aria-labelledby="modal-title"
    >
      <div className="relative w-full max-w-2xl bg-white rounded-2xl shadow-2xl border border-gray-100 overflow-hidden flex flex-col max-h-[90vh]">
        {/* Modal Header */}
        <div className="px-6 py-5 border-b border-gray-100 flex items-center justify-between">
          <div className="flex items-center gap-3">
            <div className="w-8 h-8 rounded-full bg-[#191919] text-white flex items-center justify-center">
              <svg
                className="w-4 h-4"
                viewBox="0 0 24 24"
                fill="none"
                stroke="currentColor"
                strokeWidth="2"
                strokeLinecap="round"
                strokeLinejoin="round"
              >
                <path d="M12 2L4 5.5V11.5C4 16.5 7.4 21.1 12 22.3C16.6 21.1 20 16.5 20 11.5V5.5L12 2Z" />
              </svg>
            </div>
            <div>
              <span className="text-[10px] uppercase tracking-wider text-[#191919]/50 font-semibold block">
                DigitalShield Platform
              </span>
              <h3
                id="modal-title"
                className="text-lg font-serif font-medium text-[#191919]"
              >
                {destination === 'scam-checker' && 'Scam Checker & Verification'}
                {destination === 'learning' && 'Digital Fraud Learning Hub'}
                {destination === 'quizzes' && 'Safety Practice & Habits'}
              </h3>
            </div>
          </div>

          <button
            type="button"
            onClick={onClose}
            className="p-1.5 text-gray-400 hover:text-[#191919] rounded-lg hover:bg-gray-100 transition-colors focus:outline-none focus:ring-2 focus:ring-[#191919]/20"
            aria-label="Close dialog"
          >
            <svg
              className="w-5 h-5"
              fill="none"
              stroke="currentColor"
              viewBox="0 0 24 24"
              strokeWidth="2"
            >
              <path strokeLinecap="round" strokeLinejoin="round" d="M6 18L18 6M6 6l12 12" />
            </svg>
          </button>
        </div>

        {/* Modal Body */}
        <div className="p-6 overflow-y-auto space-y-6">
          {destination === 'scam-checker' && (
            <div className="space-y-5">
              <p className="text-sm text-[#191919]/70 leading-relaxed">
                The DigitalShield Scam Checker provides heuristic and threat-intelligence analysis for suspicious messages, URLs, and emails.
              </p>

              <div className="rounded-xl border border-gray-100 bg-[#F4F3F3]/60 p-4 space-y-2">
                <span className="text-xs font-semibold text-[#191919] uppercase tracking-wider block">
                  Backend Verification Architecture
                </span>
                <p className="text-xs text-[#191919]/70 leading-relaxed">
                  Live fraud scoring and multi-point indicator analysis are performed securely through the ASP.NET Core backend API (<code className="text-[#191919] font-mono bg-white px-1 py-0.5 rounded">/api/fraud/check</code>), requiring an authorized user session to protect threat feed integrity and data privacy.
                </p>
              </div>

              <div className="space-y-3">
                <span className="text-xs font-semibold text-[#191919] uppercase tracking-wider block">
                  Immediate Self-Check Indicators
                </span>
                <div className="grid grid-cols-1 sm:grid-cols-2 gap-3 text-xs">
                  <div className="p-3 bg-white border border-gray-200 rounded-lg">
                    <span className="font-medium text-[#191919] block mb-1">1. Artificial Urgency</span>
                    <p className="text-[#191919]/60">Demands immediate action, account suspension threats, or short timers.</p>
                  </div>
                  <div className="p-3 bg-white border border-gray-200 rounded-lg">
                    <span className="font-medium text-[#191919] block mb-1">2. Domain Mismatches</span>
                    <p className="text-[#191919]/60">Look closely at domain spelling (e.g., lookalike characters, unauthorized subdomains).</p>
                  </div>
                  <div className="p-3 bg-white border border-gray-200 rounded-lg">
                    <span className="font-medium text-[#191919] block mb-1">3. Credential Harvesting</span>
                    <p className="text-[#191919]/60">Requests for OTPs, passwords, or personal identity numbers via unverified forms.</p>
                  </div>
                  <div className="p-3 bg-white border border-gray-200 rounded-lg">
                    <span className="font-medium text-[#191919] block mb-1">4. Payment Bypass</span>
                    <p className="text-[#191919]/60">Directing away from legitimate payment gateways to wire transfers or gift cards.</p>
                  </div>
                </div>
              </div>
            </div>
          )}

          {destination === 'learning' && (
            <div className="space-y-4">
              <p className="text-sm text-[#191919]/70 leading-relaxed">
                Explore guided educational modules covering prevalent online deception
                tactics and verified counter-measures.
              </p>

              <div className="grid grid-cols-1 sm:grid-cols-2 gap-3 pt-2">
                <div className="p-4 bg-[#F4F3F3] rounded-lg">
                  <div className="text-xs font-bold text-[#191919] mb-1">
                    01 / Phishing & Spear-Phishing
                  </div>
                  <p className="text-xs text-[#191919]/70 leading-relaxed">
                    Identify fraudulent emails, spoofed headers, malicious attachments, and
                    fake login portals mimicking familiar services.
                  </p>
                </div>

                <div className="p-4 bg-[#F4F3F3] rounded-lg">
                  <div className="text-xs font-bold text-[#191919] mb-1">
                    02 / Impersonation & Social Engineering
                  </div>
                  <p className="text-xs text-[#191919]/70 leading-relaxed">
                    Learn how scammers pose as government officials, banking agents, or
                    executives requesting emergency actions.
                  </p>
                </div>

                <div className="p-4 bg-[#F4F3F3] rounded-lg">
                  <div className="text-xs font-bold text-[#191919] mb-1">
                    03 / Payment & Wire Fraud
                  </div>
                  <p className="text-xs text-[#191919]/70 leading-relaxed">
                    Unpack fake invoice scams, unverified crypto transfers, and peer-to-peer
                    payment bypass traps.
                  </p>
                </div>

                <div className="p-4 bg-[#F4F3F3] rounded-lg">
                  <div className="text-xs font-bold text-[#191919] mb-1">
                    04 / Account Takeover Prevention
                  </div>
                  <p className="text-xs text-[#191919]/70 leading-relaxed">
                    Master multi-factor authentication, credential hygiene, and proactive
                    breach alerting.
                  </p>
                </div>
              </div>
            </div>
          )}

          {destination === 'quizzes' && (
            <div className="space-y-4">
              <p className="text-sm text-[#191919]/70 leading-relaxed">
                Test your instincts through real-world scenarios designed to build long-term
                digital safety muscle memory.
              </p>

              <div className="space-y-3 pt-2">
                <div className="p-4 border border-gray-200 rounded-lg flex items-center justify-between">
                  <div>
                    <h4 className="text-sm font-medium text-[#191919]">
                      Scenario: Delivery Delay Notification
                    </h4>
                    <p className="text-xs text-[#191919]/60 mt-0.5">
                      SMS claiming a package cannot be delivered without a $1.50 redelivery fee.
                    </p>
                  </div>
                  <span className="text-xs px-2.5 py-1 bg-[#F4F3F3] rounded font-medium text-[#191919]/80 shrink-0">
                    5 min
                  </span>
                </div>

                <div className="p-4 border border-gray-200 rounded-lg flex items-center justify-between">
                  <div>
                    <h4 className="text-sm font-medium text-[#191919]">
                      Scenario: Urgent Tech Support Alert
                    </h4>
                    <p className="text-xs text-[#191919]/60 mt-0.5">
                      Browser pop-up warning of virus infection with an urgent toll-free hotline.
                    </p>
                  </div>
                  <span className="text-xs px-2.5 py-1 bg-[#F4F3F3] rounded font-medium text-[#191919]/80 shrink-0">
                    7 min
                  </span>
                </div>

                <div className="p-4 border border-gray-200 rounded-lg flex items-center justify-between">
                  <div>
                    <h4 className="text-sm font-medium text-[#191919]">
                      Scenario: Executive Gift Card Request
                    </h4>
                    <p className="text-xs text-[#191919]/60 mt-0.5">
                      Email apparently from company leadership requesting prompt purchase of gift cards.
                    </p>
                  </div>
                  <span className="text-xs px-2.5 py-1 bg-[#F4F3F3] rounded font-medium text-[#191919]/80 shrink-0">
                    4 min
                  </span>
                </div>
              </div>
            </div>
          )}
        </div>

        {/* Modal Footer */}
        <div className="px-6 py-4 bg-gray-50 border-t border-gray-100 flex justify-between items-center">
          <span className="text-xs text-[#191919]/50">
            DigitalShield &bull; Educational Platform
          </span>
          <button
            type="button"
            onClick={onClose}
            className="px-5 py-2 text-xs font-medium text-[#191919] hover:bg-gray-200 bg-gray-100 rounded-full transition-colors"
          >
            Close
          </button>
        </div>
      </div>
    </div>
  );
};
            <div className="space-y-4">
              <p className="text-sm text-[#191919]/70 leading-relaxed">
                Explore guided educational modules covering prevalent online deception
                tactics and verified counter-measures.
              </p>

              <div className="grid grid-cols-1 sm:grid-cols-2 gap-3 pt-2">
                <div className="p-4 bg-[#F4F3F3] rounded-lg">
                  <div className="text-xs font-bold text-[#191919] mb-1">
                    01 / Phishing & Spear-Phishing
                  </div>
                  <p className="text-xs text-[#191919]/70 leading-relaxed">
                    Identify fraudulent emails, spoofed headers, malicious attachments, and
                    fake login portals mimicking familiar services.
                  </p>
                </div>

                <div className="p-4 bg-[#F4F3F3] rounded-lg">
                  <div className="text-xs font-bold text-[#191919] mb-1">
                    02 / Impersonation & Social Engineering
                  </div>
                  <p className="text-xs text-[#191919]/70 leading-relaxed">
                    Learn how scammers pose as government officials, banking agents, or
                    executives requesting emergency actions.
                  </p>
                </div>

                <div className="p-4 bg-[#F4F3F3] rounded-lg">
                  <div className="text-xs font-bold text-[#191919] mb-1">
                    03 / Payment & Wire Fraud
                  </div>
                  <p className="text-xs text-[#191919]/70 leading-relaxed">
                    Unpack fake invoice scams, unverified crypto transfers, and peer-to-peer
                    payment bypass traps.
                  </p>
                </div>

                <div className="p-4 bg-[#F4F3F3] rounded-lg">
                  <div className="text-xs font-bold text-[#191919] mb-1">
                    04 / Account Takeover Prevention
                  </div>
                  <p className="text-xs text-[#191919]/70 leading-relaxed">
                    Master multi-factor authentication, credential hygiene, and proactive
                    breach alerting.
                  </p>
                </div>
              </div>
            </div>
          )}

          {destination === 'quizzes' && (
            <div className="space-y-4">
              <p className="text-sm text-[#191919]/70 leading-relaxed">
                Test your instincts through real-world scenarios designed to build long-term
                digital safety muscle memory.
              </p>

              <div className="space-y-3 pt-2">
                <div className="p-4 border border-gray-200 rounded-lg flex items-center justify-between">
                  <div>
                    <h4 className="text-sm font-medium text-[#191919]">
                      Scenario: Delivery Delay Notification
                    </h4>
                    <p className="text-xs text-[#191919]/60 mt-0.5">
                      SMS claiming a package cannot be delivered without a $1.50 redelivery fee.
                    </p>
                  </div>
                  <span className="text-xs px-2.5 py-1 bg-[#F4F3F3] rounded font-medium text-[#191919]/80 shrink-0">
                    5 min
                  </span>
                </div>

                <div className="p-4 border border-gray-200 rounded-lg flex items-center justify-between">
                  <div>
                    <h4 className="text-sm font-medium text-[#191919]">
                      Scenario: Urgent Tech Support Alert
                    </h4>
                    <p className="text-xs text-[#191919]/60 mt-0.5">
                      Browser pop-up warning of virus infection with an urgent toll-free hotline.
                    </p>
                  </div>
                  <span className="text-xs px-2.5 py-1 bg-[#F4F3F3] rounded font-medium text-[#191919]/80 shrink-0">
                    7 min
                  </span>
                </div>

                <div className="p-4 border border-gray-200 rounded-lg flex items-center justify-between">
                  <div>
                    <h4 className="text-sm font-medium text-[#191919]">
                      Scenario: Executive Gift Card Request
                    </h4>
                    <p className="text-xs text-[#191919]/60 mt-0.5">
                      Email apparently from company leadership requesting prompt purchase of gift cards.
                    </p>
                  </div>
                  <span className="text-xs px-2.5 py-1 bg-[#F4F3F3] rounded font-medium text-[#191919]/80 shrink-0">
                    4 min
                  </span>
                </div>
              </div>
            </div>
          )}
        </div>

        {/* Modal Footer */}
        <div className="px-6 py-4 bg-gray-50 border-t border-gray-100 flex justify-between items-center">
          <span className="text-xs text-[#191919]/50">
            DigitalShield &bull; Educational Platform
          </span>
          <button
            type="button"
            onClick={onClose}
            className="px-5 py-2 text-xs font-medium text-[#191919] hover:bg-gray-200 bg-gray-100 rounded-full transition-colors"
          >
            Close
          </button>
        </div>
      </div>
    </div>
  );
};
