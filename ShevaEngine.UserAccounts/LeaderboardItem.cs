namespace ShevaEngine.UserAccounts
{
    /// <summary>
    /// Leader board item.
    /// </summary>
    public class LeaderboardItem<T>
    {
        public string GamerName { get; set; }
        public uint Rank { get; set; }
        public T Score { get; set; }
    }
}
