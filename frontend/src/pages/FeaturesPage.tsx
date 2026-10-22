import React from 'react';
import { Link } from 'react-router-dom';
import { ArrowRight } from 'lucide-react';

export const FeaturesPage: React.FC = () => {
  return (
    <div className="pt-28 pb-24 max-w-5xl mx-auto px-6 sm:px-10">
      {/* Page Header */}
      <div className="max-w-2xl mb-16">
        <span className="text-xs uppercase tracking-widest text-[#191919]/50 font-semibold block mb-2">
          Platform Architecture & Capabilities
        </span>
        <h1 className="text-4xl sm:text-5xl font-serif text-[#191919] leading-tight">
          Comprehensive tools built for digital confidence.
        </h1>
        <p className="text-sm md:text-base text-[#191919]/70 mt-4 leading-relaxed">
          DigitalShield combines automated rule-based threat evaluation with structured educational curricula, interactive scenario quizzes, and verified progress tracking.
        </p>
      </div>

      {/* Feature Grid */}
      <div className="space-y-12">
        {/* Feature 1: Scam Checker */}
        <div className="p-8 sm:p-10 bg-white border border-gray-200 rounded-2xl shadow-sm grid grid-cols-1 md:grid-cols-12 gap-8 items-start">
          <div className="md:col-span-8 space-y-3">
            <div className="flex items-center space-x-2.5">
              <span className="text-xs font-mono font-semibold text-[#191919]/40">01</span>
              <span className="text-xs uppercase tracking-wider font-semibold text-emerald-700 bg-emerald-50 px-2.5 py-0.5 rounded-full">
                Core Engine &bull; Backend Connected
              </span>
            </div>
            <h2 className="text-2xl font-serif text-[#191919]">
              Interactive Scam & URL Checker
            </h2>
            <p className="text-xs sm:text-sm text-[#191919]/70 leading-relaxed">
              Submit suspicious text messages, emails, or links to our ASP.NET Core backend evaluation pipeline. The engine processes inputs against threat indicators including artificial urgency, payment redirect traps, sensitive credential requests, and unverified shortened link structures.
            </p>
            <div className="pt-2 flex flex-wrap gap-2 text-[11px] text-[#191919]/60">
              <span className="bg-[#F4F3F3] px-2.5 py-1 rounded">Heuristic scoring</span>
              <span className="bg-[#F4F3F3] px-2.5 py-1 rounded">Actionable recommendations</span>
              <span className="bg-[#F4F3F3] px-2.5 py-1 rounded">Educational risk breakdown</span>
            </div>
          </div>
          <div className="md:col-span-4 flex flex-col justify-between h-full space-y-4">
            <div className="p-4 bg-[#F4F3F3] rounded-xl text-xs text-[#191919]/70">
              <strong className="block text-[#191919] font-medium mb-1">Authenticated API</strong>
              Connects to <code className="font-mono text-[#191919]">POST /api/fraud/check</code> with full session protection.
            </div>
            <Link
              to="/scam-checker"
              className="inline-flex items-center justify-center gap-1.5 bg-[#191919] text-white text-xs font-medium px-5 py-3 rounded-full hover:bg-black/85 transition-colors"
            >
              Open Scam Checker <ArrowRight className="w-3.5 h-3.5" />
            </Link>
          </div>
        </div>

        {/* Feature 2: Learning Library */}
        <div className="p-8 sm:p-10 bg-white border border-gray-200 rounded-2xl shadow-sm grid grid-cols-1 md:grid-cols-12 gap-8 items-start">
          <div className="md:col-span-8 space-y-3">
            <div className="flex items-center space-x-2.5">
              <span className="text-xs font-mono font-semibold text-[#191919]/40">02</span>
              <span className="text-xs uppercase tracking-wider font-semibold text-emerald-700 bg-emerald-50 px-2.5 py-0.5 rounded-full">
                Curriculum &bull; Backend Connected
              </span>
            </div>
            <h2 className="text-2xl font-serif text-[#191919]">
              Searchable Learning Modules Library
            </h2>
            <p className="text-xs sm:text-sm text-[#191919]/70 leading-relaxed">
              Explore bite-sized, accessible lessons organized by threat vector. Each module delivers structured definitions, common deceptive scenarios, red flags to look out for, and preventative safety checklists.
            </p>
            <div className="pt-2 flex flex-wrap gap-2 text-[11px] text-[#191919]/60">
              <span className="bg-[#F4F3F3] px-2.5 py-1 rounded">Categorized by threat</span>
              <span className="bg-[#F4F3F3] px-2.5 py-1 rounded">Step-by-step guidance</span>
              <span className="bg-[#F4F3F3] px-2.5 py-1 rounded">Per-module completion state</span>
            </div>
          </div>
          <div className="md:col-span-4 flex flex-col justify-between h-full space-y-4">
            <div className="p-4 bg-[#F4F3F3] rounded-xl text-xs text-[#191919]/70">
              <strong className="block text-[#191919] font-medium mb-1">Module Catalog</strong>
              Connects to <code className="font-mono text-[#191919]">GET /api/learning-modules</code> with category filters.
            </div>
            <Link
              to="/learn"
              className="inline-flex items-center justify-center gap-1.5 bg-[#191919] text-white text-xs font-medium px-5 py-3 rounded-full hover:bg-black/85 transition-colors"
            >
              Browse Library <ArrowRight className="w-3.5 h-3.5" />
            </Link>
          </div>
        </div>

        {/* Feature 3: Interactive Quizzes */}
        <div className="p-8 sm:p-10 bg-white border border-gray-200 rounded-2xl shadow-sm grid grid-cols-1 md:grid-cols-12 gap-8 items-start">
          <div className="md:col-span-8 space-y-3">
            <div className="flex items-center space-x-2.5">
              <span className="text-xs font-mono font-semibold text-[#191919]/40">03</span>
              <span className="text-xs uppercase tracking-wider font-semibold text-emerald-700 bg-emerald-50 px-2.5 py-0.5 rounded-full">
                Interactive Testing &bull; Backend Connected
              </span>
            </div>
            <h2 className="text-2xl font-serif text-[#191919]">
              Scenario-Driven Quizzes & Scoring
            </h2>
            <p className="text-xs sm:text-sm text-[#191919]/70 leading-relaxed">
              Test your ability to spot scams through realistic multiple-choice scenarios. Submissions are scored authoritatively by the ASP.NET Core backend, recording attempts, calculated scores, and detailed explanations for each question.
            </p>
            <div className="pt-2 flex flex-wrap gap-2 text-[11px] text-[#191919]/60">
              <span className="bg-[#F4F3F3] px-2.5 py-1 rounded">Instant answer feedback</span>
              <span className="bg-[#F4F3F3] px-2.5 py-1 rounded">Authoritative scoring</span>
              <span className="bg-[#F4F3F3] px-2.5 py-1 rounded">Attempt history tracking</span>
            </div>
          </div>
          <div className="md:col-span-4 flex flex-col justify-between h-full space-y-4">
            <div className="p-4 bg-[#F4F3F3] rounded-xl text-xs text-[#191919]/70">
              <strong className="block text-[#191919] font-medium mb-1">Authoritative Evaluation</strong>
              Connects to <code className="font-mono text-[#191919]">POST /api/quiz-attempts</code> to record results.
            </div>
            <Link
              to="/quizzes"
              className="inline-flex items-center justify-center gap-1.5 bg-[#191919] text-white text-xs font-medium px-5 py-3 rounded-full hover:bg-black/85 transition-colors"
            >
              Start Quizzes <ArrowRight className="w-3.5 h-3.5" />
            </Link>
          </div>
        </div>

        {/* Feature 4: User Dashboard & Progress */}
        <div className="p-8 sm:p-10 bg-white border border-gray-200 rounded-2xl shadow-sm grid grid-cols-1 md:grid-cols-12 gap-8 items-start">
          <div className="md:col-span-8 space-y-3">
            <div className="flex items-center space-x-2.5">
              <span className="text-xs font-mono font-semibold text-[#191919]/40">04</span>
              <span className="text-xs uppercase tracking-wider font-semibold text-emerald-700 bg-emerald-50 px-2.5 py-0.5 rounded-full">
                Progress &bull; Backend Connected
              </span>
            </div>
            <h2 className="text-2xl font-serif text-[#191919]">
              Personal Safety Dashboard
            </h2>
            <p className="text-xs sm:text-sm text-[#191919]/70 leading-relaxed">
              View your personalized learning status, module completion percentages, quiz scores, and unlocked achievement badges. Everything is tied directly to your authenticated profile.
            </p>
            <div className="pt-2 flex flex-wrap gap-2 text-[11px] text-[#191919]/60">
              <span className="bg-[#F4F3F3] px-2.5 py-1 rounded">Persistent progress</span>
              <span className="bg-[#F4F3F3] px-2.5 py-1 rounded">Attempt history</span>
              <span className="bg-[#F4F3F3] px-2.5 py-1 rounded">Earned badge showcase</span>
            </div>
          </div>
          <div className="md:col-span-4 flex flex-col justify-between h-full space-y-4">
            <div className="p-4 bg-[#F4F3F3] rounded-xl text-xs text-[#191919]/70">
              <strong className="block text-[#191919] font-medium mb-1">User Isolation</strong>
              Connects to <code className="font-mono text-[#191919]">GET /api/progress/me</code> and <code className="font-mono text-[#191919]">GET /api/quiz-attempts/me</code>.
            </div>
            <Link
              to="/dashboard"
              className="inline-flex items-center justify-center gap-1.5 bg-[#191919] text-white text-xs font-medium px-5 py-3 rounded-full hover:bg-black/85 transition-colors"
            >
              Open Dashboard <ArrowRight className="w-3.5 h-3.5" />
            </Link>
          </div>
        </div>

        {/* Feature 5: Administrative Curriculum Management */}
        <div className="p-8 sm:p-10 bg-white border border-gray-200 rounded-2xl shadow-sm grid grid-cols-1 md:grid-cols-12 gap-8 items-start">
          <div className="md:col-span-8 space-y-3">
            <div className="flex items-center space-x-2.5">
              <span className="text-xs font-mono font-semibold text-[#191919]/40">05</span>
              <span className="text-xs uppercase tracking-wider font-semibold text-purple-700 bg-purple-50 px-2.5 py-0.5 rounded-full">
                Role-Protected &bull; Admin Only
              </span>
            </div>
            <h2 className="text-2xl font-serif text-[#191919]">
              Administrative Operations & Content Control
            </h2>
            <p className="text-xs sm:text-sm text-[#191919]/70 leading-relaxed">
              Authorized administrators can manage threat categories, add or edit learning modules, update quiz questions, and monitor curriculum readiness directly through backend-authorized endpoints.
            </p>
            <div className="pt-2 flex flex-wrap gap-2 text-[11px] text-[#191919]/60">
              <span className="bg-[#F4F3F3] px-2.5 py-1 rounded">Category management</span>
              <span className="bg-[#F4F3F3] px-2.5 py-1 rounded">Module publishing</span>
              <span className="bg-[#F4F3F3] px-2.5 py-1 rounded">Server-enforced role checks</span>
            </div>
          </div>
          <div className="md:col-span-4 flex flex-col justify-between h-full space-y-4">
            <div className="p-4 bg-[#F4F3F3] rounded-xl text-xs text-[#191919]/70">
              <strong className="block text-[#191919] font-medium mb-1">Protected Role</strong>
              Enforced by ASP.NET Core policy <code className="font-mono text-[#191919]">[Authorize(Roles = "Admin")]</code>.
            </div>
            <Link
              to="/admin"
              className="inline-flex items-center justify-center gap-1.5 border border-gray-300 text-[#191919] text-xs font-medium px-5 py-3 rounded-full hover:bg-gray-50 transition-colors"
            >
              Admin Portal <ArrowRight className="w-3.5 h-3.5" />
            </Link>
          </div>
        </div>
      </div>
    </div>
  );
};
