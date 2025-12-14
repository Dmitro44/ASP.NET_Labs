using Microsoft.AspNetCore.SignalR.Client;
using WEB_Lab13.Core.Logic;
using Microsoft.JSInterop;

public class GameClient : IAsyncDisposable
{
    private readonly HubConnection _hubConnection;
    
    public event Action<GameState>? GameStateUpdated;
    public event Action<string>? GameJoined;

    public GameClient(HttpClient httpClient, IJSRuntime jsRuntime)
    {
        _hubConnection = new HubConnectionBuilder()
            .WithUrl("https://localhost:7107/gamehub", options =>
            {
                options.AccessTokenProvider = async () =>
                {
                    var token = await jsRuntime.InvokeAsync<string>("localStorage.getItem", "authToken");
                    Console.WriteLine($"SignalR token: {token}");
                    return token;
                };
            })
            .Build();
        
        _hubConnection.On<GameState>("ReceiveGameState", (state) =>
        {
            GameStateUpdated?.Invoke(state);
        });
        
        _hubConnection.On<string>("JoinGame", (gameId) =>
        {
            GameJoined?.Invoke(gameId);
        });
    }
    
    public async Task StartConnection()
    {
        if (_hubConnection.State == HubConnectionState.Disconnected)
        {
            await _hubConnection.StartAsync();
        }
    }
    
    public string? GetConnectionId()
    {
        return _hubConnection.ConnectionId;
    }
    
    public async Task CreateGame(string username) => 
        await _hubConnection.SendAsync("CreateGame", username);

    public async Task JoinExistingGame(string gameId, string username) => 
        await _hubConnection.SendAsync("JoinGame", gameId, username);

    public async Task RollDice() => 
        await _hubConnection.SendAsync("RollDice");

    public async Task Hold() => 
        await _hubConnection.SendAsync("Hold");

    public async ValueTask DisposeAsync()
    {
        await _hubConnection.DisposeAsync();
    }
}