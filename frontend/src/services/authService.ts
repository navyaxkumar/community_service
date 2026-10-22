import { api } from './apiClient';
import {
  LoginRequest,
  LoginResponse,
  RegisterRequest,
  UpdateUserRequest,
  User,
} from '../types/auth';

export const authService = {
  login: async (credentials: LoginRequest): Promise<LoginResponse> => {
    return api.post<LoginResponse>('/auth/login', credentials);
  },

  register: async (data: RegisterRequest): Promise<User> => {
    return api.post<User>('/auth/register', data);
  },

  getCurrentUser: async (): Promise<User> => {
    return api.get<User>('/users/me');
  },

  updateCurrentUser: async (data: UpdateUserRequest): Promise<User> => {
    return api.put<User>('/users/me', data);
  },
};
