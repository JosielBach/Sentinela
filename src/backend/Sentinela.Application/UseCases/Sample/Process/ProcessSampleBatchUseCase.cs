using Sentinela.Domain.Caching;
using Sentinela.Domain.Repositories;
using Sentinela.Domain.Repositories.Asset;
using Sentinela.Domain.Repositories.Sample;

namespace Sentinela.Application.UseCases.Sample.Process;

public class ProcessSampleBatchUseCase : IProcessSampleBatchUseCase
{
    private static readonly TimeSpan IDEMPOTENCY_TIME_TO_LIVE = TimeSpan.FromHours(24);
    private readonly IIdempotencyCache _idempotencyCache;
    private readonly ISampleWriteOnlyRepository _sampleWriteOnlyRepository;
    private readonly IAssetUpdateOnlyRepository _assetUpdateOnlyRepository;
    private readonly IUnitOfWork _unitOfWork;
    public ProcessSampleBatchUseCase(ISampleWriteOnlyRepository sampleWriteOnlyRepository, 
            IAssetUpdateOnlyRepository assetUpdateOnlyRepository, IUnitOfWork unitOfWork, IIdempotencyCache idempotencyCache)
    {
        _sampleWriteOnlyRepository = sampleWriteOnlyRepository;
        _assetUpdateOnlyRepository = assetUpdateOnlyRepository;
        _unitOfWork = unitOfWork;
        _idempotencyCache = idempotencyCache;
    }

    public async Task Execute(SampleBatchMessage sampleBatchMessage)
    {
        var idempotencyKey = $"idempotency:{sampleBatchMessage.AssetId}:{sampleBatchMessage.IdempotencyKey}";

        if (!await _idempotencyCache.TryRegister(idempotencyKey, IDEMPOTENCY_TIME_TO_LIVE))
            return;

        try
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
        catch
        {
            await _idempotencyCache.Remove(idempotencyKey);
            throw;
        }
    }
}
