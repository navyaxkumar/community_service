using DigitalShield.API.DTOs.Fraud;

namespace DigitalShield.API.Interfaces.Services;

public interface IFraudAnalysisService
{
    FraudAnalysisResponseDto Analyze(FraudCheckRequestDto request);
}
