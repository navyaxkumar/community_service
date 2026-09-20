# DigitalShield — Software Requirements Specification

## 1. Project Overview

### 1.1 Project Name

**DigitalShield — Interactive Digital Fraud Awareness and Risk Detection Platform**

### 1.2 Purpose

DigitalShield is a web-based platform designed to improve users' awareness of digital fraud and phishing. The system teaches users about common scams, provides interactive fraud scenarios and quizzes, rewards learning through gamification, and provides an explainable risk checker for suspicious messages and URLs.

The system also provides an anonymized analytics dashboard for evaluating participation, performance, fraud-category awareness, and improvement between pre-test and post-test assessments.

---

# 2. Problem Statement

The increasing use of smartphones, UPI, online banking, social media, and digital services has increased exposure to phishing, OTP scams, fake job offers, shopping scams, impersonation, malicious links, and other digital fraud.

Elderly people and first-time digital users may have difficulty identifying warning signs such as urgency, suspicious URLs, requests for OTPs/PINs, fake rewards, and impersonation.

The proposed system provides a simple and interactive environment for learning, practicing, testing, and measuring digital fraud awareness.

---

# 3. Objectives

The system shall:

1. Educate users about common digital fraud techniques.
2. Explain common fraud warning signs using simple language.
3. Provide interactive real-life-inspired fraud scenarios.
4. Assess users using quizzes and assessments.
5. Provide points, levels, badges, and progress tracking.
6. Allow users to check suspicious messages and URLs.
7. Detect and explain common fraud indicators.
8. Provide recommended safety actions.
9. Measure awareness improvement using pre-test and post-test assessments.
10. Provide anonymized aggregate statistics to administrators/community organizations.

---

# 4. Scope

## 4.1 In Scope

- User registration and authentication
- Learning modules
- Fraud-category information
- Interactive scenarios
- Quizzes
- Pre-test and post-test assessments
- Points and badges
- User progress tracking
- Suspicious message checker
- Suspicious URL checker
- Explainable risk scoring
- Safety recommendations
- Admin analytics dashboard
- Anonymized aggregate reporting

## 4.2 Out of Scope

The initial version will not:

- Guarantee that a message or URL is legitimate or fraudulent.
- Automatically contact banks, police, or financial institutions.
- Automatically recover money lost through fraud.
- Replace professional cybersecurity investigation.
- Provide unrestricted automated web crawling.
- Store unnecessary sensitive user content.
- Make financial or legal decisions on behalf of users.

---

# 5. User Roles

## 5.1 Normal User

A normal user can:

- Register/login
- Learn about fraud
- Complete learning modules
- Attempt scenarios
- Take quizzes
- View explanations
- Earn points and badges
- View personal progress
- Submit suspicious messages/URLs to the risk checker

## 5.2 Administrator

An administrator can:

- Manage learning content
- Manage fraud categories
- Manage scenarios
- Manage quiz questions
- View aggregate analytics
- View participation statistics
- Monitor system usage
- Manage badges and achievement rules

Administrative access must be protected by role-based authorization.

---

# 6. Functional Requirements

## FR-01: User Registration

The system shall allow users to create an account.

Required information may include:

- Name
- Email/username
- Password

The system shall validate registration input and prevent duplicate accounts where applicable.

---

## FR-02: User Authentication

The system shall allow registered users to log in securely.

The system shall:

- Validate credentials.
- Create an authenticated session/token.
- Restrict protected resources to authenticated users.
- Provide logout functionality.

---

## FR-03: Learning Modules

The system shall provide educational modules for common fraud categories.

Possible categories include:

- Phishing
- OTP scams
- UPI fraud
- Fake customer-care scams
- Fake job offers
- Shopping scams
- Investment scams
- Lottery/reward scams
- Social-media impersonation
- QR-code scams
- Malicious links
- Remote-access scams

Each module should provide:

- Definition
- Example
- Warning signs
- Safety tips
- Recommended actions

---

## FR-04: Interactive Scenarios

The system shall present realistic fraud scenarios.

Each scenario shall contain:

- Scenario description
- Multiple possible actions
- Correct/safe response
- Explanation
- Relevant fraud category

The system shall provide immediate feedback after the user selects an option.

---

## FR-05: Quiz System

The system shall provide quizzes related to learning modules.

The quiz system shall:

- Display questions and options.
- Validate answers.
- Calculate scores.
- Display explanations.
- Store quiz attempts.
- Update user progress.

---

## FR-06: Pre-Test and Post-Test

The system shall support awareness assessments before and after training.

The system shall record:

- Initial score
- Final score
- Score difference
- Assessment date

Awareness improvement can be calculated as:

```text
Improvement = Post-test Score - Pre-test Score
```

The system should report the result as a change in score rather than presenting it as proof of long-term behavioral change.

---

## FR-07: Gamification

The system shall award points for activities.

Example:

```text
Complete learning module    +20
Correct scenario answer     +15
Correct quiz answer         +10
Complete assessment         +25
```

The system shall support:

- Points
- Levels
- Badges
- Achievement tracking

The exact scoring values should be configurable.

---

## FR-08: Progress Tracking

The system shall display the user's learning progress.

The dashboard may display:

- Modules completed
