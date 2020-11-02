using System;

namespace ServiceModels.RoundModels
{
    public class RoundModel
    {
        public int Id { get; set; }
        public string BetString { get; set; }
        public int Winningresult { get; set; }
        public long BetAmount { get; set; }
        public long WonAmount { get; set; }
        public string IpAddress { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
