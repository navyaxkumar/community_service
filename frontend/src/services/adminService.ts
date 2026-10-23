import { api } from './apiClient';
import { AdminDashboardStats } from '../types/admin';

export const adminService = {
  getStats: async (): Promise<AdminDashboardStats> => {
    return api.get<AdminDashboardStats>('/admin/stats');
  },
};
