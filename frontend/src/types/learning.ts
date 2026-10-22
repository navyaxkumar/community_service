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
}

export interface CreateLearningModuleRequest {
  title: string;
  description?: string;
  content: string;
  order: number;
  isPublished: boolean;
  fraudCategoryId?: number;
}

export interface UpdateLearningModuleRequest {
  title: string;
  description?: string;
  content: string;
  order: number;
  isPublished: boolean;
  fraudCategoryId?: number;
}
