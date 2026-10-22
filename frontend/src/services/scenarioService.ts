import { api } from './apiClient';
import { Scenario } from '../types/badge';

export const scenarioService = {
  getPublishedScenarios: async (): Promise<Scenario[]> => {
    return api.get<Scenario[]>('/scenarios');
  },

  getScenarioById: async (id: number): Promise<Scenario> => {
    return api.get<Scenario>(`/scenarios/${id}`);
  },

  getScenariosByCategory: async (categoryId: number): Promise<Scenario[]> => {
    return api.get<Scenario[]>(`/scenarios/category/${categoryId}`);
  },
};
