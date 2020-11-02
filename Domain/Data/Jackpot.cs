namespace Domain.Data
{
    public class Jackpot : BaseEntity<int>
    {
        /// <summary>
        /// Jackpot amount is in millicents, we cant predict what minimal bet amount can be
        /// and if someone bets 1 cent or so the jackpot amount will be very small and it still might be added
        /// </summary>
        public long Amount { get; set; }
    }
}
