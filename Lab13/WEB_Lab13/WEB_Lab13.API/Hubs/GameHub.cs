using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using WEB_Lab13.Core.Logic;
using System.Collections.Concurrent;
using WEB_Lab13.API.Hubs;

namespace WEB_Lab13.API.Hubs;

[Authorize]
public class GameHub : Hub<IGameClient>
{
    private static readonly ConcurrentDictionary<string, PigGameLogic> ActiveGames = 
        new ConcurrentDictionary<string, PigGameLogic>();
    
    private static readonly ConcurrentDictionary<string, string> PlayerGameMap = 
        new ConcurrentDictionary<string, string>();
    private readonly ILogger<GameHub> _logger;

    public GameHub(ILogger<GameHub> logger)
    {
        _logger = logger;
    }

    public async Task CreateGame(string username)
    {
        string gameId = Guid.NewGuid().ToString();
        var players = new Dictionary<string, string> { { Context.ConnectionId, username } };
        var game = new PigGameLogic(gameId, players);
        ActiveGames.TryAdd(gameId, game);
        
        await Groups.AddToGroupAsync(Context.ConnectionId, gameId);
        PlayerGameMap.TryAdd(Context.ConnectionId, gameId);
        
        _logger.LogInformation("Player {Username} created a new game with ID {GameId}", username, gameId);
        
        await Clients.Caller.JoinGame(gameId);
        await Clients.Group(gameId).ReceiveGameState(game.GetState());
    }
    
    public async Task JoinGame(string gameId, string username)
    {
        gameId = gameId.Trim();
        if (ActiveGames.TryGetValue(gameId, out var game))
        {
            if (game.GetState().Players.Count >= 2)
            {
                _logger.LogWarning("Player {Username} failed to join game {GameId}: game is full", username, gameId);
                return; 
            }
            
            var updatedState = game.AddPlayer(Context.ConnectionId, username);
            await Groups.AddToGroupAsync(Context.ConnectionId, gameId);
            PlayerGameMap.TryAdd(Context.ConnectionId, gameId);
            
            _logger.LogInformation("Player {Username} joined game {GameId}", username, gameId);
            
            await Clients.Caller.JoinGame(gameId);
            
            await Clients.Group(gameId).ReceiveGameState(updatedState);
        }
        else
        {
            _logger.LogWarning("Player {Username} failed to join game {GameId}: game not found", username, gameId);
        }
    }
    
    public async Task RollDice()
    {
        if (PlayerGameMap.TryGetValue(Context.ConnectionId, out var gameId) && 
            ActiveGames.TryGetValue(gameId, out var game))
        {
            if (!game.GetState().IsGameRunning)
            {
                _logger.LogWarning("Player {ConnectionId} tried to roll dice in game {GameId} which is not running", Context.ConnectionId, gameId);
                return;
            }
            var currentPlayer = game.GetState().Players[game.GetState().CurrentPlayerIndex];
            if (currentPlayer.ConnectionId != Context.ConnectionId)
            {
                _logger.LogWarning("Player {ConnectionId} tried to roll dice out of turn in game {GameId}", Context.ConnectionId, gameId);
                return;
            }
            
            var newState = game.RollDice();
            _logger.LogInformation("Player {ConnectionId} rolled the dice in game {GameId}", Context.ConnectionId, gameId);
            await Clients.Group(gameId).ReceiveGameState(newState);
        }
    }
    
    public async Task Hold()
    {
        if (PlayerGameMap.TryGetValue(Context.ConnectionId, out var gameId) && 
            ActiveGames.TryGetValue(gameId, out var game))
        {
            if (!game.GetState().IsGameRunning)
            {
                _logger.LogWarning("Player {ConnectionId} tried to hold in game {GameId} which is not running", Context.ConnectionId, gameId);
                return;
            }
           
            var currentPlayer = game.GetState().Players[game.GetState().CurrentPlayerIndex];
            if (currentPlayer.ConnectionId != Context.ConnectionId)
            {
                _logger.LogWarning("Player {ConnectionId} tried to hold out of turn in game {GameId}", Context.ConnectionId, gameId);
                return;
            }
            
            var newState = game.Hold();
            _logger.LogInformation("Player {ConnectionId} held in game {GameId}", Context.ConnectionId, gameId);
            await Clients.Group(gameId).ReceiveGameState(newState);
        }
    }
    

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        if (PlayerGameMap.TryRemove(Context.ConnectionId, out var gameId))
        {
            _logger.LogInformation("Player {ConnectionId} disconnected from game {GameId}", Context.ConnectionId, gameId);
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, gameId);
        }
        await base.OnDisconnectedAsync(exception);
    }
}