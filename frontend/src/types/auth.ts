export interface User {
  id: number;
  name: string;
  email: string;
  role: 'User' | 'Admin' | string;
  createdAt: string;
  updatedAt?: string | null;
  isActive: boolean;
}

export interface LoginResponse {
  accessToken: string;
  expiresAt: string;
  user: User;
}

export interface LoginRequest {
  email: string;
  password: string;
}

export interface RegisterRequest {
  name: string;
  email: string;
  password: string;
}

export interface UpdateUserRequest {
  name: string;
  email: string;
}

export interface AuthState {
  user: User | null;
  token: string | null;
  isAuthenticated: boolean;
  isAdmin: boolean;
  isLoading: boolean;
}
