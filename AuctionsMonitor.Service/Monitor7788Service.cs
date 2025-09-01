using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;
using AuctionsMonitor.Model;
using Microsoft.Extensions.Hosting;
using System.Collections.Generic;

namespace AuctionsMonitor.Service
{
    public class Monitor7788Service : BackgroundService
    {
        private const string SearchBaseUrl = "https://www.997788.com/all_0/0/?searchtype=1&www=all&t2=0&s0=";
        private const string BaseUrl = "https://www.997788.com";
        private List<string> _searchKeyWords = [];
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _searchKeyWords = ["邓丽君"];
            await Search(stoppingToken);

            try
            {
                await Task.Delay(Timeout.Infinite, stoppingToken);
            }
            catch (TaskCanceledException) { }
        }

        private async Task<List<Monitor7788Model>> Search(CancellationToken stoppingToken)
        {
            var htmlParser = new HtmlParser();
            var list = new List<Monitor7788Model>();
            foreach (var keyWord in _searchKeyWords)
            {
                if (stoppingToken.IsCancellationRequested)
                {
                    return list;
                }

                var fullUrl = $"{SearchBaseUrl}{Uri.EscapeDataString(keyWord)}";

                using var httpClient = new HttpClient();

                var html = await httpClient.GetStreamAsync(fullUrl, stoppingToken);

                var document = await htmlParser.ParseDocumentAsync(html, stoppingToken);

                // Every item is in a div with class "tbc"
                // Every item is a line with a shop link with full information
                var items = document.GetElementsByClassName("tbc");

                foreach (var item in items)
                {
                    // Every item contains 5 tds
                    var tds = item.GetElementsByTagName("td");

                    var titleAnchorElement = tds[1].GetElementsByTagName("a")[0] as IHtmlAnchorElement;
                    var title = titleAnchorElement?.Text ?? "";
                    var urlRelative = titleAnchorElement?.PathName ?? "";
                    var url = $"{BaseUrl}{urlRelative}";

                    var coverPhotoImageElement = tds[2].GetElementsByTagName("img")[0] as IHtmlImageElement;
                    var urlCoverPhoto = coverPhotoImageElement?.Source ?? "";

                    var countButtonAuction = tds[5].GetElementsByClassName("botton_auction_direct").Length;
                    var isAuctions = countButtonAuction > 0;

                    var td4 = tds[4];
                    var td4Strongs = td4.GetElementsByTagName("strong");
                    var priceNow = td4Strongs.Length > 0 ? td4Strongs[0].TextContent?.Trim() ?? "" : "";

                    var td4Fonts = td4.GetElementsByTagName("font");
                    var leftTime = td4Fonts.Length > 1 ? td4Fonts[1].TextContent?.Trim() ?? "" : null;

                    var model = new Monitor7788Model
                    {
                        IsAuctions = isAuctions,
                        LeftTime = leftTime,
                        PriceNow = priceNow,
                        Title = title,
                        Url = url,
                        UrlCoverPhoto = urlCoverPhoto
                    };

                    list.Add(model);
                }
            }

            return list;
        }

    }
}
