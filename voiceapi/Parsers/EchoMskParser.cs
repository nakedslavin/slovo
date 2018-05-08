using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using CsQuery;
using voiceapi.Models;

namespace voiceapi.Services {
    public class EchoMskParser : GenericParser {
        public override string Host { get; set; } = "echo.msk.ru";
        public override Podcast Parse(string url) {
            var podcast = base.Parse(url);
            foreach(var item in podcast.Items) {
                item.PubDate = Utils.EMConvertToPubDate(item.PubDate);
                if (string.IsNullOrWhiteSpace(item.Title)) {
                    if (!string.IsNullOrWhiteSpace(item.GuestName))
                        item.Title = item.GuestName;
                    else if (!string.IsNullOrWhiteSpace(podcast.Title))
                        item.Title = podcast.Title;
                }
            }
            return podcast;
            
        }
    }
}