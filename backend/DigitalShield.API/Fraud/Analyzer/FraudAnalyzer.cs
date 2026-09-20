using DigitalShield.API.Fraud.Rules;

namespace DigitalShield.API.Fraud.Analyzer;

public class FraudAnalyzer : IFraudAnalyzer
{
    private readonly IReadOnlyList<IFraudRule> _rules;

    public FraudAnalyzer(IEnumerable<IFraudRule> rules)
    {
        _rules = rules.OrderBy(rule => rule.GetType().FullName, StringComparer.Ordinal).ToList();
    }

    public IReadOnlyList<FraudRuleResult> Analyze(FraudAnalysisInput input)
    {
        return _rules
            .Select(rule => rule.Evaluate(input))
            .Where(result => result.Triggered)
            .OrderBy(result => result.Code, StringComparer.Ordinal)
            .ToList();
    }
}
