import { api } from './apiClient';
import {
  Quiz,
  QuizAttempt,
  QuizDetail,
  SubmitQuizAttemptRequest,
} from '../types/quiz';

export const quizService = {
  getPublishedQuizzes: async (): Promise<Quiz[]> => {
    return api.get<Quiz[]>('/quizzes');
  },

  getQuizById: async (id: number): Promise<QuizDetail> => {
    return api.get<QuizDetail>(`/quizzes/${id}`);
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
