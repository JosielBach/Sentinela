using CommonTestUtilities.Entities;
using CommonTestUtilities.Requests;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using Sentinela.Application.UseCases.Sample;
using Sentinela.Domain.Messaging;
using Sentinela.Infrastructure.DataAccess;
using Sentinela.Infrastructure.Messaging;
using Shouldly;
using System.Text;

namespace Worker.Tests.Sample.Process;

public class IngestionConsumerWorkerTests : IClassFixture<SentinelaWorkerFactory>
{
    private static readonly TimeSpan TIMEOUT = TimeSpan.FromSeconds(30);
    private const string DEATH_REASON_HEADER = "x-first-death-reason";
    private readonly SentinelaWorkerFactory _factory;

    public IngestionConsumerWorkerTests(SentinelaWorkerFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Success()
    {
        var assetId = await CreateAsset();
        var receivedAt = DateTime.UtcNow;
        var message = new SampleBatchMessage(assetId, Guid.NewGuid().ToString(), receivedAt, RequestSampleBatchJsonBuilder.Build().Samples);

        await Publish(message);

        await WaitUntil(async () => await CountSamples(assetId) == message.Samples.Count);

        await using var scope = _factory.Services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<SentinelaDbContext>();
        var asset = await dbContext.Assets.AsNoTracking().FirstAsync(asset => asset.Id == assetId);

        asset.LastSeenAt!.Value.ShouldBe(receivedAt, TimeSpan.FromMilliseconds(1));
    }

    [Fact]
    public async Task Success_Duplicated_Batch()
    {
        var assetId = await CreateAsset();
        var duplicated = new SampleBatchMessage(assetId, Guid.NewGuid().ToString(), DateTime.UtcNow, RequestSampleBatchJsonBuilder.Build().Samples);
        var other = new SampleBatchMessage(assetId, Guid.NewGuid().ToString(), DateTime.UtcNow, RequestSampleBatchJsonBuilder.Build().Samples);
        var expectedCount = duplicated.Samples.Count + other.Samples.Count;

        await Publish(duplicated);
        await Publish(duplicated);
        await Publish(other);

        await WaitUntil(async () => await CountSamples(assetId) >= expectedCount);

        (await CountSamples(assetId)).ShouldBe(expectedCount);
    }

    [Fact]
    public async Task Error_Invalid_Message()
    {
        await using var channel = await _factory.Connection.CreateChannelAsync();

        await channel.BasicPublishAsync(RabbitMqTopology.EXCHANGE, RabbitMqTopology.ROUTING_KEY, Encoding.UTF8.GetBytes("abc"));

        var deadLetter = await GetDeadLetter();

        Encoding.UTF8.GetString(deadLetter.Body.Span).ShouldBe("abc");
        DeathReason(deadLetter).ShouldBe("rejected");
    }

    [Fact]
    public async Task Error_Delivery_Limit_Reached()
    {
        var message = new SampleBatchMessage(Guid.NewGuid(), Guid.NewGuid().ToString(), DateTime.UtcNow, RequestSampleBatchJsonBuilder.Build().Samples);

        await Publish(message);

        var deadLetter = await GetDeadLetter();

        Encoding.UTF8.GetString(deadLetter.Body.Span).ShouldContain(message.IdempotencyKey);
        DeathReason(deadLetter).ShouldBe("delivery_limit");
    }

    private async Task<Guid> CreateAsset()
    {
        await using var scope = _factory.Services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<SentinelaDbContext>();

        var asset = AssetBuilder.Collection(1)[0];

        await dbContext.Assets.AddAsync(asset);
        await dbContext.SaveChangesAsync();

        return asset.Id;
    }

    private async Task Publish(SampleBatchMessage message)
    {
        await using var scope = _factory.Services.CreateAsyncScope();
        var queuePublisher = scope.ServiceProvider.GetRequiredService<IQueuePublisher>();

        await queuePublisher.Publish(message);
    }

    private async Task<int> CountSamples(Guid assetId)
    {
        await using var scope = _factory.Services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<SentinelaDbContext>();

        return await dbContext.Samples.CountAsync(sample => sample.AssetId == assetId);
    }

    private async Task<BasicGetResult> GetDeadLetter()
    {
        await using var channel = await _factory.Connection.CreateChannelAsync();

        BasicGetResult? result = null;

        await WaitUntil(async () =>
        {
            result = await channel.BasicGetAsync(RabbitMqTopology.DEAD_LETTER_QUEUE, autoAck: true);
            return result is not null;
        });

        return result!;
    }

    private static string DeathReason(BasicGetResult deadLetter)
    {
        return Encoding.UTF8.GetString((byte[])deadLetter.BasicProperties.Headers![DEATH_REASON_HEADER]!);
    }

    private static async Task WaitUntil(Func<Task<bool>> condition)
    {
        var limit = DateTime.UtcNow.Add(TIMEOUT);

        while (!await condition())
        {
            if (DateTime.UtcNow > limit)
                throw new TimeoutException();

            await Task.Delay(200);
        }
    }
}
