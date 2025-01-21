using HtmlAgilityPack;
using PuppeteerSharp;
using System.Drawing;
using ZXing;
using ZXing.Windows.Compatibility;

namespace SimpleIdServer.IdServer.AuthFaker;

public class QrCodeHelper
{
    public async static Task<string> ReadQrCode(Options options)
    {
        var src = await GetImageSrcFromRunningChromeInstance(options);
        return ReadQrCode(src);
    }

    public async static Task<byte[]?> GetImageSrcFromRunningChromeInstance(Options options)
    {
        var browser = await Puppeteer.ConnectAsync(new ConnectOptions
        {
            BrowserWSEndpoint = $"ws://localhost:{options.Port}/devtools/browser/{options.BrowserId}"
        });
        var pages = await browser.PagesAsync();
        var targetPage = pages.FirstOrDefault(p => p.Url.StartsWith(options.Url));
        if (targetPage == null) return null;
        var htmlContent = await targetPage.GetContentAsync();
        var htmlDocument = new HtmlDocument();
        htmlDocument.LoadHtml(htmlContent);
        var pictureContainerImg = htmlDocument.DocumentNode.SelectSingleNode("//div[@class='picture-container']/img");
        var srcAttribute = pictureContainerImg?.GetAttributeValue("src", null);
        if (string.IsNullOrWhiteSpace(srcAttribute)) return null;
        using (var newPage = await browser.NewPageAsync())
        {
            var tcs = new TaskCompletionSource();
            byte[]? payload = null;
            newPage.Response += async (sender, e) =>
            {
                await e.Response.BufferAsync().AsTask().ContinueWith(async (data) => payload = await data);
                tcs.TrySetResult();
            };
            await newPage.GoToAsync(srcAttribute);
            var pageContent = await newPage.GetContentAsync();
            await tcs.Task;
            await newPage.CloseAsync();
            browser.Disconnect();
            return payload;
        }
    }

    private static string ReadQrCode(byte[] payload)
    {
        var reader = new BarcodeReader();
        using(var str = new MemoryStream(payload))
        {
            var img = (Bitmap)Image.FromStream(str);
            var qrCodeResult = reader.Decode(img);
            return qrCodeResult.Text;
        }
    }
}