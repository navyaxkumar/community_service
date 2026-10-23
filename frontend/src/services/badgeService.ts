import { api } from './apiClient';
import {
  Badge,
  CreateBadgeRequest,
  UpdateBadgeRequest,
} from '../types/badge';

export const badgeService = {
  getActiveBadges: async (): Promise<Badge[]> => {
    return api.get<Badge[]>('/badges');
  },

  getAllBadgesAdmin: async (): Promise<Badge[]> => {
    return api.get<Badge[]>('/badges/admin/all');
  },

  getBadgeById: async (id: number): Promise<Badge> => {
    return api.get<Badge>(`/badges/${id}`);
  },

  createBadge: async (badge: CreateBadgeRequest): Promise<Badge> => {
    return api.post<Badge>('/badges', badge);
  },

  updateBadge: async (id: number, badge: UpdateBadgeRequest): Promise<Badge> => {
    return api.put<Badge>(`/badges/${id}`, badge);
  },

  deleteBadge: async (id: number): Promise<void> => {
    return api.delete<void>(`/badges/${id}`);
  },
};
