using CommonTestUtilities.Requests;
using Sentinela.Application.UseCases.Sample;
using Sentinela.Domain.Extensions;
using Sentinela.Exception;
using Shouldly;
using System.Globalization;
using System.Net;
using System.Text.Json;
using WebApi.Tests.Resources;

namespace WebApi.Tests.Sample.Ingest;

public class IngestSampleBatchTests : BaseIntegrationTest
{
    private const string REQUEST_URI = "/api/v1/ingest";
    private const string AGENT_KEY_HEADER = "X-Agent-Key";
    private const string IDEMPOTENCY_KEY_HEADER = "Idempotency-Key";
    private readonly FakeQueuePublisher _queuePublisher;
    private readonly string _activeAssetApiKey;
    private readonly string _inactiveAssetApiKey;
    private readonly Guid _activeAssetId;
    private readonly string _idempotencyKey = Guid.NewGuid().ToString();

    public IngestSampleBatchTests(SentinelaApplicationFactory factory) : base(factory)
    {
        _queuePublisher = factory.QueuePublisher;
        _activeAssetApiKey = factory.ACTIVE_ASSET_API_KEY;
        _inactiveAssetApiKey = factory.INACTIVE_ASSET_API_KEY;
        _activeAssetId = factory.ActiveAssetId;
    }

    [Fact]
    public async Task Success()
    {
        var request = RequestSampleBatchJsonBuilder.Build();
        var headers = new Dictionary<string, string>
        {
            [AGENT_KEY_HEADER] = _activeAssetApiKey,
            [IDEMPOTENCY_KEY_HEADER] = _idempotencyKey
        };

        var response = await Post(REQUEST_URI, request, headers: headers);

        response.StatusCode.ShouldBe(HttpStatusCode.Accepted);

        var messages = PublishedMessages();

        messages.Count.ShouldBe(1);
        messages[0].AssetId.ShouldBe(_activeAssetId);
        messages[0].Samples.Count.ShouldBe(request.Samples.Count);
    }

    [Fact]
    public async Task Error_Agent_Key_Invalid()
    {
        var request = RequestSampleBatchJsonBuilder.Build();
        var headers = new Dictionary<string, string>
        {
            [AGENT_KEY_HEADER] = "chaveInvalida",
            [IDEMPOTENCY_KEY_HEADER] = _idempotencyKey
        };

        var response = await Post(REQUEST_URI, request, headers: headers);

        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
        PublishedMessages().ShouldBeEmpty();
    }

    [Fact]
    public async Task Error_Agent_Key_From_Inactive_Asset()
    {
        var request = RequestSampleBatchJsonBuilder.Build();
        var headers = new Dictionary<string, string>
        {
            [AGENT_KEY_HEADER] = _inactiveAssetApiKey,
            [IDEMPOTENCY_KEY_HEADER] = _idempotencyKey
        };

        var response = await Post(REQUEST_URI, request, headers: headers);

        response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
        PublishedMessages().ShouldBeEmpty();
    }

    [Theory]
    [InlineData("en")]
    [InlineData("pt-BR")]
    public async Task Validate_ShouldBeAnErrorResponse_WhenIdempotencyKeyIsMissing(string culture)
    {
        var request = RequestSampleBatchJsonBuilder.Build();
        var headers = new Dictionary<string, string>
        {
            [AGENT_KEY_HEADER] = _activeAssetApiKey
        };
        var messagesBefore = _queuePublisher.Messages.Count;

        var response = await Post(REQUEST_URI, request, culture: culture, headers: headers);

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);

        await using var responseBody = await response.Content.ReadAsStreamAsync();

        var responseData = await JsonDocument.ParseAsync(responseBody);

        var errors = responseData.RootElement.GetProperty("errors").EnumerateArray();
        var errorMessage = ResourceMessagesException.ResourceManager.GetString("VALIDATION_IDEMPOTENCY_KEY_REQUIRED", new CultureInfo(culture));

        errors.ShouldSatisfyAllConditions(errorsList =>
        {
            errorsList.Count().ShouldBe(1);
            errorsList.ShouldContain(error => error.GetString().IsNotEmpty()! && error.GetString()!.Equals(errorMessage));
        });

        _queuePublisher.Messages.Count.ShouldBe(messagesBefore);
    }

    [Theory]
    [InlineData("en")]
    [InlineData("pt-BR")]
    public async Task Validate_ShouldBeAnErrorResponse_WhenSamplesIsEmpty(string culture)
    {
        var request = RequestSampleBatchJsonBuilder.Build();
        request.Samples.Clear();

        var headers = new Dictionary<string, string>
        {
            [AGENT_KEY_HEADER] = _activeAssetApiKey,
            [IDEMPOTENCY_KEY_HEADER] = _idempotencyKey
        };

        var response = await Post(REQUEST_URI, request, culture: culture, headers: headers);

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);

        await using var responseBody = await response.Content.ReadAsStreamAsync();

        var responseData = await JsonDocument.ParseAsync(responseBody);

        var errors = responseData.RootElement.GetProperty("errors").EnumerateArray();
        var errorMessage = ResourceMessagesException.ResourceManager.GetString("VALIDATION_SAMPLES_REQUIRED", new CultureInfo(culture));

        errors.ShouldSatisfyAllConditions(errorsList =>
        {
            errorsList.Count().ShouldBe(1);
            errorsList.ShouldContain(error => error.GetString().IsNotEmpty()! && error.GetString()!.Equals(errorMessage));
        });

        PublishedMessages().ShouldBeEmpty();
    }
    private List<SampleBatchMessage> PublishedMessages()
    {
        return _queuePublisher.Messages.OfType<SampleBatchMessage>()
            .Where(message => message.IdempotencyKey == _idempotencyKey).ToList();
    }
}
