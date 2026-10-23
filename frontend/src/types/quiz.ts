export interface QuizOption {
  id: number;
  optionText: string;
  order: number;
}

export interface QuizOptionAdmin extends QuizOption {
  isCorrect: boolean;
}

export interface QuizQuestion {
  id: number;
  questionText: string;
  explanation?: string;
  order: number;
  options: QuizOption[];
}

export interface QuizQuestionAdmin {
  id: number;
  questionText: string;
  explanation?: string;
  order: number;
  options: QuizOptionAdmin[];
}

export interface Quiz {
  id: number;
  title: string;
  description?: string;
  isPublished: boolean;
  fraudCategoryId?: number;
  createdAt: string;
  updatedAt?: string | null;

  targetAudience?: string;
  difficultyLevel?: string;
}

export interface QuizDetail extends Quiz {
  questions: QuizQuestion[];
}

export interface QuizManagementDetail extends Quiz {
  questions: QuizQuestionAdmin[];
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

export interface CreateQuizOptionRequest {
  optionText: string;
  isCorrect: boolean;
  order: number;
}

export interface CreateQuizQuestionRequest {
  questionText: string;
  explanation?: string;
  order: number;
  options: CreateQuizOptionRequest[];
}

export interface CreateQuizRequest {
  title: string;
  description?: string;
  isPublished: boolean;
  fraudCategoryId?: number;
  targetAudience?: string;
  difficultyLevel?: string;
  questions: CreateQuizQuestionRequest[];
}

export interface UpdateQuizRequest {
  title: string;
  description?: string;
  isPublished: boolean;
  fraudCategoryId?: number;
  targetAudience?: string;
  difficultyLevel?: string;
  questions: CreateQuizQuestionRequest[];
}
