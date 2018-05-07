using System;
using CsQuery;
using voiceapi.Models;
using YoutubeExtractor;
using YoutubeExplode;
using System.Collections.Generic;
using System.Linq;

namespace voiceapi.Services {

    public class YoutubeParser : GenericParser
    {
        public override string Host { get; set; } = "youtube.com";

        public override Podcast Parse(string url)
        {
            var channelId = Utils.YTUrlParser(url);
            var podcast = new Podcast();
            var yc = new YoutubeClient();
            var channel = yc.GetChannelAsync(channelId).Result;
            podcast.Title = channel.Title;
            podcast.Description = channel.Title;
            podcast.Image = channel.LogoUrl;
            podcast.Link = "https://www.youtube.com/channel/" + channelId;
            podcast.Host = "www.youtube.com";
            podcast.Copyright = podcast.Title;
            podcast.Items = new List<Item>();
            foreach(var upload in yc.GetChannelUploadsAsync(channelId, 1).Result.Take(10).ToList()) {
                var item = new Item();
                item.Duration = upload.Duration.ToString(@"hh\:mm\:ss");
                item.Title = upload.Title;
                item.Keywords = upload.Keywords.ToList();
                item.Description = upload.Description;
                item.PubDate = Utils.ConvertToPubDate(upload.UploadDate.DateTime);
                item.Image = upload.Thumbnails.LowResUrl;
                item.Guid = upload.Id;
                item.Link = "https://www.youtube.com/watch?v=" + item.Guid;
                var (flink,fmime) = Utils.YTGetAudioLinks(upload.Id);
                item.EncloseUrl = flink;
                item.EncloseType = fmime;
                item.Length = (long)upload.Duration.TotalMilliseconds;
                if (string.IsNullOrWhiteSpace(item.EncloseUrl))
                    continue;
                podcast.Items.Add(item);
                if (podcast.Keywords == null || podcast.Keywords.Count == 0)
                    podcast.Keywords = item.Keywords;
                if (podcast.Author == null)
                    podcast.Author = upload.Author;
            };

            podcast.CreationTimestamp = DateTime.UtcNow;
            podcast.BuildTimestamp = podcast.CreationTimestamp;

            return podcast;
        }
    }
}