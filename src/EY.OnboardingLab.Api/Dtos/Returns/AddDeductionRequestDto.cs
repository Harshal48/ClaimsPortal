namespace EY.OnboardingLab.Api.Dtos.Returns;

public record AddDeductionRequestDto(string DeductionName, string DeductionType, decimal Amount);

