export interface LearningModule {
  id: number;
  title: string;
  description?: string;
  content: string;
  order: number;
  isPublished: boolean;
  fraudCategoryId?: number;
  createdAt: string;
  updatedAt?: string | null;

  targetAudience?: string;
  difficultyLevel?: string;
  estimatedMinutes?: number;
  learningObjectives?: string;
  safetyTips?: string;
}

export interface CreateLearningModuleRequest {
  title: string;
  description?: string;
  content: string;
  order: number;
  isPublished: boolean;
  fraudCategoryId?: number;

  targetAudience?: string;
  difficultyLevel?: string;
  estimatedMinutes?: number;
  learningObjectives?: string;
  safetyTips?: string;
}

export interface UpdateLearningModuleRequest {
  title: string;
  description?: string;
  content: string;
  order: number;
  isPublished: boolean;
  fraudCategoryId?: number;

  targetAudience?: string;
  difficultyLevel?: string;
  estimatedMinutes?: number;
  learningObjectives?: string;
  safetyTips?: string;
}
