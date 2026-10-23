import { api } from './apiClient';
import {
  CreateScenarioRequest,
  Scenario,
  UpdateScenarioRequest,
} from '../types/badge';

export const scenarioService = {
  getPublishedScenarios: async (audience?: string): Promise<Scenario[]> => {
    const query = audience && audience !== 'All' ? `?audience=${encodeURIComponent(audience)}` : '';
    return api.get<Scenario[]>(`/scenarios${query}`);
  },

  getAllScenariosAdmin: async (): Promise<Scenario[]> => {
    return api.get<Scenario[]>('/scenarios/admin/all');
  },

  getScenarioById: async (id: number): Promise<Scenario> => {
    return api.get<Scenario>(`/scenarios/${id}`);
  },

  getScenarioByIdAdmin: async (id: number): Promise<Scenario> => {
    return api.get<Scenario>(`/scenarios/admin/${id}`);
  },

  getScenariosByCategory: async (categoryId: number): Promise<Scenario[]> => {
    return api.get<Scenario[]>(`/scenarios/category/${categoryId}`);
  },

  createScenario: async (scenario: CreateScenarioRequest): Promise<Scenario> => {
    return api.post<Scenario>('/scenarios', scenario);
  },

  updateScenario: async (id: number, scenario: UpdateScenarioRequest): Promise<Scenario> => {
    return api.put<Scenario>(`/scenarios/${id}`, scenario);
  },

  setPublishStatus: async (id: number, isPublished: boolean): Promise<Scenario> => {
    return api.patch<Scenario>(`/scenarios/${id}/publish`, { isPublished });
  },

  deleteScenario: async (id: number): Promise<void> => {
    return api.delete<void>(`/scenarios/${id}`);
  },
};
