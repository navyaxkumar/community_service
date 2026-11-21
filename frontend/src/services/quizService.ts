import { api } from './apiClient';
import {
  Quiz,
  QuizAttempt,
  QuizDetail,
  QuizManagementDetail,
  SaveQuizRequest,
  SubmitQuizAttemptRequest,
} from '../types/quiz';

export const quizService = {
  getPublishedQuizzes: async (): Promise<Quiz[]> => {
    return api.get<Quiz[]>('/quizzes');
  },

  getAllForManagement: async (): Promise<Quiz[]> => {
    return api.get<Quiz[]>('/quizzes/manage');
  },

  getManagementDetails: async (id: number): Promise<QuizManagementDetail> => {
    return api.get<QuizManagementDetail>(`/quizzes/manage/${id}`);
  },

  getQuizById: async (id: number): Promise<QuizDetail> => {
    return api.get<QuizDetail>(`/quizzes/${id}`);
  },

  createQuiz: async (quiz: SaveQuizRequest): Promise<QuizDetail> => {
    return api.post<QuizDetail>('/quizzes', quiz);
  },

  updateQuiz: async (id: number, quiz: SaveQuizRequest): Promise<QuizDetail> => {
    return api.put<QuizDetail>(`/quizzes/${id}`, quiz);
  },

  getQuizzesByCategory: async (categoryId: number): Promise<Quiz[]> => {
    return api.get<Quiz[]>(`/quizzes/category/${categoryId}`);
  },

  submitQuizAttempt: async (attempt: SubmitQuizAttemptRequest): Promise<QuizAttempt> => {
    return api.post<QuizAttempt>('/quiz-attempts', attempt);
  },

  getMyAttempts: async (): Promise<QuizAttempt[]> => {
    return api.get<QuizAttempt[]>('/quiz-attempts/me');
  },

  getAttemptById: async (id: number): Promise<QuizAttempt> => {
    return api.get<QuizAttempt>(`/quiz-attempts/${id}`);
  },
};
