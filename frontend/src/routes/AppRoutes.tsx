import React from 'react';
import { Routes, Route } from 'react-router-dom';

// Layouts
import { PublicLayout } from '../components/layout/PublicLayout';
import { AppLayout } from '../components/layout/AppLayout';
import { AdminLayout } from '../components/layout/AdminLayout';
import { ProtectedRoute } from '../components/common/ProtectedRoute';

// Pages
import { HomePage } from '../pages/HomePage';
import { FeaturesPage } from '../pages/FeaturesPage';
import { ScamCheckerPage } from '../pages/ScamCheckerPage';
import { LearningPage } from '../pages/LearningPage';
import { ModuleDetailPage } from '../pages/ModuleDetailPage';
import { QuizzesPage } from '../pages/QuizzesPage';
import { QuizTakingPage } from '../pages/QuizTakingPage';
import { QuizResultsPage } from '../pages/QuizResultsPage';
import { LoginPage } from '../pages/LoginPage';
import { RegisterPage } from '../pages/RegisterPage';
import { DashboardPage } from '../pages/DashboardPage';
import { ProfilePage } from '../pages/ProfilePage';
import { SettingsPage } from '../pages/SettingsPage';
import { AdminPage } from '../pages/AdminPage';
import { NotFoundPage } from '../pages/NotFoundPage';
import { AccessDeniedPage } from '../pages/AccessDeniedPage';

export const AppRoutes: React.FC = () => {
  return (
    <Routes>
      {/* 1. Public Layout Routes */}
      <Route element={<PublicLayout />}>
        <Route path="/" element={<HomePage />} />
        <Route path="/features" element={<FeaturesPage />} />
        <Route path="/scam-checker" element={<ScamCheckerPage />} />
        <Route path="/learn" element={<LearningPage />} />
        <Route path="/learn/:moduleId" element={<ModuleDetailPage />} />
        <Route path="/quizzes" element={<QuizzesPage />} />
        <Route path="/quizzes/:quizId" element={<QuizTakingPage />} />
        <Route path="/quizzes/:quizId/results" element={<QuizResultsPage />} />
        <Route path="/login" element={<LoginPage />} />
        <Route path="/register" element={<RegisterPage />} />
        <Route path="/access-denied" element={<AccessDeniedPage />} />
        <Route path="*" element={<NotFoundPage />} />
      </Route>

      {/* 2. Authenticated Application Layout Routes */}
      <Route
        element={
          <ProtectedRoute>
            <AppLayout />
          </ProtectedRoute>
        }
      >
        <Route path="/dashboard" element={<DashboardPage />} />
        <Route path="/profile" element={<ProfilePage />} />
        <Route path="/settings" element={<SettingsPage />} />
      </Route>

      {/* 3. Admin Application Layout Routes */}
      <Route
        element={
          <ProtectedRoute requireAdmin>
            <AdminLayout />
          </ProtectedRoute>
        }
      >
        <Route path="/admin" element={<AdminPage />} />
      </Route>
    </Routes>
  );
};
