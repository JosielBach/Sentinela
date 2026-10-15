using CommonTestUtilities.Caching;
using CommonTestUtilities.Repositories;
using CommonTestUtilities.Requests;
using Moq;
using Sentinela.Application.UseCases.Sample;
using Sentinela.Application.UseCases.Sample.Process;
using Shouldly;

namespace UseCases.Tests.Sample.Process;

public class ProcessSampleBatchUseCaseTests
{
    [Fact]
    public async Task Success()
    {
        var request = RequestSampleBatchJsonBuilder.Build().Samples;
        var assetId = Guid.NewGuid();
        var idempotencyKey = Guid.NewGuid().ToString();

        var sampleRepository = ISampleWriteOnlyRepositoryBuilder.Build();
        var unitOfWork = IUnitOfWorkBuilder.Build();
        var assetRepository = IAssetUpdateOnlyRepositoryBuilder.Build();

        var receivedAt = DateTime.UtcNow;
        var message = new SampleBatchMessage(assetId, idempotencyKey, receivedAt, request);

        var useCase = new ProcessSampleBatchUseCase(sampleRepository, assetRepository, unitOfWork, IIdempotencyCacheBuilder.Build());

        await useCase.Execute(message);


        IUnitOfWorkBuilder.VerifyCommit(unitOfWork);
        Mock.Get(sampleRepository).Verify(repository => repository.AddRange(It.Is<IList<Sentinela.Domain.Entities.Sample>>(list => list.Count == request.Count)), Times.Once);
        Mock.Get(assetRepository).Verify(repository => repository.UpdateLastSeen(assetId, receivedAt), Times.Once);
    }

    [Fact]
    public async Task Success_ShouldMapMessageToSamples()
    {
        var request = RequestSampleBatchJsonBuilder.Build().Samples;
        var assetId = Guid.NewGuid();
        var receivedAt = DateTime.UtcNow;

        IList<Sentinela.Domain.Entities.Sample> samples = [];

        var sampleRepository = ISampleWriteOnlyRepositoryBuilder.Build();
        Mock.Get(sampleRepository)
            .Setup(repository => repository.AddRange(It.IsAny<IList<Sentinela.Domain.Entities.Sample>>()))
            .Callback<IList<Sentinela.Domain.Entities.Sample>>(list => samples = list);

        var message = new SampleBatchMessage(assetId, Guid.NewGuid().ToString(), receivedAt, request);

        var useCase = new ProcessSampleBatchUseCase(sampleRepository, IAssetUpdateOnlyRepositoryBuilder.Build(), IUnitOfWorkBuilder.Build(), IIdempotencyCacheBuilder.Build());

        await useCase.Execute(message);

        samples.ShouldAllBe(sample => sample.AssetId == assetId);
        samples.ShouldAllBe(sample => sample.ReceivedAt == receivedAt);
        samples.Select(sample => sample.Value).ShouldBe(request.Select(item => item.Value));
        samples.Select(sample => sample.CollectedAt).ShouldBe(request.Select(item => item.CollectedAt));
        samples.Select(sample => sample.Metric.ToString()).ShouldBe(request.Select(item => item.Metric.ToString()));
    }

    [Fact]
    public async Task Success_ShouldNotPersist_WhenBatchIsDuplicated()
    {
        var request = RequestSampleBatchJsonBuilder.Build().Samples;
        var message = new SampleBatchMessage(Guid.NewGuid(), Guid.NewGuid().ToString(), DateTime.UtcNow, request);

        var sampleRepository = ISampleWriteOnlyRepositoryBuilder.Build();
        var assetRepository = IAssetUpdateOnlyRepositoryBuilder.Build();
        var unitOfWork = IUnitOfWorkBuilder.Build();

        var useCase = new ProcessSampleBatchUseCase(sampleRepository, assetRepository, unitOfWork, IIdempotencyCacheBuilder.Build(registered: false));

        await useCase.Execute(message);

        Mock.Get(sampleRepository).Verify(repository => repository.AddRange(It.IsAny<IList<Sentinela.Domain.Entities.Sample>>()), Times.Never);
        Mock.Get(assetRepository).Verify(repository => repository.UpdateLastSeen(It.IsAny<Guid>(), It.IsAny<DateTime>()), Times.Never);
        Mock.Get(unitOfWork).Verify(uow => uow.Commit(), Times.Never);
    }

    [Fact]
    public async Task ShouldRemoveIdempotencyKey_WhenCommitFails()
    {
        var request = RequestSampleBatchJsonBuilder.Build().Samples;
        var assetId = Guid.NewGuid();
        var idempotencyKey = Guid.NewGuid().ToString();
        var message = new SampleBatchMessage(assetId, idempotencyKey, DateTime.UtcNow, request);

        var unitOfWork = IUnitOfWorkBuilder.Build();
        Mock.Get(unitOfWork).Setup(uow => uow.Commit()).ThrowsAsync(new InvalidOperationException());

        var idempotencyCache = IIdempotencyCacheBuilder.Build();

        var useCase = new ProcessSampleBatchUseCase(ISampleWriteOnlyRepositoryBuilder.Build(), IAssetUpdateOnlyRepositoryBuilder.Build(), unitOfWork, idempotencyCache);

        await useCase.Execute(message).ShouldThrowAsync<InvalidOperationException>();

        Mock.Get(idempotencyCache).Verify(cache => cache.Remove($"idempotency:{assetId}:{idempotencyKey}"), Times.Once);
    }
}
