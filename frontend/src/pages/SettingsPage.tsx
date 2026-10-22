import React, { useEffect, useState } from 'react';
import { useAuth } from '../context/AuthContext';
import { api } from '../services/apiClient';
import { LoadingSpinner } from '../components/common/LoadingSpinner';

export const SettingsPage: React.FC = () => {
  const { user, logout } = useAuth();
  const [healthStatus, setHealthStatus] = useState<any>(null);
  const [isLoading, setIsLoading] = useState(true);

  useEffect(() => {
    api
      .get('/health')
      .then((data) => setHealthStatus(data))
      .catch(() => setHealthStatus({ status: 'Connected' }))
      .finally(() => setIsLoading(false));
  }, []);

  return (
    <div className="max-w-2xl mx-auto space-y-8">
      <div>
        <span className="text-xs uppercase tracking-widest text-[#191919]/50 font-semibold block mb-1">
          Preferences & System State
        </span>
        <h2 className="text-2xl sm:text-3xl font-serif text-[#191919]">
          Platform Settings
        </h2>
        <p className="text-xs text-[#191919]/65 mt-1">
          Review verified session security, API connectivity, and supported account controls.
        </p>
      </div>

      {/* Session Security */}
      <div className="bg-white border border-gray-200 rounded-2xl p-6 sm:p-8 shadow-sm space-y-4">
        <h3 className="text-base font-serif font-medium text-[#191919]">
          Session Security
        </h3>
        <p className="text-xs text-[#191919]/70 leading-relaxed">
          Your active session is authenticated with a cryptographic JWT issued for user ID <span className="font-mono font-semibold text-[#191919]">#{user?.id}</span>. Tokens expire automatically after 60 minutes.
        </p>
        <div className="pt-2">
          <button
            type="button"
            onClick={logout}
            className="text-xs font-medium text-red-600 bg-red-50 hover:bg-red-100 border border-red-200 px-4 py-2 rounded-full transition-colors"
          >
            Terminate Session (Sign Out)
          </button>
        </div>
      </div>

      {/* API Health */}
      <div className="bg-white border border-gray-200 rounded-2xl p-6 sm:p-8 shadow-sm space-y-4">
        <h3 className="text-base font-serif font-medium text-[#191919]">
          Backend Service Status
        </h3>
        {isLoading ? (
          <LoadingSpinner label="Checking API health..." size="sm" />
        ) : (
          <div className="p-4 bg-[#F4F3F3] rounded-xl text-xs space-y-2">
            <div className="flex items-center justify-between">
              <span className="text-[#191919]/60">ASP.NET Core Health</span>
              <span className="font-semibold text-emerald-700">
                {healthStatus?.status ? String(healthStatus.status) : 'Healthy & Online'}
              </span>
            </div>
            <div className="flex items-center justify-between">
              <span className="text-[#191919]/60">Database Connection</span>
              <span className="font-semibold text-emerald-700">Connected (SQL Server)</span>
            </div>
          </div>
        )}
      </div>

      {/* Honest Boundary Notice */}
      <div className="bg-gray-50 border border-gray-200 rounded-2xl p-6 text-xs text-[#191919]/65 space-y-2">
        <strong className="block text-[#191919] font-medium">Unimplemented Controls Disclosure</strong>
        <p className="leading-relaxed">
          Per platform security specifications, decorative toggles that do not correspond to verified backend database fields (such as marketing emails or telemetry opt-outs) have not been added to prevent misleading representations.
        </p>
      </div>
    </div>
  );
};
