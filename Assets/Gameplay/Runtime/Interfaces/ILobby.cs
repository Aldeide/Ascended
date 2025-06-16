namespace Gameplay.Runtime.Interfaces
{
    public interface ILobby
    {
        public void AddPlayer(IPlayer player);
        public void RemovePlayer(IPlayer player);
        public void SetPlayerReady(IPlayer player);
        public void StartGame();
    }
}