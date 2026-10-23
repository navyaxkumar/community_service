export interface AdminDashboardStats {
  totalModules: number;
  publishedModules: number;
  draftModules: number;

  totalQuizzes: number;
  publishedQuizzes: number;
  draftQuizzes: number;
  totalQuizQuestions: number;

  totalScenarios: number;
  publishedScenarios: number;
  draftScenarios: number;

  totalCategories: number;
  activeCategories: number;
  inactiveCategories: number;

  totalBadges: number;
  activeBadges: number;
  inactiveBadges: number;

  totalUsers: number;
  totalQuizAttempts: number;
}
