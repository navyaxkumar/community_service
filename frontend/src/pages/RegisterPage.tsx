import React, { useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';
import { ErrorMessage } from '../components/common/ErrorMessage';

export const RegisterPage: React.FC = () => {
  const { register, isAuthenticated } = useAuth();
  const navigate = useNavigate();

  const [name, setName] = useState('');
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [confirmPassword, setConfirmPassword] = useState('');

  const [validationError, setValidationError] = useState<string | null>(null);
  const [serverError, setServerError] = useState<string | null>(null);
  const [isLoading, setIsLoading] = useState(false);

  React.useEffect(() => {
    if (isAuthenticated) {
      navigate('/dashboard', { replace: true });
    }
  }, [isAuthenticated, navigate]);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setValidationError(null);
    setServerError(null);

    if (!name.trim()) {
      setValidationError('Please enter your full name.');
      return;
    }

    if (!email.trim() || !email.includes('@')) {
      setValidationError('Please enter a valid email address.');
      return;
    }

    if (password.length < 6) {
      setValidationError('Password must be at least 6 characters long.');
      return;
    }

    if (password !== confirmPassword) {
      setValidationError('Passwords do not match.');
      return;
    }

    setIsLoading(true);

    try {
      await register({
        name: name.trim(),
        email: email.trim(),
        password,
      });
      navigate('/dashboard', { replace: true });
    } catch (err: any) {
      setServerError(err.message || 'Registration failed. A user with this email may already exist.');
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
            DigitalShield Registration
          </span>
          <h1 className="text-2xl sm:text-3xl font-serif text-[#191919]">
            Create your account
          </h1>
          <p className="text-xs text-[#191919]/65">
            Start tracking fraud awareness lessons and test your detection instincts.
          </p>
        </div>

        {/* Server Error Alert */}
        {serverError && (
          <ErrorMessage
            title="Registration Error"
            message={serverError}
          />
        )}

        {/* Form */}
        <form onSubmit={handleSubmit} className="space-y-4">
          <div>
            <label
              htmlFor="name-input"
              className="block text-xs font-semibold text-[#191919]/70 uppercase tracking-wider mb-1.5"
            >
              Full Name
            </label>
            <input
              id="name-input"
              type="text"
              autoComplete="name"
              value={name}
              onChange={(e) => setName(e.target.value)}
              placeholder="e.g. John Doe"
              required
              className="w-full text-sm p-3.5 border border-gray-200 rounded-xl focus:outline-none focus:ring-2 focus:ring-[#191919] font-sans"
            />
          </div>

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
            <label
              htmlFor="password-input"
              className="block text-xs font-semibold text-[#191919]/70 uppercase tracking-wider mb-1.5"
            >
              Password (min. 6 characters)
            </label>
            <input
              id="password-input"
              type="password"
              autoComplete="new-password"
              value={password}
              onChange={(e) => setPassword(e.target.value)}
              placeholder="••••••••"
              required
              className="w-full text-sm p-3.5 border border-gray-200 rounded-xl focus:outline-none focus:ring-2 focus:ring-[#191919] font-sans"
            />
          </div>

          <div>
            <label
              htmlFor="confirm-password-input"
              className="block text-xs font-semibold text-[#191919]/70 uppercase tracking-wider mb-1.5"
            >
              Confirm Password
            </label>
            <input
              id="confirm-password-input"
              type="password"
              autoComplete="new-password"
              value={confirmPassword}
              onChange={(e) => setConfirmPassword(e.target.value)}
              placeholder="••••••••"
              required
              className="w-full text-sm p-3.5 border border-gray-200 rounded-xl focus:outline-none focus:ring-2 focus:ring-[#191919] font-sans"
            />
          </div>

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
            {isLoading ? 'Creating Account...' : 'Register Account'}
          </button>
        </form>

        {/* Footer Link */}
        <div className="pt-4 border-t border-gray-100 text-center text-xs text-[#191919]/60">
          Already have an account?{' '}
          <Link to="/login" className="font-semibold text-[#191919] hover:underline">
            Sign In here
          </Link>
        </div>
      </div>
    </div>
  );
};
