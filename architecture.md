# DigitalShield — System Architecture

## 1. Overview

DigitalShield is a web-based digital fraud awareness and risk detection platform. It combines cybersecurity education, interactive fraud scenarios, quizzes, gamification, a rule-based fraud risk checker, and an anonymized analytics dashboard.

The architecture is designed to be modular so that learning, assessment, gamification, fraud analysis, and analytics can evolve independently.

---

## 2. High-Level Architecture

```text
                         ┌───────────────────────┐
                         │         USERS         │
                         │                       │
                         │ Elderly / New Users   │
                         │ Students / Community  │
                         │ Administrators        │
                         └───────────┬───────────┘
                                     │
                                     ▼
                    ┌────────────────────────────────┐
                    │       PRESENTATION LAYER       │
                    │          Web Frontend          │
                    │                                │
                    │ Dashboard                      │
                    │ Learning Modules               │
                    │ Fraud Scenarios                │
                    │ Quizzes                        │
                    │ Fraud Risk Checker             │
                    │ Progress / Badges              │
                    │ Admin Analytics                │
                    └───────────────┬────────────────┘
                                    │
                              HTTPS / REST API
                                    │
                                    ▼
                    ┌────────────────────────────────┐
                    │       APPLICATION LAYER        │
                    │          Backend API           │
                    │                                │
                    │ Authentication & Users         │
                    │ Learning Service               │
                    │ Scenario Service               │
                    │ Quiz & Assessment Service      │
                    │ Gamification Service           │
                    │ Progress Service               │
                    │ Analytics Service              │
                    └───────────────┬────────────────┘
                                    │
                     ┌──────────────┴──────────────┐
                     │                             │
                     ▼                             ▼
          ┌──────────────────────┐      ┌──────────────────────┐
          │   FRAUD ANALYSIS     │      │      DATA LAYER      │
          │       ENGINE         │      │                      │
          │                      │      │ SQL Database         │
          │ Message Analyzer     │      │ Users                │
          │ URL Analyzer         │      │ Modules              │
          │ Pattern Detection    │      │ Questions            │
          │ Rule Engine          │      │ Scenarios            │
          │ Risk Scoring         │      │ Progress             │
          │ Warning Detection    │      │ Badges               │
          └──────────┬───────────┘      │ Fraud Checks         │
                     │                  └──────────┬───────────┘
                     │                             │
                     └──────────────┬──────────────┘
                                    ▼
                    ┌────────────────────────────────┐
                    │       ANALYTICS DASHBOARD      │
                    │                                │
                    │ Participation                  │
                    │ Quiz Performance               │
                    │ Fraud Categories               │
                    │ Completion Rates               │
                    │ Awareness Improvement          │
                    └────────────────────────────────┘
```

---

## 3. Architecture Layers

### 3.1 Presentation Layer

The presentation layer provides the user-facing web interface.

Main modules:

- User dashboard
- Learning modules
- Interactive fraud scenarios
- Quiz interface
- Fraud risk checker
- Progress tracking
- Points and badges
- Administrator dashboard

The frontend communicates with the backend through HTTPS REST APIs.

### 3.2 Application Layer

The backend contains the application's business logic.

#### Authentication and User Service

Responsible for:

- User registration and login
- Authentication
- Role management
- User profile management
- Session/token handling

#### Learning Service

Responsible for:

- Fraud categories
- Learning modules
- Educational content
- Safety recommendations
- Module completion

#### Scenario Service

Responsible for:

- Real-life-inspired fraud scenarios
- Scenario questions
- Correct/incorrect decisions
- Explanations and feedback

#### Quiz and Assessment Service

Responsible for:

- Quiz questions
- Answer validation
- Score calculation
- Quiz attempts
- Pre-test and post-test results

#### Gamification Service

Responsible for:

- Points
- Levels
- Badges
- Achievement rules

#### Progress Service

Responsible for:

- Module progress
- Scenario completion
- Quiz progress
- User learning history

#### Analytics Service

Responsible for:

- Aggregated participation statistics
- Quiz performance
- Fraud category statistics
- Completion rates
- Awareness improvement measurements

---

## 4. Fraud Analysis Architecture

The fraud checker uses an explainable rule-based analysis pipeline.

```text
             User Input
          Message / URL
                 │
                 ▼
        ┌─────────────────┐
        │ Input Validation │
        └────────┬────────┘
                 │
                 ▼
        ┌─────────────────┐
        │ Pre-processing  │
        └────────┬────────┘
                 │
        ┌────────┴────────┐
        ▼                 ▼
┌───────────────┐  ┌───────────────┐
│ Message       │  │ URL           │
│ Analysis      │  │ Analysis      │
└───────┬───────┘  └───────┬───────┘
        │                  │
        └────────┬─────────┘
                 ▼
        ┌──────────────────┐
        │ Warning Indicator│
        │ Detection        │
        └────────┬─────────┘
                 │
                 ▼
        ┌──────────────────┐
        │ Risk Score Engine│
        └────────┬─────────┘
                 │
        ┌────────┼─────────┐
        ▼        ▼         ▼
      Low    Suspicious   High
                 │
                 ▼
        ┌──────────────────┐
        │ Explanation      │
        │ + Safety Actions │
        └──────────────────┘
```

### Example indicators

The engine may detect:

- Requests for OTP, PIN, password, or sensitive information
- Urgent or threatening language
- Prize/reward claims
- Suspicious URLs
- Account-blocking threats
- Requests for payments
- Unknown or suspicious sender information
- Impersonation indicators

The result should be presented as a risk assessment based on detected indicators, not as a guaranteed determination that content is fraudulent.

Example:

```text
Risk Level: HIGH

Warning Signs:
- Requests an OTP
- Uses urgent language
- Contains a suspicious URL

Recommended Actions:
- Do not click the link
- Do not share OTP/PIN/password
