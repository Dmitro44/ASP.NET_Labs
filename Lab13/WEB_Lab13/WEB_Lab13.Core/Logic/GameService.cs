using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace WEB_Lab13.Core.Logic;

public class GameService
{
    private static readonly ConcurrentDictionary<string, PigGame> Games = new();

    public PigGame CreateGame(string gameId)
    {
        var game = new PigGame(gameId);
        Games.TryAdd(gameId, game);
        return game;
    }

    public PigGame? GetGame(string gameId)
    {
        Games.TryGetValue(gameId, out var game);
        return game;
    }

    public PigGame? GetGameByPlayerId(string playerId)
    {
        return Games.Values.FirstOrDefault(g => g.Players.Any(p => p.ConnectionId == playerId));
    }

    public void RemoveGame(string gameId)
    {
        Games.TryRemove(gameId, out _);
    }
}
