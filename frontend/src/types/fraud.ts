export interface FraudCheckRequest {
  type: string;
  content?: string;
  url?: string;
  senderKnown?: boolean;
}

export interface FraudIndicator {
  code: string;
  title: string;
  score: number;
  explanation: string;
  category: string;
  evidence?: string;
  severity?: string;
}

export interface Recommendation {
  code: string;
  message: string;
}

export interface FraudAnalysisResponse {
  score: number;
  riskLevel: 'Low' | 'Medium' | 'High' | 'Critical' | 'UnableToDetermine' | string;
  riskReason?: string;
  indicators: FraudIndicator[];
  recommendations: Recommendation[];
  disclaimer: string;
  externalReputationCheckPerformed?: boolean;
  threatIntelligenceSource?: string;
}

export interface FraudCategory {
  id: number;
  name: string;
  description?: string;
  isActive: boolean;
  createdAt: string;
}

export interface CreateFraudCategoryRequest {
  name: string;
  description?: string;
  isActive: boolean;
}

export interface UpdateFraudCategoryRequest {
  name: string;
  description?: string;
  isActive: boolean;
}
