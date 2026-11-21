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

export interface QuizManagementDetail extends Quiz {
  questions: Array<{
    id: number;
    questionText: string;
    explanation?: string;
    order: number;
    options: Array<QuizOption & { isCorrect: boolean }>;
  }>;
}

export interface QuizOptionRequest {
  optionText: string;
  isCorrect: boolean;
  order: number;
}

export interface QuizQuestionRequest {
  questionText: string;
  explanation?: string;
  order: number;
  options: QuizOptionRequest[];
}

export interface SaveQuizRequest {
  title: string;
  description?: string;
  isPublished: boolean;
  fraudCategoryId?: number;
  questions: QuizQuestionRequest[];
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
