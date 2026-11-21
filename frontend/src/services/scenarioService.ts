import { api } from './apiClient';
import { SaveScenarioRequest, Scenario } from '../types/badge';

export const scenarioService = {
  getPublishedScenarios: async (): Promise<Scenario[]> => {
    return api.get<Scenario[]>('/scenarios');
  },

  getAllForManagement: async (): Promise<Scenario[]> => {
    return api.get<Scenario[]>('/scenarios/manage');
  },

  getScenarioById: async (id: number): Promise<Scenario> => {
    return api.get<Scenario>(`/scenarios/${id}`);
  },

  createScenario: async (scenario: SaveScenarioRequest): Promise<Scenario> => {
    return api.post<Scenario>('/scenarios', scenario);
  },

  updateScenario: async (id: number, scenario: SaveScenarioRequest): Promise<Scenario> => {
    return api.put<Scenario>(`/scenarios/${id}`, scenario);
  },

  getScenariosByCategory: async (categoryId: number): Promise<Scenario[]> => {
    return api.get<Scenario[]>(`/scenarios/category/${categoryId}`);
  },
};
