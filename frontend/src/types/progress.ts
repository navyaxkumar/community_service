export interface UserProgress {
  id: number;
  learningModuleId: number;
  isCompleted: boolean;
  progressPercentage: number;
  startedAt: string;
  completedAt?: string | null;
}

export interface CreateUserProgressRequest {
  learningModuleId: number;
  progressPercentage: number;
  isCompleted: boolean;
}

export interface UpdateUserProgressRequest {
  progressPercentage: number;
  isCompleted: boolean;
}
