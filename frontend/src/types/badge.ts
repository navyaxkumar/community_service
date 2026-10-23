export interface Badge {
  id: number;
  name: string;
  description?: string;
  icon?: string;
  requiredPoints: number;
  isActive: boolean;
  createdAt: string;
}

export interface CreateBadgeRequest {
  name: string;
  description?: string;
  icon?: string;
  requiredPoints: number;
  isActive: boolean;
}

export interface UpdateBadgeRequest {
  name: string;
  description?: string;
  icon?: string;
  requiredPoints: number;
  isActive: boolean;
}

export interface Scenario {
  id: number;
  title: string;
  description?: string;
  situation: string;
  correctAction: string;
  isPublished: boolean;
  fraudCategoryId?: number;
  createdAt: string;

  targetAudience?: string;
  difficultyLevel?: string;
  safetyTips?: string;
}

export interface CreateScenarioRequest {
  title: string;
  description?: string;
  situation: string;
  correctAction: string;
  isPublished: boolean;
  fraudCategoryId?: number;

  targetAudience?: string;
  difficultyLevel?: string;
  safetyTips?: string;
}

export interface UpdateScenarioRequest {
  title: string;
  description?: string;
  situation: string;
  correctAction: string;
  isPublished: boolean;
  fraudCategoryId?: number;

  targetAudience?: string;
  difficultyLevel?: string;
  safetyTips?: string;
}
