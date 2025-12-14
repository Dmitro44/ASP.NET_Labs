using System.Collections.Generic;

namespace WEB_Lab13.Core.Logic
{
    public class GameState
    {
        public string? GameId { get; set; }
        public List<PlayerState> Players { get; set; } = new List<PlayerState>();
        public int CurrentPlayerIndex { get; set; }
        public int TurnScore { get; set; }
        public string LastRoll { get; set; } = "";
        public string StatusMessage { get; set; } = "Ожидание начала игры...";
        public bool IsGameRunning { get; set; } = false;
    }

    public class PlayerState
    {
        public string? ConnectionId { get; set; }
        public string? Username { get; set; }
        public int TotalScore { get; set; }
        public bool IsMyTurn { get; set; } = false;
    }

    public class PigGameLogic
    {
        private readonly GameState _state;
        private readonly Random _random = new Random();

        public PigGameLogic(string gameId, Dictionary<string, string> players)
        {
            _state = new GameState { GameId = gameId };

            foreach (var player in players)
            {
                _state.Players.Add(new PlayerState
                {
                    ConnectionId = player.Key,
                    Username = player.Value,
                    TotalScore = 0
                });
            }

            if (_state.Players.Count > 0)
            {
                _state.CurrentPlayerIndex = _random.Next(0, _state.Players.Count);
                _state.Players[_state.CurrentPlayerIndex].IsMyTurn = true;
                _state.IsGameRunning = _state.Players.Count == 2;
                _state.StatusMessage = _state.IsGameRunning
                    ? $"Игра началась. Ход игрока {_state.Players[_state.CurrentPlayerIndex].Username}."
                    : "Ожидание второго игрока...";
            }
        }

        public GameState AddPlayer(string connectionId, string username)
        {
            if (_state.Players.Count >= 2) return _state;

            _state.Players.Add(new PlayerState
            {
                ConnectionId = connectionId,
                Username = username,
                TotalScore = 0
            });

            _state.StatusMessage = $"Игрок {username} присоединился. ";

            if (_state.Players.Count == 2)
            {
                _state.IsGameRunning = true;
                _state.StatusMessage += "Игра началась!";
            }

            return _state;
        }

        public GameState GetState() => _state;

        public GameState RollDice()
        {
            if (!_state.IsGameRunning) return _state;

            int roll1 = _random.Next(1, 7);
            int roll2 = _random.Next(1, 7);
            _state.LastRoll = $"{roll1} and {roll2}";

            if (roll1 == 1 && roll2 == 1)
            {
                _state.Players[_state.CurrentPlayerIndex].TotalScore = 0;
                _state.TurnScore = 0;
                _state.StatusMessage = $"Выпало две 1! Весь ваш счет сгорел! Ход переходит к следующему игроку.";
                NextTurn();
            }
            else if (roll1 == 1 || roll2 == 1)
            {
                _state.TurnScore = 0;
                _state.StatusMessage = $"Выпала 1! Очки за ход сгорели. Ход переходит к следующему игроку.";
                NextTurn();
            }
            else
            {
                _state.TurnScore += roll1 + roll2;
                _state.StatusMessage = $"Выпало {roll1} и {roll2}. В этом ходу: {_state.TurnScore}.";
            }

            return _state;
        }

        public GameState Hold()
        {
            if (!_state.IsGameRunning) return _state;

            var currentPlayer = _state.Players[_state.CurrentPlayerIndex];
            currentPlayer.TotalScore += _state.TurnScore;
            _state.StatusMessage =
                $"{currentPlayer.Username} забрал очки ({_state.TurnScore}). Итого: {currentPlayer.TotalScore}.";
            _state.TurnScore = 0;

            if (currentPlayer.TotalScore >= 100)
            {
                _state.IsGameRunning = false;
                _state.StatusMessage = $"{currentPlayer.Username} ПОБЕДИЛ! Игра окончена.";
            }
            else
            {
                NextTurn();
            }

            return _state;
        }

        private void NextTurn()
        {
            _state.Players[_state.CurrentPlayerIndex].IsMyTurn = false;
            _state.CurrentPlayerIndex = (_state.CurrentPlayerIndex + 1) % _state.Players.Count;
            _state.Players[_state.CurrentPlayerIndex].IsMyTurn = true;

            if (_state.IsGameRunning)
            {
                _state.StatusMessage += $" Теперь ход игрока {_state.Players[_state.CurrentPlayerIndex].Username}.";
            }
        }
    }
}