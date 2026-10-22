import React, { useEffect, useState } from 'react';
import { useAuth } from '../context/AuthContext';
import { authService } from '../services/authService';
import { User } from '../types/auth';
import { LoadingSpinner } from '../components/common/LoadingSpinner';
import { ErrorMessage } from '../components/common/ErrorMessage';

export const ProfilePage: React.FC = () => {
  const { user, refreshUser } = useAuth();

  const [profile, setProfile] = useState<User | null>(user);
  const [name, setName] = useState(user?.name || '');
  const [email, setEmail] = useState(user?.email || '');

  const [isLoading, setIsLoading] = useState(!user);
  const [isSaving, setIsSaving] = useState(false);
  const [successMessage, setSuccessMessage] = useState<string | null>(null);
  const [errorMessage, setErrorMessage] = useState<string | null>(null);

  useEffect(() => {
    authService
      .getCurrentUser()
      .then((data) => {
        setProfile(data);
        setName(data.name);
        setEmail(data.email);
      })
      .catch((err) => {
        setErrorMessage(err.message || 'Failed to load profile details.');
      })
      .finally(() => setIsLoading(false));
  }, []);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setSuccessMessage(null);
    setErrorMessage(null);

    if (!name.trim()) {
      setErrorMessage('Name cannot be empty.');
      return;
    }

    if (!email.trim() || !email.includes('@')) {
      setErrorMessage('Please provide a valid email address.');
      return;
    }

    setIsSaving(true);

    try {
      const updated = await authService.updateCurrentUser({
        name: name.trim(),
        email: email.trim(),
      });
      setProfile(updated);
      await refreshUser();
      setSuccessMessage('Profile information successfully updated.');
    } catch (err: any) {
      setErrorMessage(err.message || 'Failed to update profile.');
    } finally {
      setIsSaving(false);
    }
  };

  if (isLoading) {
    return (
      <div className="py-12">
        <LoadingSpinner label="Loading account details..." />
      </div>
    );
  }

  return (
    <div className="max-w-2xl mx-auto space-y-8">
      {/* Header */}
      <div>
        <span className="text-xs uppercase tracking-widest text-[#191919]/50 font-semibold block mb-1">
          Account Settings
        </span>
        <h2 className="text-2xl sm:text-3xl font-serif text-[#191919]">
          Personal Profile & Identity
        </h2>
        <p className="text-xs text-[#191919]/65 mt-1">
          Update your public name and notification email address tied to your verified learning record.
        </p>
      </div>

      {successMessage && (
        <div className="p-4 bg-emerald-50 border border-emerald-200 text-emerald-800 text-xs rounded-xl flex items-center space-x-2">
          <span>&check;</span>
          <span>{successMessage}</span>
        </div>
      )}

      {errorMessage && (
        <ErrorMessage
          title="Profile Update Error"
          message={errorMessage}
        />
      )}

      {/* Profile Form */}
      <div className="bg-white border border-gray-200 rounded-2xl p-6 sm:p-8 shadow-sm">
        <form onSubmit={handleSubmit} className="space-y-5">
          <div>
            <label
              htmlFor="profile-name"
              className="block text-xs font-semibold text-[#191919]/70 uppercase tracking-wider mb-1.5"
            >
              Full Name
            </label>
            <input
              id="profile-name"
              type="text"
              value={name}
              onChange={(e) => setName(e.target.value)}
              required
              className="w-full text-sm p-3.5 border border-gray-200 rounded-xl focus:outline-none focus:ring-2 focus:ring-[#191919] font-sans"
            />
          </div>

          <div>
            <label
              htmlFor="profile-email"
              className="block text-xs font-semibold text-[#191919]/70 uppercase tracking-wider mb-1.5"
            >
              Email Address
            </label>
            <input
              id="profile-email"
              type="email"
              value={email}
              onChange={(e) => setEmail(e.target.value)}
              required
              className="w-full text-sm p-3.5 border border-gray-200 rounded-xl focus:outline-none focus:ring-2 focus:ring-[#191919] font-sans"
            />
          </div>

          <div className="pt-2 flex justify-end">
            <button
              type="submit"
              disabled={isSaving}
              className="bg-[#191919] text-white text-xs font-medium px-6 py-2.5 rounded-full hover:bg-black/85 disabled:opacity-50 transition-colors shadow-sm"
            >
              {isSaving ? 'Saving Changes...' : 'Save Profile Changes'}
            </button>
          </div>
        </form>
      </div>

      {/* Account Security Information Card */}
      <div className="bg-white border border-gray-200 rounded-2xl p-6 sm:p-8 shadow-sm space-y-4">
        <h3 className="text-base font-serif font-medium text-[#191919]">
          Account & Security Metadata
        </h3>
        <div className="grid grid-cols-1 sm:grid-cols-2 gap-4 text-xs">
          <div className="p-3 bg-[#F4F3F3] rounded-xl">
            <span className="text-[#191919]/50 block">Account Role</span>
            <span className="font-semibold text-[#191919] mt-0.5 block">{profile?.role || 'User'}</span>
          </div>
          <div className="p-3 bg-[#F4F3F3] rounded-xl">
            <span className="text-[#191919]/50 block">Account Status</span>
            <span className="font-semibold text-emerald-700 mt-0.5 block">
              {profile?.isActive ? 'Active & Verified' : 'Inactive'}
            </span>
          </div>
          <div className="p-3 bg-[#F4F3F3] rounded-xl">
            <span className="text-[#191919]/50 block">Member Since</span>
            <span className="font-semibold text-[#191919] mt-0.5 block">
              {profile?.createdAt ? new Date(profile.createdAt).toLocaleDateString() : 'N/A'}
            </span>
          </div>
          <div className="p-3 bg-[#F4F3F3] rounded-xl">
            <span className="text-[#191919]/50 block">Last Profile Update</span>
            <span className="font-semibold text-[#191919] mt-0.5 block">
              {profile?.updatedAt ? new Date(profile.updatedAt).toLocaleDateString() : 'Never'}
            </span>
          </div>
        </div>

        <div className="pt-2 text-[11px] text-[#191919]/55 leading-relaxed">
          <strong>Security note:</strong> Passwords are protected using ASP.NET Core cryptographic hashing algorithms and are never transmitted in cleartext or readable by clients.
        </div>
      </div>
    </div>
  );
};
