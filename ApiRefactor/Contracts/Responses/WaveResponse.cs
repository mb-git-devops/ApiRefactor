namespace ApiRefactor.Contracts.Responses;

public sealed record WaveResponse(Guid Id, string Name, DateTime WaveDate);
