using System.Net;
using System.Net.Http.Json;
using ApiRefactor.Contracts.Requests;
using ApiRefactor.Contracts.Responses;
using ApiRefactor.Tests.Support;
using Xunit;

namespace ApiRefactor.Tests;

public sealed class WavesApiTests : IClassFixture<ApiWebApplicationFactory>, IDisposable
{
    private readonly ApiWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public WavesApiTests(ApiWebApplicationFactory factory)
    {
        _factory = factory;
        _factory.RecordingPublisher.Clear();
        _client = _factory.CreateClient();
    }

    public void Dispose() => _client.Dispose();

    [Fact]
    public async Task Get_waves_returns_200_and_json()
    {
        var response = await _client.GetAsync("/api/v1/waves");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<WavesListResponse>();
        Assert.NotNull(body);
        Assert.NotNull(body!.Items);
    }

    [Fact]
    public async Task Get_wave_by_unknown_id_returns_404()
    {
        var id = Guid.NewGuid();
        var response = await _client.GetAsync($"/api/v1/waves/{id}");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Post_wave_with_empty_name_returns_400()
    {
        var response = await _client.PostAsJsonAsync(
            "/api/v1/waves",
            new UpsertWaveRequest { Name = "", WaveDate = DateTime.UtcNow });
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Post_new_wave_returns_201_and_publishes_event()
    {
        var id = Guid.NewGuid();
        var response = await _client.PostAsJsonAsync(
            "/api/v1/waves",
            new UpsertWaveRequest { Id = id, Name = "Morning pick", WaveDate = new DateTime(2026, 5, 10, 8, 0, 0, DateTimeKind.Utc) });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var created = await response.Content.ReadFromJsonAsync<WaveResponse>();
        Assert.NotNull(created);
        Assert.Equal(id, created!.Id);
        Assert.Equal("Morning pick", created.Name);

        var events = _factory.RecordingPublisher.Events;
        Assert.Single(events);
        Assert.Equal(id, events[0].WaveId);
        Assert.True(events[0].WasInserted);

        var get = await _client.GetFromJsonAsync<WaveResponse>($"/api/v1/waves/{id}");
        Assert.NotNull(get);
        Assert.Equal("Morning pick", get!.Name);
    }

    [Fact]
    public async Task Post_existing_wave_returns_200_and_marks_update_in_event()
    {
        var id = Guid.NewGuid();
        await _client.PostAsJsonAsync(
            "/api/v1/waves",
            new UpsertWaveRequest { Id = id, Name = "First", WaveDate = DateTime.UtcNow });
        _factory.RecordingPublisher.Clear();

        var response = await _client.PostAsJsonAsync(
            "/api/v1/waves",
            new UpsertWaveRequest { Id = id, Name = "Second", WaveDate = DateTime.UtcNow });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var events = _factory.RecordingPublisher.Events;
        Assert.Single(events);
        Assert.False(events[0].WasInserted);
        Assert.Equal("Second", events[0].Name);
    }

    [Fact]
    public async Task Post_wave_with_quote_in_name_round_trips()
    {
        var id = Guid.NewGuid();
        const string tricky = "O'Brien's aisle-12";
        await _client.PostAsJsonAsync(
            "/api/v1/waves",
            new UpsertWaveRequest { Id = id, Name = tricky, WaveDate = DateTime.UtcNow });

        var get = await _client.GetFromJsonAsync<WaveResponse>($"/api/v1/waves/{id}");
        Assert.Equal(tricky, get!.Name);
    }
}
