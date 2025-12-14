using WEB_Lab13.Core.Logic;


public interface IGameClient
{
    Task ReceiveGameState(GameState state); 
    
    Task JoinGame(string gameId);
}