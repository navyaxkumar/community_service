export interface QuizOption {
  id: number;
  optionText: string;
  order: number;
}

export interface QuizQuestion {
  id: number;
  questionText: string;
  explanation?: string;
  order: number;
  options: QuizOption[];
}

export interface Quiz {
  id: number;
  title: string;
  description?: string;
  isPublished: boolean;
  fraudCategoryId?: number;
  createdAt: string;
  updatedAt?: string | null;
}

export interface QuizDetail extends Quiz {
  questions: QuizQuestion[];
}

export interface QuizAnswer {
  questionId: number;
  optionId: number;
}

export interface SubmitQuizAttemptRequest {
  quizId: number;
  answers: QuizAnswer[];
}

export interface QuizAttempt {
  id: number;
  quizId: number;
  score: number;
  totalQuestions: number;
  startedAt: string;
  completedAt?: string | null;
}
