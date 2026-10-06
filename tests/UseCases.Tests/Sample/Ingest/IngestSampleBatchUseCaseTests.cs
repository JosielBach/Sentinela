using CommonTestUtilities.Messaging;
using CommonTestUtilities.Requests;
using Moq;
using Sentinela.Application.UseCases.Sample;
using Sentinela.Application.UseCases.Sample.Ingest;
using Sentinela.Communication.Enums;
using Sentinela.Exception;
using Sentinela.Exception.ExceptionBase;
using Shouldly;

namespace UseCases.Tests.Sample.Ingest;

public class IngestSampleBatchUseCaseTests
{
    [Fact]
    public async Task Success()
    {
        var request = RequestSampleBatchJsonBuilder.Build();
        var assetId = Guid.NewGuid();
        var idempotencyKey = Guid.NewGuid().ToString();

        var queuePublisher = new IQueuePublisherBuilder();
        var useCase = CreateUseCase(queuePublisher);

        await useCase.Execute(request, assetId, idempotencyKey);

        queuePublisher.Mock.Verify(publisher => publisher.Publish(It.Is<SampleBatchMessage>(message =>
            message.AssetId == assetId &&
            message.IdempotencyKey == idempotencyKey &&
            message.Samples == request.Samples)), Times.Once);
        queuePublisher.Mock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task Validate_ShouldThrowException_WhenSamplesIsEmpty()
    {
        var request = RequestSampleBatchJsonBuilder.Build();
        request.Samples.Clear();

        var assetId = Guid.NewGuid();
        var idempotencyKey = Guid.NewGuid().ToString();

        var queuePublisher = new IQueuePublisherBuilder();
        var useCase = CreateUseCase(queuePublisher);

        var exception = await useCase.Execute(request, assetId, idempotencyKey).ShouldThrowAsync<ErrorOnValidationException>();

        exception.GetErrorMessages().ShouldSatisfyAllConditions(errorMessage =>
        {
            errorMessage.Count.ShouldBe(1);
            errorMessage.ShouldContain(ResourceMessagesException.VALIDATION_SAMPLES_REQUIRED);
        });

        queuePublisher.Mock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task Validate_ShouldThrowException_WhenMetricIsInvalid()
    {
        var request = RequestSampleBatchJsonBuilder.Build();
        request.Samples[0].Metric = (MetricType)99;

        var assetId = Guid.NewGuid();
        var idempotencyKey = Guid.NewGuid().ToString();

        var queuePublisher = new IQueuePublisherBuilder();
        var useCase = CreateUseCase(queuePublisher);

        var exception = await useCase.Execute(request, assetId, idempotencyKey).ShouldThrowAsync<ErrorOnValidationException>();

        exception.GetErrorMessages().ShouldSatisfyAllConditions(errorMessage =>
        {
            errorMessage.Count.ShouldBe(1);
            errorMessage.ShouldContain(ResourceMessagesException.VALIDATION_METRIC_INVALID);
        });

        queuePublisher.Mock.VerifyNoOtherCalls();
    }
    private IngestSampleBatchUseCase CreateUseCase(IQueuePublisherBuilder queuePublisher)
    {
        return new IngestSampleBatchUseCase(queuePublisher.Build());
    }
}
