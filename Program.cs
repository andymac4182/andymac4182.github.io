using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Statiq.App;
using Statiq.Common;
using Statiq.Web;

namespace Website
{
    internal class Program
    {
        public static async Task<int> Main(string[] args) =>
         await Bootstrapper
                .Factory
                .CreateWeb(args)
                .AddSetting(Keys.Host, "andrewmac.cloud")
                .AddSetting(Keys.LinksUseHttps, true)
                .AddSetting(
                    Keys.DestinationPath,
                    Config.FromDocument((doc, ctx) =>
                    {
                        // Only applies to the content pipeline
                        if (ctx.PipelineName == nameof(Statiq.Web.Pipelines.Content))
                        {
                            return doc.Source.Parent.Segments.Last().SequenceEqual("posts".AsMemory())
                                ? new NormalizedPath("blog").Combine(doc.GetDateTime(WebKeys.Published).ToString("yyyy/MM/dd")).Combine(NormalizedPath.OptimizeFileName(doc.GetString(WebKeys.Title))).Combine("index.html")
                                : doc.Destination.ChangeExtension(".html");
                        }
                        return doc.Destination;
                    }))
                .AddSetting("EditLink", Config.FromDocument((doc, ctx) => "https://github.com/andymac4182/andymac4182.github.io/edit/develop/input/" + doc.Source.GetRelativeInputPath()))
                .AddSetting(SiteKeys.NoChildPages, Config.FromDocument(doc => doc.Destination.Segments[0].SequenceEqual("blog".AsMemory())))
                .RunAsync();
    }
    public static class SectionNames
    {
        public const string Head = nameof(Head);
        public const string Scripts = nameof(Scripts);
        public const string Splash = nameof(Splash);
        public const string Sidebar = nameof(Sidebar);
        public const string Subtitle = nameof(Subtitle);
    }
    public static class SiteKeys
    {
        public const string NoContainer = nameof(NoContainer);
        public const string NoSidebar = nameof(NoSidebar);
        public const string Topic = nameof(Topic);
        public const string NoChildPages = nameof(NoChildPages);
    }
}