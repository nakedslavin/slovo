using System;
using System.Linq;
using System.Net;
using System.Xml.Linq;
using voiceapi.Models;

namespace voiceapi.Services {
    public class XmlParser {
        public static string Parse(Podcast podcast) {
            var xText = RecipiesService.GetTemplate();
            xText = xText.Replace("{{title}}", podcast.Title)
                .Replace("{{description}}", podcast.Description)
                .Replace("{{copyright}}", podcast.Copyright)
                .Replace("{{language}}", podcast.Language)
                .Replace("{{pubDate}}", podcast.PubDate)
                .Replace("{{link}}", podcast.Link)
                .Replace("{{imageUrl}}", podcast.Image)
                .Replace("{{imageTitle}}", podcast.Title)
                .Replace("{{imageLink}}", podcast.Link)
                .Replace("{{author}}", podcast.Author)
                .Replace("{{explicit}}", "no")
                .Replace("{{keywords}}", string.Join(",", podcast.Keywords))
                .Replace("{{authorEmail}}", podcast.Author)
                .Replace("{{category}}", podcast.Category.Replace("&","&amp;"));
            var xDocument = XDocument.Parse(xText);
            var itemElement = xDocument.Descendants("item").Single();
            var itemTemplate = itemElement.ToString();  
            itemElement.Remove();
            foreach(var item in podcast.Items) {
                string template = itemTemplate.Clone().ToString();
                template = template.Replace("{{itemTitle}}", $"{item.GuestName}, {item.GuestDescription}, {item.Title}")
                    .Replace("{{itemDescription}}", item.Description)
                    .Replace("{{itemGuid}}", item.Guid)
                    .Replace("{{language}}", podcast.Language)
                    .Replace("{{itemPubDate}}", item.PubDate)
                    .Replace("{{itemLink}}", item.Link)
                    .Replace("{{imageUrl}}", item.Image)
                    .Replace("{{imageTitle}}", item.Title)
                    .Replace("{{imageLink}}", item.Link)
                    .Replace("{{itemAuthor}}", podcast.Author)
                    .Replace("{{explicit}}", item.Explicit)
                    .Replace("{{keywords}}", string.Join(",", item.Keywords))
                    .Replace("{{authorEmail}}", podcast.Author)
                    .Replace("{{itemDuration}}", item.Duration)
                    .Replace("{{itemLength}}", item.Length.ToString())
                    .Replace("{{itemEncloseUrl}}",  Utils.MediaUrlEncode(item.EncloseUrl, item.EncloseType));

                if (item.EncloseType != null)
                    template = template.Replace("audio/mpeg", item.EncloseType);

                var xElementItem = XElement.Parse(template);
                xDocument.Root.Element("channel").Add(xElementItem);
            }
            string xDocumentText = xDocument.ToString();
            return xDocumentText;
        }
    }
}