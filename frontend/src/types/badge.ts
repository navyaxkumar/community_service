export interface Badge {
  id: number;
  name: string;
  description?: string;
  icon?: string;
  requiredPoints: number;
  isActive: boolean;
  createdAt: string;
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
}
