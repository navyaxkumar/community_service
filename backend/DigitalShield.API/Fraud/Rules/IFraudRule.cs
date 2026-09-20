using DigitalShield.API.Fraud.Analyzer;

namespace DigitalShield.API.Fraud.Rules;

public interface IFraudRule
{
    FraudRuleResult Evaluate(FraudAnalysisInput input);
}
