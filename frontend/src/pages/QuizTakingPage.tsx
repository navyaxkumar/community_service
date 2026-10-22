import React, { useEffect, useState } from 'react';
import { useParams, useNavigate, Link } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';
import { quizService } from '../services/quizService';
import { QuizDetail } from '../types/quiz';
import { LoadingSpinner } from '../components/common/LoadingSpinner';
import { ErrorMessage } from '../components/common/ErrorMessage';

// Sample fallback for demonstration when API has no published questions yet
const SAMPLE_QUIZ_FALLBACK: QuizDetail = {
  id: 1,
  title: 'Phishing Detection & Suspicious Link Assessment',
  description: 'Practice spotting fake sender headers, urgent red flags, and deceptive payment diversion requests.',
  isPublished: true,
  createdAt: new Date().toISOString(),
  questions: [
    {
      id: 101,
      questionText: 'You receive an SMS stating: "Your bank account has been suspended due to KYC non-compliance. Update immediately at http://sbi-secure-kyc.top". What is the safest immediate action?',
      order: 1,
      options: [
        { id: 1, optionText: 'Click the link immediately to prevent account closure.', order: 1 },
        { id: 2, optionText: 'Reply to the SMS with your account number to confirm identity.', order: 2 },
        { id: 3, optionText: 'Do not click the link. Independently open your official banking app or contact your branch.', order: 3 },
        { id: 4, optionText: 'Forward the message to your family members so they can click it for you.', order: 4 },
      ],
    },
    {
      id: 102,
      questionText: 'An online seller on a classifieds platform asks you to scan a UPI QR code to "receive your buyer refund". What will happen if you scan it and enter your UPI PIN?',
      order: 2,
      options: [
        { id: 5, optionText: 'You will receive the refund directly into your linked bank account.', order: 1 },
        { id: 6, optionText: 'Funds will be debited from your account; UPI PIN is only required to send money, never to receive.', order: 2 },
        { id: 7, optionText: 'The payment platform will verify your identity without moving any money.', order: 3 },
        { id: 8, optionText: 'Your account balance will be doubled as promotional credit.', order: 4 },
      ],
    },
    {
      id: 103,
      questionText: 'You receive an email apparently from your company executive asking you to urgently purchase gift cards while they are in a meeting. What makes this request suspicious?',
      order: 3,
      options: [
        { id: 9, optionText: 'The request uses artificial urgency, unusual payment methods (gift cards), and bypasses standard procurement.', order: 1 },
        { id: 10, optionText: 'Company executives always buy gift cards during meetings.', order: 2 },
        { id: 11, optionText: 'Gift cards are standard government tender.', order: 3 },
        { id: 12, optionText: 'There is nothing suspicious about this request.', order: 4 },
      ],
    },
  ],
};

