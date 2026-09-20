using DigitalShield.API.Fraud.Rules;

namespace DigitalShield.API.Fraud.Analyzer;

public interface IFraudAnalyzer
{
    IReadOnlyList<FraudRuleResult> Analyze(FraudAnalysisInput input);
}
