namespace ServiceModels
{
    public class JwtConfiguration
    {
        public string Key { get; set; }
        public int LifetimeMin { get; set; }
    }
}
