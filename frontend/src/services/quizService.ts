import { api } from './apiClient';
import {
  CreateQuizRequest,
  Quiz,
  QuizAttempt,
  QuizDetail,
  QuizManagementDetail,
  SubmitQuizAttemptRequest,
  UpdateQuizRequest,
} from '../types/quiz';

export const quizService = {
  getPublishedQuizzes: async (audience?: string): Promise<Quiz[]> => {
    const query = audience && audience !== 'All' ? `?audience=${encodeURIComponent(audience)}` : '';
    return api.get<Quiz[]>(`/quizzes${query}`);
  },

  getAllQuizzesAdmin: async (): Promise<Quiz[]> => {
    return api.get<Quiz[]>('/quizzes/admin/all');
  },

  getQuizById: async (id: number): Promise<QuizDetail> => {
    return api.get<QuizDetail>(`/quizzes/${id}`);
  },

  getQuizManagementDetail: async (id: number): Promise<QuizManagementDetail> => {
    return api.get<QuizManagementDetail>(`/quizzes/admin/${id}`);
  },

  getQuizzesByCategory: async (categoryId: number): Promise<Quiz[]> => {
    return api.get<Quiz[]>(`/quizzes/category/${categoryId}`);
  },

  createQuiz: async (quiz: CreateQuizRequest): Promise<Quiz> => {
    return api.post<Quiz>('/quizzes', quiz);
  },

  updateQuiz: async (id: number, quiz: UpdateQuizRequest): Promise<Quiz> => {
    return api.put<Quiz>(`/quizzes/${id}`, quiz);
  },

  setPublishStatus: async (id: number, isPublished: boolean): Promise<Quiz> => {
    return api.patch<Quiz>(`/quizzes/${id}/publish`, { isPublished });
  },

  deleteQuiz: async (id: number): Promise<void> => {
    return api.delete<void>(`/quizzes/${id}`);
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
