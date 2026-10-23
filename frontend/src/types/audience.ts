export type AudienceSegment =
  | 'All'
  | 'EarlyLearners'
  | 'Children'
  | 'Teens'
  | 'YoungAdults'
  | 'Adults'
  | 'Seniors'
  | 'Professionals';

export type DifficultyLevel = 'Beginner' | 'Intermediate' | 'Advanced';

export interface AudienceOption {
  id: AudienceSegment;
  label: string;
  ageRange: string;
  badge: string;
  iconName: string;
  tagline: string;
  themeColor: string;
}

export const AUDIENCE_SEGMENTS: AudienceOption[] = [
  {
    id: 'All',
    label: 'All Audiences',
    ageRange: 'Universal',
    badge: 'Universal',
    iconName: 'Globe',
    tagline: 'Standard digital safety guidelines suitable for all users.',
    themeColor: 'bg-gray-100 text-gray-800 border-gray-200',
  },
  {
    id: 'EarlyLearners',
    label: 'Early Learners',
    ageRange: 'Ages 5–7',
    badge: 'Kids 5–7',
    iconName: 'Smile',
    tagline: 'Simple safety habits, friendly illustrations, and trusted adult guidance.',
    themeColor: 'bg-emerald-50 text-emerald-800 border-emerald-200',
  },
  {
    id: 'Children',
    label: 'Children',
    ageRange: 'Ages 8–12',
    badge: 'Kids 8–12',
    iconName: 'Gamepad2',
    tagline: 'Spot game currency traps, fake giveaways, and secret code safety.',
    themeColor: 'bg-sky-50 text-sky-800 border-sky-200',
  },
  {
    id: 'Teens',
    label: 'Teenagers',
    ageRange: 'Ages 13–17',
    badge: 'Teens 13–17',
    iconName: 'Sparkles',
    tagline: 'Social media clone profiles, gaming account trades, and peer privacy.',
    themeColor: 'bg-purple-50 text-purple-800 border-purple-200',
  },
  {
    id: 'YoungAdults',
    label: 'Students & Young Adults',
    ageRange: 'Ages 18–24',
    badge: 'Students 18–24',
    iconName: 'GraduationCap',
    tagline: 'Work-from-home job scams, UPI collect traps, and rental fraud.',
    themeColor: 'bg-indigo-50 text-indigo-800 border-indigo-200',
  },
  {
    id: 'Adults',
    label: 'General Adults',
    ageRange: 'Everyday Users',
    badge: 'Adults',
    iconName: 'Shield',
    tagline: 'Banking fraud, courier SMS links, marketplace traps, and recovery.',
    themeColor: 'bg-blue-50 text-blue-800 border-blue-200',
  },
  {
    id: 'Seniors',
    label: 'Senior Citizens',
    ageRange: 'Seniors & First-Time',
    badge: 'Seniors',
    iconName: 'HeartHandshake',
    tagline: 'Calm plain language, phone verification, high contrast, and remote-app safety.',
    themeColor: 'bg-amber-50 text-amber-900 border-amber-200',
  },
  {
    id: 'Professionals',
    label: 'Professionals & Orgs',
    ageRange: 'Enterprise',
    badge: 'Enterprise',
    iconName: 'Briefcase',
    tagline: 'Business Email Compromise (BEC), invoice redirection, and MFA fatigue.',
    themeColor: 'bg-slate-100 text-slate-800 border-slate-300',
  },
];

export function getAudienceMeta(segment?: string): AudienceOption {
  if (!segment) return AUDIENCE_SEGMENTS[0];
  const found = AUDIENCE_SEGMENTS.find(
    (s) => s.id.toLowerCase() === segment.toLowerCase().replace(/[-_ ]/g, '')
  );
  return found || AUDIENCE_SEGMENTS[0];
}
