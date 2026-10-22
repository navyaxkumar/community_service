import React, { useState } from 'react';
import { Link, useNavigate, useLocation } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';
import { ErrorMessage } from '../components/common/ErrorMessage';

export const LoginPage: React.FC = () => {
  const { login, isAuthenticated } = useAuth();
  const navigate = useNavigate();
  const location = useLocation();

  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [showPassword, setShowPassword] = useState(false);
  const [showForgotNotice, setShowForgotNotice] = useState(false);

  const [validationError, setValidationError] = useState<string | null>(null);
  const [serverError, setServerError] = useState<string | null>(null);
  const [isLoading, setIsLoading] = useState(false);

  // If already authenticated, redirect
  const from = (location.state as any)?.from?.pathname || '/dashboard';

  React.useEffect(() => {
    if (isAuthenticated) {
      navigate(from, { replace: true });
    }
  }, [isAuthenticated, navigate, from]);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setValidationError(null);
    setServerError(null);

    if (!email.trim() || !password) {
      setValidationError('Please enter both email address and password.');
      return;
    }

    setIsLoading(true);

    try {
      await login({ email: email.trim(), password });
      navigate(from, { replace: true });
    } catch (err: any) {
      setServerError(err.message || 'Invalid email or password. Please verify your credentials.');
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <div className="pt-28 pb-24 max-w-md mx-auto px-6">
      <div className="bg-white border border-gray-200 rounded-3xl p-8 sm:p-10 shadow-sm space-y-6">
        {/* Header */}
        <div className="text-center space-y-1">
          <span className="text-[11px] uppercase tracking-widest text-[#191919]/50 font-semibold block">
            DigitalShield Authentication
          </span>
          <h1 className="text-2xl sm:text-3xl font-serif text-[#191919]">
            Sign in to your account
          </h1>
          <p className="text-xs text-[#191919]/65">
            Access your verified learning history, quiz scores, and saved progress.
          </p>
        </div>

        {/* Server Error Alert */}
        {serverError && (
          <ErrorMessage
            title="Authentication Failed"
            message={serverError}
          />
        )}

        {/* Form */}
        <form onSubmit={handleSubmit} className="space-y-4">
          <div>
            <label
              htmlFor="email-input"
              className="block text-xs font-semibold text-[#191919]/70 uppercase tracking-wider mb-1.5"
            >
              Email Address
            </label>
            <input
              id="email-input"
              type="email"
              autoComplete="email"
              value={email}
              onChange={(e) => setEmail(e.target.value)}
              placeholder="name@example.com"
              required
              className="w-full text-sm p-3.5 border border-gray-200 rounded-xl focus:outline-none focus:ring-2 focus:ring-[#191919] font-sans"
            />
          </div>

          <div>
            <div className="flex items-center justify-between mb-1.5">
              <label
                htmlFor="password-input"
                className="text-xs font-semibold text-[#191919]/70 uppercase tracking-wider"
              >
                Password
              </label>
              <button
                type="button"
                onClick={() => setShowForgotNotice(!showForgotNotice)}
                className="text-[11px] text-[#191919]/60 hover:text-[#191919] underline"
              >
                Forgot password?
              </button>
            </div>
            <div className="relative">
              <input
                id="password-input"
                type={showPassword ? 'text' : 'password'}
                autoComplete="current-password"
                value={password}
                onChange={(e) => setPassword(e.target.value)}
                placeholder="••••••••"
                required
                className="w-full text-sm p-3.5 pr-10 border border-gray-200 rounded-xl focus:outline-none focus:ring-2 focus:ring-[#191919] font-sans"
              />
              <button
                type="button"
                onClick={() => setShowPassword(!showPassword)}
                className="absolute right-3 top-3.5 text-xs text-gray-400 hover:text-gray-700"
              >
                {showPassword ? 'Hide' : 'Show'}
              </button>
            </div>
          </div>

          {/* Honest Notice for Forgot Password */}
          {showForgotNotice && (
            <div className="p-3 bg-gray-50 border border-gray-200 rounded-xl text-xs text-[#191919]/70 leading-relaxed">
              <strong>Account Recovery Notice:</strong> Self-service automated password reset is currently disabled to prevent credential hijacking. To reset your credentials, please contact your system administrator.
            </div>
          )}

          {validationError && (
            <p className="text-xs text-red-600 font-medium" role="alert">
              {validationError}
            </p>
          )}

          <button
            type="submit"
            disabled={isLoading}
            className="w-full bg-[#191919] text-white text-sm font-medium py-3 rounded-full hover:bg-black/85 disabled:opacity-50 transition-colors shadow-sm mt-2"
          >
            {isLoading ? 'Verifying Credentials...' : 'Sign In'}
          </button>
        </form>

        {/* Footer Link */}
        <div className="pt-4 border-t border-gray-100 text-center text-xs text-[#191919]/60">
          Do not have an account yet?{' '}
          <Link to="/register" className="font-semibold text-[#191919] hover:underline">
            Register for free
          </Link>
        </div>
      </div>
    </div>
  );
};
