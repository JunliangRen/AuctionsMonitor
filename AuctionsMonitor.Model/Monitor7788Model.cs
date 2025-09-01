namespace AuctionsMonitor.Model
{
    public class Monitor7788Model
    {
        public required string Title { get; set; }
        public required string Url { get; set; }
        public required string UrlCoverPhoto { get; set; }
        public required string PriceNow { get; set; }
        public string? LeftTime { get; set; }
        public bool IsAuctions { get; set; }
    }
}
