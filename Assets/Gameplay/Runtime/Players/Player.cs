namespace Gameplay.Runtime.Players
{
    public class Player
    {
        public string Name;
        public int PlayerId;
        public Player(string name, int playerId)
        {
            Name = name;
            PlayerId = playerId;
        }
    }
}