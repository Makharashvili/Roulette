namespace Domain.Data
{
    public class Round : BaseEntity<int>
    {
        public string IpAddress { get; set; }
        public string BetString { get; set; }
        public int WinningResult { get; set; }
        /// <summary>
        /// Bet amount in cents
        /// </summary>
        public long BetAmount { get; set; }
        /// <summary>
        /// Won amount in cents
        /// </summary>
        public long WonAmount { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
    }
}
