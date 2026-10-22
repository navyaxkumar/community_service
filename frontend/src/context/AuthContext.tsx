import React, { createContext, useContext, useEffect, useState } from 'react';
import { LoginRequest, RegisterRequest, User } from '../types/auth';
import { authService } from '../services/authService';

interface AuthContextType {
  user: User | null;
  token: string | null;
  isAuthenticated: boolean;
  isAdmin: boolean;
  isLoading: boolean;
  login: (credentials: LoginRequest) => Promise<void>;
  register: (data: RegisterRequest) => Promise<void>;
  logout: () => void;
  refreshUser: () => Promise<void>;
}

const AuthContext = createContext<AuthContextType | undefined>(undefined);

export const AuthProvider: React.FC<{ children: React.ReactNode }> = ({ children }) => {
  const [user, setUser] = useState<User | null>(() => {
    try {
      const stored = localStorage.getItem('ds_user');
      return stored ? JSON.parse(stored) : null;
    } catch {
      return null;
    }
  });

  const [token, setToken] = useState<string | null>(() => {
    return localStorage.getItem('ds_token');
  });

  const [isLoading, setIsLoading] = useState<boolean>(true);

  const refreshUser = async () => {
    if (!localStorage.getItem('ds_token')) {
      setUser(null);
      setIsLoading(false);
      return;
    }

    try {
      const currentUser = await authService.getCurrentUser();
      setUser(currentUser);
      localStorage.setItem('ds_user', JSON.stringify(currentUser));
    } catch {
      // Token might be invalid or expired
      setUser(null);
      setToken(null);
      localStorage.removeItem('ds_token');
      localStorage.removeItem('ds_user');
    } finally {
      setIsLoading(false);
    }
  };

  useEffect(() => {
    refreshUser();

    const handleExpired = () => {
      setUser(null);
      setToken(null);
    };

    window.addEventListener('ds_auth_expired', handleExpired);
    return () => window.removeEventListener('ds_auth_expired', handleExpired);
  }, []);

  const login = async (credentials: LoginRequest) => {
    setIsLoading(true);
    try {
      const response = await authService.login(credentials);
      setToken(response.accessToken);
      setUser(response.user);
      localStorage.setItem('ds_token', response.accessToken);
      localStorage.setItem('ds_user', JSON.stringify(response.user));
    } finally {
      setIsLoading(false);
    }
  };

  const register = async (data: RegisterRequest) => {
    setIsLoading(true);
    try {
      await authService.register(data);
      // Auto-login after successful registration
      const loginRes = await authService.login({ email: data.email, password: data.password });
      setToken(loginRes.accessToken);
      setUser(loginRes.user);
      localStorage.setItem('ds_token', loginRes.accessToken);
      localStorage.setItem('ds_user', JSON.stringify(loginRes.user));
    } finally {
      setIsLoading(false);
    }
  };

  const logout = () => {
    setUser(null);
    setToken(null);
    localStorage.removeItem('ds_token');
    localStorage.removeItem('ds_user');
  };

  const isAuthenticated = !!token && !!user;
  const isAdmin = user?.role === 'Admin';

  return (
    <AuthContext.Provider
      value={{
        user,
        token,
        isAuthenticated,
        isAdmin,
        isLoading,
        login,
        register,
        logout,
        refreshUser,
      }}
    >
      {children}
    </AuthContext.Provider>
  );
};

export const useAuth = (): AuthContextType => {
  const context = useContext(AuthContext);
  if (!context) {
    throw new Error('useAuth must be used within an AuthProvider');
  }
  return context;
};
