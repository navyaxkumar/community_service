import React, { useState } from 'react';
import { Link } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';
import { fraudService } from '../services/fraudService';
import { FraudAnalysisResponse } from '../types/fraud';
import { LoadingSpinner } from '../components/common/LoadingSpinner';
import { ErrorMessage } from '../components/common/ErrorMessage';

const EXAMPLE_SAMPLES = [
  {
    title: 'Bank Account Suspension SMS',
    content: 'URGENT: Your State Bank account has been temporarily locked due to irregular activity. Restore access immediately to avoid permanent closure: http://sbi-login-verify.secure-portal.com',
    type: 'Message',
    senderKnown: false,
  },
  {
    title: 'Postal Delivery Fee Notification',
    content: 'Package delivery #IN-90821 cannot proceed. An unpaid parcel clearance fee of Rs 49 is pending. Confirm payment details today at http://bit.ly/del-track-post or package will be returned to sender.',
    type: 'Message',
    senderKnown: false,
  },
  {
    title: 'Executive Urgency Email',
    content: 'Are you at your desk? I am in a board meeting and need you to purchase 5 Apple gift cards immediately for a client presentation. Email the redemption codes back to me right away.',
    type: 'Message',
    senderKnown: false,
  },
];

export const ScamCheckerPage: React.FC = () => {
  const { isAuthenticated } = useAuth();

  const [inputMode, setInputMode] = useState<'message' | 'url'>('message');
  const [content, setContent] = useState('');
  const [urlInput, setUrlInput] = useState('');
  const [senderKnown, setSenderKnown] = useState<boolean>(false);

  const [validationError, setValidationError] = useState<string | null>(null);
  const [isLoading, setIsLoading] = useState<boolean>(false);
  const [apiError, setApiError] = useState<string | null>(null);
  const [analysisResult, setAnalysisResult] = useState<FraudAnalysisResponse | null>(null);

  const handleSelectExample = (example: typeof EXAMPLE_SAMPLES[0]) => {
    setContent(example.content);
    setSenderKnown(example.senderKnown);
    setInputMode('message');
    setValidationError(null);
    setAnalysisResult(null);
  };

  const handleClear = () => {
    setContent('');
    setUrlInput('');
    setSenderKnown(false);
    setValidationError(null);
    setApiError(null);
    setAnalysisResult(null);
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setValidationError(null);
    setApiError(null);

    if (inputMode === 'message' && !content.trim()) {
      setValidationError('Please enter or paste the message text you wish to analyze.');
      return;
    }

    if (inputMode === 'url' && !urlInput.trim()) {
      setValidationError('Please enter the URL or web domain to analyze.');
      return;
    }

    if (inputMode === 'url') {
      const urlPattern = /^(https?:\/\/)?([\da-z.-]+)\.([a-z.]{2,6})([/\w .-]*)*\/?$/i;
      if (!urlPattern.test(urlInput.trim())) {
        setValidationError('Please enter a valid web address (e.g., https://example.com/login).');
        return;
      }
    }

    if (!isAuthenticated) {
      setApiError('Authentication required: You must be signed in to submit live analysis requests to the backend engine.');
      return;
    }

    setIsLoading(true);

    try {
      const result = await fraudService.checkMessage({
        type: inputMode === 'url' ? 'Url' : 'Message',
        content: inputMode === 'message' ? content : undefined,
        url: inputMode === 'url' ? urlInput : undefined,
        senderKnown,
      });

      setAnalysisResult(result);
    } catch (err: any) {
      setApiError(err.message || 'Analysis request failed. Please check your connection and retry.');
    } finally {
      setIsLoading(false);
    }
  };

  const getRiskBadge = (level: string) => {
    const norm = level.toLowerCase();
    if (norm === 'high' || norm === 'critical') {
      return (
        <span className="px-3 py-1 bg-red-100 text-red-900 border border-red-300 rounded-full text-xs font-semibold flex items-center gap-1.5">
          <span className="w-2 h-2 rounded-full bg-red-600" />
          High Risk &bull; Deception Indicators Detected
        </span>
      );
    }
    if (norm === 'medium' || norm === 'moderate') {
      return (
        <span className="px-3 py-1 bg-amber-100 text-amber-900 border border-amber-300 rounded-full text-xs font-semibold flex items-center gap-1.5">
          <span className="w-2 h-2 rounded-full bg-amber-600" />
          Medium Risk &bull; Caution Advised
        </span>
      );
    }
    return (
      <span className="px-3 py-1 bg-emerald-100 text-emerald-900 border border-emerald-300 rounded-full text-xs font-semibold flex items-center gap-1.5">
        <span className="w-2 h-2 rounded-full bg-emerald-600" />
        Low Risk &bull; No High-Risk Urgency Signals
      </span>
    );
  };

  return (
    <div className="pt-28 pb-24 max-w-4xl mx-auto px-6 sm:px-10">
      {/* Header */}
      <div className="mb-10">
        <span className="text-xs uppercase tracking-widest text-[#191919]/50 font-semibold block mb-2">
          Automated Threat Analysis
        </span>
        <h1 className="text-3xl sm:text-4xl font-serif text-[#191919]">
          Scam & Phishing Checker
        </h1>
        <p className="text-sm text-[#191919]/70 mt-2 leading-relaxed">
          Evaluate unexpected messages, emergency payment demands, and suspicious URLs against verified deception patterns.
        </p>
      </div>

      {/* Unauthenticated Alert Banner */}
      {!isAuthenticated && (
        <div className="mb-8 p-5 bg-[#F4F3F3] border border-gray-200 rounded-xl flex flex-col sm:flex-row items-start sm:items-center justify-between gap-4">
          <div>
            <h4 className="text-xs font-bold text-[#191919] uppercase tracking-wider">
              Account Required For Live Engine
            </h4>
            <p className="text-xs text-[#191919]/70 mt-1 leading-relaxed">
              To prevent automated abuse and protect threat intelligence pipelines, live API evaluations connect to <code className="font-mono text-[#191919]">/api/fraud/check</code> and require an active user session.
            </p>
          </div>
          <div className="flex items-center space-x-2 shrink-0">
            <Link
              to="/login"
              className="text-xs font-medium px-4 py-2 bg-[#191919] text-white rounded-full hover:bg-black/85 transition-colors"
            >
              Sign In
            </Link>
            <Link
              to="/register"
              className="text-xs font-medium px-4 py-2 border border-gray-300 rounded-full hover:bg-white transition-colors"
            >
              Register
            </Link>
          </div>
        </div>
      )}

      {/* Example Prompts */}
      <div className="mb-8 p-5 bg-white border border-gray-200 rounded-xl">
        <span className="text-[11px] uppercase tracking-wider font-semibold text-[#191919]/50 block mb-2">
          Test With Common Sample Scenarios
        </span>
        <div className="flex flex-wrap gap-2">
          {EXAMPLE_SAMPLES.map((ex, idx) => (
            <button
              key={idx}
              type="button"
              onClick={() => handleSelectExample(ex)}
              className="text-xs px-3 py-1.5 bg-[#F4F3F3] hover:bg-[#EAEAEA] text-[#191919] rounded-lg border border-gray-100 transition-colors text-left"
            >
              {ex.title}
            </button>
          ))}
        </div>
      </div>

      {/* Input Form */}
      <form onSubmit={handleSubmit} className="bg-white border border-gray-200 rounded-2xl p-6 sm:p-8 shadow-sm space-y-6">
        {/* Mode Selector */}
        <div className="flex space-x-3 border-b border-gray-100 pb-4">
          <button
            type="button"
            onClick={() => {
              setInputMode('message');
              setValidationError(null);
            }}
            className={`text-xs font-medium px-3.5 py-1.5 rounded-full transition-colors ${
              inputMode === 'message'
                ? 'bg-[#191919] text-white'
                : 'bg-gray-100 text-[#191919]/70 hover:bg-gray-200'
            }`}
          >
            Message Content / SMS / Email
          </button>
          <button
            type="button"
            onClick={() => {
              setInputMode('url');
              setValidationError(null);
            }}
            className={`text-xs font-medium px-3.5 py-1.5 rounded-full transition-colors ${
              inputMode === 'url'
                ? 'bg-[#191919] text-white'
                : 'bg-gray-100 text-[#191919]/70 hover:bg-gray-200'
            }`}
          >
            Suspicious URL / Link
          </button>
        </div>

        {/* Input Area */}
        {inputMode === 'message' ? (
          <div>
            <label
              htmlFor="message-content"
              className="block text-xs font-semibold text-[#191919]/70 uppercase tracking-wider mb-2"
            >
              Suspicious Message Text
            </label>
            <textarea
              id="message-content"
              rows={6}
              value={content}
              onChange={(e) => setContent(e.target.value)}
              placeholder="Paste the SMS, email text, or chat message here..."
              maxLength={4000}
              className="w-full text-sm p-4 border border-gray-200 rounded-xl focus:outline-none focus:ring-2 focus:ring-[#191919] focus:border-transparent font-sans resize-y"
            />
            <div className="flex justify-between items-center text-[11px] text-[#191919]/40 mt-1">
              <span>Limit: up to 4,000 characters</span>
              <span>{content.length} / 4000</span>
            </div>
          </div>
        ) : (
          <div>
            <label
              htmlFor="url-content"
              className="block text-xs font-semibold text-[#191919]/70 uppercase tracking-wider mb-2"
            >
              Suspicious URL or Domain
            </label>
            <input
              id="url-content"
              type="text"
              value={urlInput}
              onChange={(e) => setUrlInput(e.target.value)}
              placeholder="e.g. http://secure-banking-login.update-alert.xyz"
              className="w-full text-sm p-3.5 border border-gray-200 rounded-xl focus:outline-none focus:ring-2 focus:ring-[#191919] focus:border-transparent font-sans"
            />
            <p className="text-[11px] text-[#191919]/50 mt-1.5">
              The engine inspects structural indicators (e.g. lookalikes, IP redirects, unverified subdomains). We never execute or crawl malicious payloads directly in your browser.
            </p>
          </div>
        )}

        {/* Sender Checkbox */}
        <div className="flex items-center space-x-2 text-xs text-[#191919]/70">
          <input
            id="sender-known"
            type="checkbox"
            checked={senderKnown}
            onChange={(e) => setSenderKnown(e.target.checked)}
            className="rounded border-gray-300 text-[#191919] focus:ring-[#191919]"
          />
          <label htmlFor="sender-known" className="cursor-pointer">
            The sender is someone I have communicated with legitimately before
          </label>
        </div>

        {/* Validation Error */}
        {validationError && (
          <p className="text-xs text-red-600 font-medium" role="alert">
            {validationError}
          </p>
        )}

        {/* Privacy Note */}
        <div className="p-3 bg-gray-50 rounded-lg text-[11px] text-[#191919]/55 leading-relaxed">
          <strong>Privacy note:</strong> Do not include private credentials or sensitive personal identification numbers. Submitted text is parsed solely to identify generic fraud markers and generate safety explanations.
        </div>

        {/* Buttons */}
        <div className="flex items-center justify-between pt-2">
          <button
            type="button"
            onClick={handleClear}
            className="text-xs font-medium text-[#191919]/60 hover:text-[#191919] transition-colors"
          >
            Clear Form
          </button>
          <button
            type="submit"
            disabled={isLoading}
            className="bg-[#191919] text-white text-xs sm:text-sm font-medium px-6 py-2.5 rounded-full hover:bg-black/85 disabled:opacity-50 transition-colors shadow-sm flex items-center space-x-2"
          >
            {isLoading ? <span>Analyzing Indicators...</span> : <span>Analyze for Fraud Signals</span>}
          </button>
        </div>
      </form>

      {/* Loading State */}
      {isLoading && (
        <div className="mt-8">
          <LoadingSpinner label="Evaluating message against rule-based threat engine..." />
        </div>
      )}

      {/* API Error State */}
      {apiError && (
        <ErrorMessage
          title="Analysis Could Not Proceed"
          message={apiError}
          onRetry={() => handleSubmit({ preventDefault: () => {} } as any)}
        />
      )}

      {/* Verified Backend Results */}
      {analysisResult && (
        <div className="mt-10 bg-white border border-gray-200 rounded-2xl p-6 sm:p-8 shadow-sm space-y-6">
          <div className="flex flex-col sm:flex-row sm:items-center justify-between pb-6 border-b border-gray-100 gap-3">
            <div>
              <span className="text-xs uppercase tracking-wider font-semibold text-[#191919]/50 block mb-1">
                Engine Risk Assessment
              </span>
              <h3 className="text-2xl font-serif text-[#191919]">
                Score: {analysisResult.score} / 100
              </h3>
            </div>
            <div>
              {getRiskBadge(analysisResult.riskLevel)}
            </div>
          </div>

          {/* Indicators Section */}
          <div>
            <h4 className="text-xs uppercase tracking-wider font-semibold text-[#191919]/60 mb-3">
              Detected Risk Indicators ({analysisResult.indicators.length})
            </h4>
            {analysisResult.indicators.length === 0 ? (
              <p className="text-xs text-[#191919]/70 bg-gray-50 p-4 rounded-lg">
                No high-risk keywords, artificial urgency patterns, or suspicious URL structures were detected.
              </p>
            ) : (
              <div className="space-y-3">
                {analysisResult.indicators.map((ind, i) => (
                  <div key={i} className="p-4 bg-[#F4F3F3] rounded-xl border border-gray-100 text-xs space-y-1">
                    <div className="flex items-center justify-between">
                      <span className="font-semibold text-[#191919]">{ind.title}</span>
                      <span className="font-mono text-[10px] bg-white px-2 py-0.5 rounded border border-gray-200 text-[#191919]/70">
                        Weight +{ind.score}
                      </span>
                    </div>
                    <p className="text-[#191919]/70 leading-relaxed">{ind.explanation}</p>
                  </div>
                ))}
              </div>
            )}
          </div>

          {/* Recommendations Section */}
          <div>
            <h4 className="text-xs uppercase tracking-wider font-semibold text-[#191919]/60 mb-3">
              Actionable Safety Recommendations
            </h4>
            <div className="space-y-2">
              {analysisResult.recommendations.map((rec, i) => (
                <div key={i} className="p-3.5 bg-emerald-50/70 border border-emerald-200/80 rounded-xl text-xs flex items-start space-x-2.5">
                  <span className="text-emerald-700 font-bold shrink-0">&bull;</span>
                  <span className="text-emerald-950 leading-relaxed">{rec.message}</span>
                </div>
              ))}
            </div>
          </div>

          {/* Official Engine Disclaimer */}
          <div className="pt-4 border-t border-gray-100 text-[11px] text-[#191919]/50 italic leading-relaxed">
            <strong>Platform Disclaimer:</strong> {analysisResult.disclaimer || 'Automated results provide heuristic pattern recognition and do not constitute legal or investigative guarantee. Always verify unexpected communications directly with trusted organizations.'}
          </div>
        </div>
      )}
    </div>
  );
};
