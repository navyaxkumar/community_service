import { api } from './apiClient';
import { Badge, SaveBadgeRequest } from '../types/badge';

export const badgeService = {
  getActiveBadges: async (): Promise<Badge[]> => {
    return api.get<Badge[]>('/badges');
  },

  getAllForManagement: async (): Promise<Badge[]> => {
    return api.get<Badge[]>('/badges/manage');
  },

  getBadgeById: async (id: number): Promise<Badge> => {
    return api.get<Badge>(`/badges/${id}`);
  },

  createBadge: async (badge: SaveBadgeRequest): Promise<Badge> => {
    return api.post<Badge>('/badges', badge);
  },

  updateBadge: async (id: number, badge: SaveBadgeRequest): Promise<Badge> => {
    return api.put<Badge>(`/badges/${id}`, badge);
  },
};