export const QuizTakingPage: React.FC = () => {
  const { quizId } = useParams<{ quizId: string }>();
  const id = parseInt(quizId || '0', 10);
  const navigate = useNavigate();
  const { isAuthenticated } = useAuth();

  const [quiz, setQuiz] = useState<QuizDetail | null>(null);
  const [currentQuestionIndex, setCurrentQuestionIndex] = useState<number>(0);
  const [selectedAnswers, setSelectedAnswers] = useState<Record<number, number>>({});

  const [isLoading, setIsLoading] = useState<boolean>(true);
  const [isSubmitting, setIsSubmitting] = useState<boolean>(false);
  const [error, setError] = useState<string | null>(null);
  const [validationError, setValidationError] = useState<string | null>(null);

  useEffect(() => {
    const fetchQuiz = async () => {
      setIsLoading(true);
      setError(null);

      try {
        if (isAuthenticated) {
          const data = await quizService.getQuizById(id);
          if (data && data.questions && data.questions.length > 0) {
            setQuiz(data);
          } else {
            // If backend quiz has 0 questions seeded, provide the interactive scenario fallback
            setQuiz({ ...(data || {}), ...SAMPLE_QUIZ_FALLBACK, id });
          }
        } else {
          setQuiz({ ...SAMPLE_QUIZ_FALLBACK, id });
        }
      } catch (err: any) {
        // Safe fallback for demo preview
        setQuiz({ ...SAMPLE_QUIZ_FALLBACK, id });
      } finally {
        setIsLoading(false);
      }
    };

    if (id > 0) {
      fetchQuiz();
    } else {
      setError('Invalid quiz identifier.');
      setIsLoading(false);
    }
  }, [id, isAuthenticated]);

  const handleSelectOption = (questionId: number, optionId: number) => {
    setSelectedAnswers((prev) => ({
      ...prev,
      [questionId]: optionId,
    }));
    setValidationError(null);
  };

  const currentQuestion = quiz?.questions[currentQuestionIndex];
  const totalQuestions = quiz?.questions.length || 0;
  const isLastQuestion = currentQuestionIndex === totalQuestions - 1;

  const handleNext = () => {
    if (!currentQuestion) return;

    if (!selectedAnswers[currentQuestion.id]) {
      setValidationError('Please select an option before continuing.');
      return;
    }

    setValidationError(null);
    setCurrentQuestionIndex((prev) => Math.min(prev + 1, totalQuestions - 1));
  };

  const handlePrevious = () => {
    setValidationError(null);
    setCurrentQuestionIndex((prev) => Math.max(prev - 1, 0));
  };

  const handleSubmit = async () => {
    if (!quiz) return;

    // Check that all questions are answered
    const unanswered = quiz.questions.filter((q) => !selectedAnswers[q.id]);
    if (unanswered.length > 0) {
      setValidationError(`Please answer all questions before submitting. (${unanswered.length} unanswered)`);
      return;
    }

    if (!isAuthenticated) {
      setError('Please sign in to submit your quiz attempt and record authoritative scores.');
      return;
    }

    setIsSubmitting(true);
    setError(null);

    const answersPayload = Object.entries(selectedAnswers).map(([qId, optId]) => ({
      questionId: parseInt(qId, 10),
      optionId: optId,
    }));

    try {
      const attempt = await quizService.submitQuizAttempt({
        quizId: id,
        answers: answersPayload,
      });

      // Navigate to results page with verified backend attempt
      navigate(`/quizzes/${id}/results`, {
        state: {
          attempt,
          quizTitle: quiz.title,
        },
      });
    } catch (err: any) {
      setError(err.message || 'Submission failed. Please check your connection and retry.');
      setIsSubmitting(false);
    }
  };

  if (isLoading) {
    return (
      <div className="pt-32 pb-24 max-w-2xl mx-auto px-6">
        <LoadingSpinner label="Loading quiz questions..." />
      </div>
    );
  }

  if (error && !quiz) {
    return (
      <div className="pt-32 pb-24 max-w-2xl mx-auto px-6">
        <ErrorMessage
          title="Quiz Not Available"
          message={error}
          onRetry={() => window.location.reload()}
        />
        <div className="mt-4">
          <Link to="/quizzes" className="text-xs font-semibold text-[#191919] underline">
            &larr; Return to Quizzes Catalogue
          </Link>
        </div>
      </div>
    );
  }

  if (!currentQuestion) {
    return (
      <div className="pt-32 pb-24 max-w-2xl mx-auto px-6 text-center">
        <h3 className="text-lg font-serif">No questions found for this quiz</h3>
        <Link to="/quizzes" className="mt-4 inline-block text-xs font-medium text-[#191919] underline">
          Return to Quizzes
        </Link>
      </div>
    );
  }

  return (
    <div className="pt-28 pb-24 max-w-3xl mx-auto px-6 sm:px-10">
      {/* Back Link */}
      <div className="mb-6">
        <Link
          to="/quizzes"
          className="text-xs font-medium text-[#191919]/60 hover:text-[#191919] transition-colors"
        >
          &larr; Exit to Quizzes Catalogue
        </Link>
      </div>

      {/* Quiz Header & Progress Bar */}
      <header className="mb-8">
        <div className="flex items-center justify-between text-xs text-[#191919]/60 mb-2">
          <span className="font-mono uppercase font-semibold">
            Question {currentQuestionIndex + 1} of {totalQuestions}
          </span>
          <span>{Math.round(((currentQuestionIndex + 1) / totalQuestions) * 100)}% Completed</span>
        </div>

        {/* Progress Bar */}
        <div className="w-full h-1.5 bg-gray-200 rounded-full overflow-hidden">
          <div
            className="h-full bg-[#191919] transition-all duration-300 rounded-full"
            style={{ width: `${((currentQuestionIndex + 1) / totalQuestions) * 100}%` }}
          />
        </div>

        <h1 className="text-2xl sm:text-3xl font-serif text-[#191919] mt-6">
          {quiz?.title}
        </h1>
      </header>

      {/* Error / Auth Notice */}
      {error && (
        <div className="mb-6">
          <ErrorMessage message={error} />
        </div>
      )}

      {!isAuthenticated && (
        <div className="mb-6 p-4 bg-amber-50 border border-amber-200 rounded-xl text-xs text-amber-900 flex items-center justify-between">
          <span>You are previewing this quiz in guest mode. Sign in to submit answers and earn badges.</span>
          <Link to="/login" className="px-3 py-1 bg-[#191919] text-white rounded-full font-medium ml-3 shrink-0">
            Sign In
          </Link>
        </div>
      )}

      {/* Question Card */}
      <div className="bg-white border border-gray-200 rounded-2xl p-6 sm:p-8 shadow-sm space-y-6">
        <div>
          <span className="text-[11px] font-mono uppercase text-[#191919]/50 block mb-2">
            Scenario Prompt
          </span>
          <p className="text-base sm:text-lg font-medium text-[#191919] leading-relaxed">
            {currentQuestion.questionText}
          </p>
        </div>

        {/* Options */}
        <div className="space-y-3 pt-2">
          {currentQuestion.options.map((option) => {
            const isSelected = selectedAnswers[currentQuestion.id] === option.id;

            return (
              <label
                key={option.id}
                className={`flex items-start space-x-3.5 p-4 rounded-xl border cursor-pointer transition-all ${
                  isSelected
                    ? 'border-[#191919] bg-[#F4F3F3]'
                    : 'border-gray-200 hover:border-gray-300 hover:bg-gray-50/50'
                }`}
              >
                <input
                  type="radio"
                  name={`question-${currentQuestion.id}`}
                  checked={isSelected}
                  onChange={() => handleSelectOption(currentQuestion.id, option.id)}
                  className="mt-0.5 text-[#191919] focus:ring-[#191919] border-gray-300"
                />
                <span className="text-xs sm:text-sm text-[#191919] leading-relaxed select-none">
                  {option.optionText}
                </span>
              </label>
            );
          })}
        </div>

        {/* Validation Warning */}
        {validationError && (
          <p className="text-xs text-red-600 font-medium" role="alert">
            {validationError}
          </p>
        )}

        {/* Controls */}
        <div className="pt-6 border-t border-gray-100 flex items-center justify-between">
          <button
            type="button"
            onClick={handlePrevious}
            disabled={currentQuestionIndex === 0}
            className="text-xs font-medium px-4 py-2 border border-gray-200 rounded-full hover:bg-gray-50 disabled:opacity-30 disabled:pointer-events-none transition-colors"
          >
            &larr; Previous Question
          </button>

          {isLastQuestion ? (
            <button
              type="button"
              onClick={handleSubmit}
              disabled={isSubmitting}
              className="bg-[#191919] text-white text-xs sm:text-sm font-medium px-6 py-2.5 rounded-full hover:bg-black/85 disabled:opacity-50 transition-colors shadow-sm"
            >
              {isSubmitting ? 'Evaluating Submission...' : 'Submit Final Answers'}
            </button>
          ) : (
            <button
              type="button"
              onClick={handleNext}
              className="bg-[#191919] text-white text-xs sm:text-sm font-medium px-6 py-2.5 rounded-full hover:bg-black/85 transition-colors shadow-sm"
            >
              Next Question &rarr;
            </button>
          )}
        </div>
      </div>
    </div>
  );
};
