using Sentinela.Domain.Repositories;
using Sentinela.Domain.Repositories.Asset;
using Sentinela.Domain.Repositories.Sample;

namespace Sentinela.Application.UseCases.Sample.Process;

public class ProcessSampleBatchUseCase : IProcessSampleBatchUseCase
{
    private readonly ISampleWriteOnlyRepository _sampleWriteOnlyRepository;
    private readonly IAssetUpdateOnlyRepository _assetUpdateOnlyRepository;
    private readonly IUnitOfWork _unitOfWork;
    public ProcessSampleBatchUseCase(ISampleWriteOnlyRepository sampleWriteOnlyRepository, 
            IAssetUpdateOnlyRepository assetUpdateOnlyRepository, IUnitOfWork unitOfWork)
    {
        _sampleWriteOnlyRepository = sampleWriteOnlyRepository;
        _assetUpdateOnlyRepository = assetUpdateOnlyRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task Execute(SampleBatchMessage sampleBatchMessage)
    {
        var samples = sampleBatchMessage.Samples.Select(item => new Domain.Entities.Sample
        {
            AssetId = sampleBatchMessage.AssetId,   
            ReceivedAt = sampleBatchMessage.ReceivedAt,
            Value = item.Value,
            Metric = (Domain.Enums.MetricType)item.Metric,
            CollectedAt = item.CollectedAt
        }).ToList();

        await _sampleWriteOnlyRepository.AddRange(samples);
        await _assetUpdateOnlyRepository.UpdateLastSeen(sampleBatchMessage.AssetId, sampleBatchMessage.ReceivedAt);

        await _unitOfWork.Commit();
    }
}
