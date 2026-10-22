import { api } from './apiClient';
import { Badge } from '../types/badge';

export const badgeService = {
  getActiveBadges: async (): Promise<Badge[]> => {
    return api.get<Badge[]>('/badges');
  },

  getBadgeById: async (id: number): Promise<Badge> => {
    return api.get<Badge>(`/badges/${id}`);
  },
};
