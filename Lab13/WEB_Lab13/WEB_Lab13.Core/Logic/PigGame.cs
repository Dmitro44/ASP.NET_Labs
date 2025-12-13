using WEB_Lab13.Core.Entities;

namespace WEB_Lab13.Core.Logic;

public class PigGame
{
    public string Id { get; private set; }
    public List<Player> Players { get; } = new();
    public int CurrentTurnScore { get; private set; }
    public int CurrentDieValue { get; private set; }
    public bool IsGameOver { get; private set; }
    public bool IsGameStarted { get; private set; }
    public Player? Winner { get; private set; }
    private int _currentPlayerIndex;
    private const int WinningScore = 100;

    public PigGame(string id)
    {
        Id = id;
    }

    public void AddPlayer(Player player)
    {
        if (IsGameStarted || IsGameOver) return;
        
        // Проверяем, не добавлен ли уже этот игрок
        if (Players.Any(p => p.ConnectionId == player.ConnectionId)) return;
        
        if (Players.Count < 2)
        {
            Players.Add(player);
        }
    }

    public void RemovePlayer(Player player)
    {
        Players.Remove(player);
    }

    public void StartGame()
    {
        if (IsGameStarted || IsGameOver) return;
        
        if (Players.Count >= 2)
        {
            IsGameStarted = true;
            _currentPlayerIndex = 0;
            CurrentTurnScore = 0;
            CurrentDieValue = 0;
            foreach (var player in Players)
            {
                player.Score = 0;
                player.IsTurn = false;
            }
            Players[_currentPlayerIndex].IsTurn = true;
        }
    }

    public void Roll(string playerId)
    {
        if (!IsGameStarted || IsGameOver || GetCurrentPlayer()?.ConnectionId != playerId) return;

        var random = new Random();
        CurrentDieValue = random.Next(1, 7);

        if (CurrentDieValue == 1)
        {
            CurrentTurnScore = 0;
            NextTurn();
        }
        else
        {
            CurrentTurnScore += CurrentDieValue;
        }
    }

    public void Hold(string playerId)
    {
        if (!IsGameStarted || IsGameOver || GetCurrentPlayer()?.ConnectionId != playerId) return;

        Players[_currentPlayerIndex].Score += CurrentTurnScore;
        CurrentTurnScore = 0;
        CurrentDieValue = 0;

        if (Players[_currentPlayerIndex].Score >= WinningScore)
        {
            IsGameOver = true;
            Winner = Players[_currentPlayerIndex];
        }
        else
        {
            NextTurn();
        }
    }
    
    public Player? GetCurrentPlayer()
    {
        return Players.Count > 0 ? Players[_currentPlayerIndex] : null;
    }

    private void NextTurn()
    {
        Players[_currentPlayerIndex].IsTurn = false;
        _currentPlayerIndex = (_currentPlayerIndex + 1) % Players.Count;
        Players[_currentPlayerIndex].IsTurn = true;
    }
}
