namespace Sentinela.Application.UseCases.Sample.Process;

public interface IProcessSampleBatchUseCase
{
    Task Execute(SampleBatchMessage sampleBatchMessage);
}
