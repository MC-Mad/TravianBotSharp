namespace MainCore.Common.Extensions
{
    public static class ChromeBrowserExtension
    {
        public static HtmlDocument GetHtml(this IWebDriver driver)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(driver.PageSource);
            return doc;
        }

        public static Task<Result> Click(this IChromeBrowser browser, HtmlNode node, CancellationToken cancellationToken)
        {
            return browser.Click(By.XPath(node.XPath), cancellationToken);
        }

        public static Task<Result> Input(this IChromeBrowser browser, HtmlNode node, string content, CancellationToken cancellationToken)
        {
            return browser.Input(By.XPath(node.XPath), content, cancellationToken);
        }

        public static Task<Result> WaitHtml(this IChromeBrowser browser, Predicate<HtmlDocument> condition, CancellationToken cancellationToken)
        {
            return browser.Wait(driver => condition(driver.GetHtml()), cancellationToken);
        }

        public static Task<Result> WaitButtonClickable(this IChromeBrowser browser, Func<HtmlDocument, HtmlNode?> getButton, CancellationToken cancellationToken)
        {
            return browser.Wait(driver =>
            {
                var button = getButton(driver.GetHtml());
                if (button is null) return false;

                var elements = driver.FindElements(By.XPath(button.XPath));
                return elements.Count > 0 && elements[0].Enabled;
            }, cancellationToken);
        }

        public static async Task<Result> ClickAndWait(this IChromeBrowser browser, HtmlNode node, Predicate<HtmlDocument> condition, CancellationToken cancellationToken)
        {
            var result = await browser.Click(node, cancellationToken);
            if (result.IsFailed) return result;

            return await browser.WaitHtml(condition, cancellationToken);
        }

        public static async Task<Result> ClickAndWaitPageChanged(this IChromeBrowser browser, HtmlNode node, string part, CancellationToken cancellationToken)
        {
            var result = await browser.Click(node, cancellationToken);
            if (result.IsFailed) return result;

            return await browser.WaitPageChanged(part, cancellationToken);
        }

        public static async Task<Result> ClickAndWaitPageChanged(this IChromeBrowser browser, HtmlNode node, string part, Predicate<HtmlDocument> condition, CancellationToken cancellationToken)
        {
            var result = await browser.Click(node, cancellationToken);
            if (result.IsFailed) return result;

            return await browser.WaitPageChanged(part, driver => condition(driver.GetHtml()), cancellationToken);
        }
    }
}
