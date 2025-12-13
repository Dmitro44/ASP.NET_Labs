namespace WEB_Lab13.Core.Entities;

public class Player
{
    public string? UserId { get; set; }
    public string? ConnectionId { get; set; }
    public string? Name { get; set; }
    public int Score { get; set; }
    public bool IsTurn { get; set; }
}
