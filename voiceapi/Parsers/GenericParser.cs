using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using CsQuery;
using voiceapi.Models;

namespace voiceapi.Services {
    public class GenericParser : IParser
    {
        public virtual string Host { get; set; } = "generic";
        public virtual Podcast Parse(string url)
        {
            try {
                var uri = new Uri(url);
                var host = uri.Host;
                var recipe = RecipiesService.GetRecipe(host);
                var podcast = new Podcast();
                var cq = CQ.CreateFromUrl(url);
                podcast.Link = url;
                podcast.Host = host;
                podcast.Title = (string)recipe["title"];
                if (podcast.Title != null && podcast.Title.StartsWith("$")) {
                    podcast.Title = podcast.Title.Substring(1);
                    string cmd = podcast.Title.Split('|').ElementAtOrDefault(0);
                    string attr = podcast.Title.Split('|').ElementAtOrDefault(1);
                    podcast.Title = attr == null ? cq[cmd].Text() : cq[cmd].Attr(attr);
                }
                podcast.Description = (string)recipe["description"];
                if (podcast.Description != null && podcast.Description.StartsWith("$")) {
                    podcast.Description = podcast.Description.Substring(1);
                    string cmd = podcast.Description.Split('|').ElementAtOrDefault(0);
                    string attr = podcast.Description.Split('|').ElementAtOrDefault(1);
                    podcast.Description = attr == null ? cq[cmd].Text() : cq[cmd].Attr(attr);
                    podcast.Description = podcast.Description.Replace("\n","").Trim();
                }
                podcast.Copyright = (string)recipe["copyright"];
                if (podcast.Copyright != null && podcast.Copyright.StartsWith("$")) {
                    podcast.Copyright = podcast.Copyright.Substring(1);
                    string cmd = podcast.Copyright.Split('|').ElementAtOrDefault(0);
                    string attr = podcast.Copyright.Split('|').ElementAtOrDefault(1);
                    podcast.Copyright = attr == null ? cq[cmd].Text() : cq[cmd].Attr(attr);
                }
                podcast.Author = (string)recipe["author"];
                if (podcast.Author != null && podcast.Author.StartsWith("$")) {
                    podcast.Author = podcast.Author.Substring(1);
                    string cmd = podcast.Author.Split('|').ElementAtOrDefault(0);
                    string attr = podcast.Author.Split('|').ElementAtOrDefault(1);
                    podcast.Author = attr == null ? cq[cmd].Text() : cq[cmd].Attr(attr);
                }
                podcast.Category = (string)recipe["category"];
                if (podcast.Category != null && podcast.Category.StartsWith("$")) {
                    podcast.Category = podcast.Category.Substring(1);
                    string cmd = podcast.Category.Split('|').ElementAtOrDefault(0);
                    string attr = podcast.Category.Split('|').ElementAtOrDefault(1);
                    podcast.Category = attr == null ? cq[cmd].Text() : cq[cmd].Attr(attr);
                }
                string keywords = (string)recipe["keywords"];
                if (keywords != null && keywords.StartsWith("$")) {
                    keywords = keywords.Substring(1);
                    string cmd =keywords.Split('|').ElementAtOrDefault(0);
                    string attr = keywords.Split('|').ElementAtOrDefault(1);
                    keywords = attr == null ? cq[cmd].Text() : cq[cmd].Attr(attr);
                }
                if (keywords == null)
                    podcast.Keywords = new List<string>();
                else
                    podcast.Keywords = keywords.Split(',').Select(k => k.ToLower().Trim()).ToList();

                podcast.Image = (string)recipe["image"];
                if (podcast.Image != null && podcast.Image.StartsWith("$")) {
                    podcast.Image = podcast.Image.Substring(1);
                    string cmd = podcast.Image.Split('|').ElementAtOrDefault(0);
                    string attr = podcast.Image.Split('|').ElementAtOrDefault(1);
                    podcast.Image = attr == null ? cq[cmd].Text() : cq[cmd].Attr(attr);
                    if (podcast.Image != null && !podcast.Image.Contains(host))
                        podcast.Image = $"{uri.Scheme}://{host}{podcast.Image}";
                }
                podcast.PubDate = (string)recipe["pubDate"];
                if (podcast.PubDate != null && podcast.PubDate.StartsWith("$")) {
                    podcast.PubDate = podcast.PubDate.Substring(1);
                    string cmd = podcast.PubDate.Split('|').ElementAtOrDefault(0);
                    string attr = podcast.PubDate.Split('|').ElementAtOrDefault(1);
                    podcast.PubDate = attr == null ? cq[cmd].Text() : cq[cmd].Attr(attr);
                }
                else {
                    podcast.PubDate = Utils.ConvertToPubDate(DateTime.Now);
                }
                podcast.Language = (string)recipe["language"];
                if (podcast.Language != null && podcast.Language.StartsWith("$")) {
                    podcast.Language = podcast.Language.Substring(1);
                    string cmd = podcast.Language.Split('|').ElementAtOrDefault(0);
                    string attr = podcast.Language.Split('|').ElementAtOrDefault(1);
                    podcast.Language = attr == null ? cq[cmd].Text() : cq[cmd].Attr(attr);
                }

                var items = new List<Item>();
                var emit  = cq[((string)recipe["emit"]).Substring(1)];
                emit.Each(el => {
                    var item = new Item();
                    
                    item.Title = (string)recipe["map"]["title"];
                    if (item.Title != null && item.Title.StartsWith("$")) {
                        item.Title = item.Title.Substring(1);
                        string cmd = item.Title.Split('|').ElementAtOrDefault(0);
                        string attr = item.Title.Split('|').ElementAtOrDefault(1);
                        item.Title = attr == null ? el.Cq().Find(cmd).Text() : el.Cq().Find(cmd).Attr(attr);
                    }
                    item.Description = (string)recipe["map"]["description"];
                    if (item.Description != null && item.Description.StartsWith("$")) {
                        item.Description = item.Description.Substring(1);
                        string cmd = item.Description.Split('|').ElementAtOrDefault(0);
                        string attr = item.Description.Split('|').ElementAtOrDefault(1);
                        item.Description = attr == null ? el.Cq().Find(cmd).Text() : el.Cq().Find(cmd).Attr(attr);
                        item.Description = item.Description.Trim();
                    }
                    item.Link = (string)recipe["map"]["link"];
                    if (item.Link != null && item.Link.StartsWith("$")) {
                        item.Link = item.Link.Substring(1);
                        string cmd = item.Link.Split('|').ElementAtOrDefault(0);
                        string attr = item.Link.Split('|').ElementAtOrDefault(1);
                        item.Link = attr == null ? el.Cq().Find(cmd).Text() : el.Cq().Find(cmd).Attr(attr);
                        if (item.Link != null && !item.Link.Contains(host))
                            item.Link = $"{uri.Scheme}://{host}{item.Link}";
                    }
                    item.Guid = (string)recipe["map"]["guid"];
                    if (item.Guid != null && item.Guid.StartsWith("$")) {
                        item.Guid = item.Guid.Substring(1);
                        string cmd = item.Guid.Split('|').ElementAtOrDefault(0);
                        string attr = item.Guid.Split('|').ElementAtOrDefault(1);
                        item.Guid = attr == null ? el.Cq().Find(cmd).Text() : el.Cq().Find(cmd).Attr(attr);
                    }
                    item.Duration = (string)recipe["map"]["duration"];
                    if (item.Duration != null && item.Duration.StartsWith("$")) {
                        item.Duration = item.Duration.Substring(1);
                        string cmd = item.Duration.Split('|').ElementAtOrDefault(0);
                        string attr = item.Duration.Split('|').ElementAtOrDefault(1);
                        item.Duration = attr == null ? el.Cq().Find(cmd).Text() : el.Cq().Find(cmd).Attr(attr);
                    }
                    string ikeywords = (string)recipe["map"]["keywords"];
                    if (ikeywords != null && ikeywords.StartsWith("$")) {
                        ikeywords = ikeywords.Substring(1);
                        string cmd =ikeywords.Split('|').ElementAtOrDefault(0);
                        string attr = ikeywords.Split('|').ElementAtOrDefault(1);
                        ikeywords = attr == null ? el.Cq().Find(cmd).Text() : el.Cq().Find(cmd).Attr(attr);
                    }
                    if (ikeywords == null)
                        item.Keywords = new List<string>();
                    else
                        item.Keywords = ikeywords.Split(',').Select(k => k.ToLower().Trim()).ToList();

                    item.Image = (string)recipe["map"]["image"];
                    if (item.Image != null && item.Image.StartsWith("$")) {
                        item.Image = item.Image.Substring(1);
                        string cmd = item.Image.Split('|').ElementAtOrDefault(0);
                        string attr = item.Image.Split('|').ElementAtOrDefault(1);
                        item.Image = attr == null ? el.Cq().Find(cmd).Text() : el.Cq().Find(cmd).Attr(attr);
                        if (item.Image != null && !item.Image.Contains(host))
                            item.Image = $"{uri.Scheme}://{host}{item.Image}";
                    }
                    item.PubDate = (string)recipe["map"]["pubDate"];
                    if (item.PubDate != null && item.PubDate.StartsWith("$")) {
                        item.PubDate = item.PubDate.Substring(1);
                        string cmd = item.PubDate.Split('|').ElementAtOrDefault(0);
                        string attr = item.PubDate.Split('|').ElementAtOrDefault(1);
                        item.PubDate = attr == null ? el.Cq().Find(cmd).Text() : el.Cq().Find(cmd).Attr(attr);
                    }
                    item.Explicit = (string)recipe["map"]["explicit"];
                    if (item.Explicit != null && item.Explicit.StartsWith("$")) {
                        item.Explicit = item.Explicit.Substring(1);
                        string cmd = item.Explicit.Split('|').ElementAtOrDefault(0);
                        string attr = item.Explicit.Split('|').ElementAtOrDefault(1);
                        item.Explicit = attr == null ? el.Cq().Find(cmd).Text() : el.Cq().Find(cmd).Attr(attr);
                    }
                    item.EncloseUrl = (string)recipe["map"]["encloseUrl"];
                    if (item.EncloseUrl != null && item.EncloseUrl.StartsWith("$")) {
                        item.EncloseUrl = item.EncloseUrl.Substring(1);
                        string cmd = item.EncloseUrl.Split('|').ElementAtOrDefault(0);
                        string attr = item.EncloseUrl.Split('|').ElementAtOrDefault(1);
                        item.EncloseUrl = attr == null ? el.Cq().Find(cmd).Text() : el.Cq().Find(cmd).Attr(attr);
                        if (item.EncloseUrl != null && !item.EncloseUrl.Contains(host))
                            item.EncloseUrl = $"{uri.Scheme}://{host}{item.EncloseUrl}";
                    }

                    item.EncloseType = (string)recipe["map"]["encloseType"];
                    if (item.EncloseType != null && item.EncloseType.StartsWith("$")) {
                        item.EncloseType = item.EncloseType.Substring(1);
                        string cmd = item.EncloseType.Split('|').ElementAtOrDefault(0);
                        string attr = item.EncloseType.Split('|').ElementAtOrDefault(1);
                        item.EncloseType = attr == null ? el.Cq().Find(cmd).Text() : el.Cq().Find(cmd).Attr(attr);
                    }
                    item.GuestName = (string)recipe["map"]["guestName"];
                    if (item.GuestName != null && item.GuestName.StartsWith("$")) {
                        item.GuestName = item.GuestName.Substring(1);
                        string cmd = item.GuestName.Split('|').ElementAtOrDefault(0);
                        string attr = item.GuestName.Split('|').ElementAtOrDefault(1);
                        item.GuestName = attr == null ? el.Cq().Find(cmd).Text() : el.Cq().Find(cmd).Attr(attr);
                    }
                    item.GuestDescription = (string)recipe["map"]["guestDescription"];
                    if (item.GuestDescription != null && item.GuestDescription.StartsWith("$")) {
                        item.GuestDescription = item.GuestDescription.Substring(1);
                        string cmd = item.GuestDescription.Split('|').ElementAtOrDefault(0);
                        string attr = item.GuestDescription.Split('|').ElementAtOrDefault(1);
                        item.GuestDescription = attr == null ? el.Cq().Find(cmd).Text() : el.Cq().Find(cmd).Attr(attr);
                    }

                    items.Add(item);
                });
                podcast.Items = items.Where(a => a.EncloseUrl != null).ToList();
                podcast.CreationTimestamp = DateTime.UtcNow;
                podcast.BuildTimestamp = podcast.CreationTimestamp;
                
                return podcast;
            }
            catch (Exception exc)
            {
                Console.Write(exc.Message);
                return null;
            }
        }
    }
}