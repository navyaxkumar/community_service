# DigitalShield — Project Folder Structure

## 1. Recommended Project Structure

```text
DigitalShield/
│
├── README.md
├── architecture.md
├── requirements.md
├── structure.md
├── .gitignore
├── docker-compose.yml
│
├── frontend/
│   ├── public/
│   │   ├── favicon.ico
│   │   └── images/
│   │
│   ├── src/
│   │   ├── assets/
│   │   │
│   │   ├── components/
│   │   │   ├── common/
│   │   │   │   ├── Button.jsx
│   │   │   │   ├── Modal.jsx
│   │   │   │   ├── Loader.jsx
│   │   │   │   └── Alert.jsx
│   │   │   │
│   │   │   ├── dashboard/
│   │   │   │   ├── StatCard.jsx
│   │   │   │   ├── ProgressCard.jsx
│   │   │   │   └── BadgeCard.jsx
│   │   │   │
│   │   │   ├── learning/
│   │   │   │   ├── FraudCategoryCard.jsx
│   │   │   │   ├── LearningModule.jsx
│   │   │   │   └── SafetyTips.jsx
│   │   │   │
│   │   │   ├── scenarios/
│   │   │   │   ├── ScenarioCard.jsx
│   │   │   │   └── ScenarioResult.jsx
│   │   │   │
│   │   │   ├── quiz/
│   │   │   │   ├── QuestionCard.jsx
│   │   │   │   ├── QuizResult.jsx
│   │   │   │   └── ScoreCard.jsx
│   │   │   │
│   │   │   ├── fraud-checker/
│   │   │   │   ├── MessageChecker.jsx
│   │   │   │   ├── UrlChecker.jsx
│   │   │   │   ├── RiskResult.jsx
│   │   │   │   └── WarningIndicator.jsx
│   │   │   │
│   │   │   └── admin/
│   │   │       ├── AnalyticsCard.jsx
│   │   │       ├── FraudChart.jsx
│   │   │       └── UserStats.jsx
│   │   │
│   │   ├── pages/
│   │   │   ├── Login.jsx
│   │   │   ├── Register.jsx
│   │   │   ├── Dashboard.jsx
│   │   │   ├── Learning.jsx
│   │   │   ├── Scenarios.jsx
│   │   │   ├── Quiz.jsx
│   │   │   ├── FraudChecker.jsx
│   │   │   ├── Progress.jsx
│   │   │   └── AdminDashboard.jsx
│   │   │
│   │   ├── services/
│   │   │   ├── api.js
│   │   │   ├── authService.js
│   │   │   ├── quizService.js
│   │   │   ├── learningService.js
│   │   │   ├── progressService.js
│   │   │   └── fraudService.js
│   │   │
│   │   ├── hooks/
│   │   ├── context/
│   │   │   └── AuthContext.jsx
│   │   │
│   │   ├── utils/
│   │   ├── routes/
│   │   │   └── AppRoutes.jsx
│   │   ├── App.jsx
│   │   └── main.jsx
│   │
│   ├── package.json
│   └── vite.config.js
│
├── backend/
│   ├── DigitalShield.API/
│   │   ├── Controllers/
│   │   │   ├── AuthController.cs
│   │   │   ├── LearningController.cs
│   │   │   ├── ScenarioController.cs
│   │   │   ├── QuizController.cs
│   │   │   ├── ProgressController.cs
│   │   │   ├── FraudController.cs
│   │   │   └── AdminController.cs
│   │   │
│   │   ├── Services/
│   │   │   ├── AuthService.cs
│   │   │   ├── LearningService.cs
│   │   │   ├── ScenarioService.cs
│   │   │   ├── QuizService.cs
│   │   │   ├── ProgressService.cs
│   │   │   ├── GamificationService.cs
│   │   │   └── AnalyticsService.cs
│   │   │
│   │   ├── Models/
│   │   │   ├── User.cs
│   │   │   ├── FraudCategory.cs
│   │   │   ├── LearningModule.cs
│   │   │   ├── Scenario.cs
│   │   │   ├── Question.cs
│   │   │   ├── QuizAttempt.cs
│   │   │   ├── UserProgress.cs
│   │   │   ├── Badge.cs
│   │   │   └── FraudCheck.cs
│   │   │
│   │   ├── DTOs/
│   │   │   ├── LoginRequest.cs
│   │   │   ├── RegisterRequest.cs
│   │   │   ├── QuizSubmissionDto.cs
│   │   │   └── FraudCheckRequest.cs
│   │   │
│   │   ├── Data/
│   │   │   ├── ApplicationDbContext.cs
│   │   │   └── Migrations/
│   │   │
│   │   ├── Middleware/
│   │   ├── Validators/
│   │   ├── Configuration/
│   │   ├── Program.cs
│   │   └── appsettings.json
│   │
│   └── DigitalShield.sln
│
├── fraud-engine/
│   ├── Rules/
│   │   ├── MessageRules/
│   │   │   ├── OtpRule
│   │   │   ├── UrgencyRule
│   │   │   ├── RewardRule
│   │   │   └── PaymentRule
│   │   │
│   │   └── UrlRules/
│   │       ├── SuspiciousDomainRule
│   │       ├── UrlPatternRule
│   │       └── HttpRule
│   │
│   ├── Analyzer/
│   │   ├── MessageAnalyzer
│   │   ├── UrlAnalyzer
│   │   └── IndicatorDetector
│   │
│   ├── Scoring/
│   │   ├── RiskScorer
│   │   └── RiskLevel
│   │
│   └── Recommendations/
│       └── SafetyRecommendation
│
├── database/
│   ├── schema/
│   │   ├── 001_users.sql
│   │   ├── 002_fraud_categories.sql
│   │   ├── 003_learning_modules.sql
│   │   ├── 004_scenarios.sql
│   │   ├── 005_questions.sql
│   │   ├── 006_quiz_attempts.sql
│   │   ├── 007_user_progress.sql
│   │   ├── 008_badges.sql
│   │   └── 009_fraud_checks.sql
│   │
│   └── seed/
│       ├── fraud_categories.sql
│       ├── learning_modules.sql
│       ├── scenarios.sql
│       ├── questions.sql
│       └── badges.sql
│
├── docs/
│   ├── diagrams/
│   │   ├── architecture.md
│   │   ├── er-diagram.md
│   │   ├── dfd-level-0.md
│   │   ├── dfd-level-1.md
│   │   └── use-case.md
│   │
│   ├── api/
│   │   └── api-documentation.md
│   │
│   └── testing/
│       └── test-plan.md
│
└── tests/
    ├── frontend/
    ├── backend/
    └── fraud-engine/
```

---

# 2. Frontend Structure

The frontend contains everything related to the user interface.

```text
frontend/
└── src/
    ├── components/
    ├── pages/
    ├── services/
    ├── hooks/
    ├── context/
    ├── utils/
    └── routes/
```

### `components/`

Reusable UI components are organized by feature.

```text
components/
├── common/
├── dashboard/
├── learning/
├── scenarios/
├── quiz/
├── fraud-checker/
└── admin/
```

### `pages/`

Pages represent complete application screens.

```text
pages/
├── Login.jsx
├── Register.jsx
├── Dashboard.jsx
├── Learning.jsx
├── Scenarios.jsx
├── Quiz.jsx
├── FraudChecker.jsx
