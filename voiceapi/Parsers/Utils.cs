using System;
using System.Globalization;
using System.Linq;
using YoutubeExtractor;
using YoutubeExplode;
using System.Net;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using System.Text;
using ITB.Business.Repos;
using voiceapi.Models;

namespace voiceapi.Services {
    public static class Utils {
        public static string MediaUrlEncode(string url, string mime) {
            if (!url.Contains("&"))
                return url;
            var hash = MD5Hash(url);
            var client = new MongoSession<EnclosureLink>();
            var stored = client.Get(_ => _.ShortUrl == hash).FirstOrDefault();
            if (stored == null) {
                var linkObject = new EnclosureLink { RealUrl = url, ShortUrl = hash };
                client.Save(linkObject);
                stored = linkObject;
            }
            string ext = ".mp3";
            if (mime == "audio/aac")
                ext = ".aac";
            if (mime == "audio/ogg")
                ext = ".ogg";

            var link = "http://slovo.io/a/" + stored.ShortUrl + ext;
            return link;
        }

        public static string MD5Hash(string str)
        {
            using (var provider = System.Security.Cryptography.MD5.Create())
            {
                StringBuilder builder = new StringBuilder();                           

                foreach (byte b in provider.ComputeHash(Encoding.UTF8.GetBytes(str)))
                    builder.Append(b.ToString("x2").ToLower());

                return builder.ToString();
            }
        }
        public static (string,string) YTGetAudioLinks(string yt) {
            try {
                var videoInfos = DownloadUrlResolver.GetDownloadUrls("https://www.youtube.com/watch?v=" + yt);
                
                var audios = videoInfos
                    .Where(info => info.AdaptiveType == AdaptiveType.Audio && info.Resolution == 0)
                    .Where(info => info.AudioType == AudioType.Aac || info.AudioType == AudioType.Mp3 || info.AudioType == AudioType.Vorbis).ToList();
                if (audios.Count == 0)
                    return (null,null);
                var aac = audios.FirstOrDefault(info => info.AudioType == AudioType.Aac);
                if (aac != null)
                    return (aac.DownloadUrl, "audio/aac");
                var mp3 = audios.FirstOrDefault(info => info.AudioType == AudioType.Mp3);
                if (mp3 != null)
                    return (mp3.DownloadUrl, "audio/mp3");
                var ogg = audios.FirstOrDefault(info => info.AudioType == AudioType.Vorbis);
                if (ogg != null)
                    return (ogg.DownloadUrl,"audio/ogg");
            }
            catch (Exception) {}
            return (null,null);
        }
        public static string YTUrlParser(string ytLink) {
            // https://youtu.be/Gi-H7C2oHdA
            if (yt.Contains("youtu.be")) {
                var id = ytLink.Split('/').Last().Trim();
                ytLink = "https://www.youtube.com/watch?v=" + id;
            }
            
            //https://www.youtube.com/channel/UCgxTPTFbIbCWfTR9I2-5SeQ
            //https://www.youtube.com/user/NavalnyRu
            //https://www.youtube.com/watch?v=cHqtLrpRQ3Y&
            var yc = new YoutubeClient();
            string link = ytLink.Trim().Split("&")[0].TrimEnd('/');
            string cid = null;
            if (link.Contains("watch?v")) {
                cid = yc.GetVideoAuthorChannelAsync(link.Split('=')[1]).Result.Id;
            }
            if (link.Contains(".com/user")) {
                var text = new WebClient().DownloadString(link);
                var match = new Regex("(?:data-channel-external-id=\")(.*?)(?:\")").Match(text);
                cid = match.Groups.Last().Value;
            }
            if (link.Contains(".com/channel")) {
                link = link.Replace("/videos","");
                cid = link.Split("/").Last();
            }
            return cid;
        }
        public static string YTConvertFromYoutubeDate(string ytDate) {
            // 2 days ago
            // 1 week ago
            // 2 hours ago
            // 3 weeks ago
            // 20 minutes ago
            // 1 minute ago
            if (!string.IsNullOrWhiteSpace(ytDate))
            {
                string cleanYtDate = ytDate.ToLower().Trim();
                var parts = cleanYtDate.Split(' ');
                if (parts.Count() == 3 && int.TryParse(parts[0], out int lapseCount))
                {
                    var timeLapse = parts[1];
                    var currentTime = DateTime.Now;
                    TimeSpan? timeSpan = null;

                    if (timeLapse.StartsWith("hour"))
                        timeSpan = TimeSpan.FromHours(lapseCount);
                    if (timeLapse.StartsWith("minut"))
                        timeSpan = TimeSpan.FromMinutes(lapseCount);
                    if (timeLapse.StartsWith("week"))
                        timeSpan = TimeSpan.FromDays(lapseCount * 7);
                    if (timeLapse.StartsWith("day"))
                        timeSpan = TimeSpan.FromDays(lapseCount);
                    if (timeLapse.StartsWith("year"))
                        timeSpan = TimeSpan.FromDays(lapseCount * 365);
                    if (timeLapse.StartsWith("second"))
                        timeSpan = TimeSpan.FromSeconds(lapseCount);
                    if (timeSpan == null)
                        return null;

                    return ConvertToPubDate(currentTime.Subtract(timeSpan.Value));
                }
            }
            return null;
        }
        public static string EMConvertToPubDate(string ruDate)
        {
            string parseFormat = "dd MMMM yyyy, HH:mm";

            if (DateTime.TryParseExact(ruDate, parseFormat,
                                            CultureInfo.GetCultureInfo("ru"), DateTimeStyles.None, out DateTime date))
                return ConvertToPubDate(date);
            return null;
        }
        public static string ConvertToPubDate(DateTime dt)
        {
            return dt.ToString("ddd',' d MMM yyyy HH':'mm':'ss") + " " + dt.ToString("zzzz").Replace(":", "");
        }

    }
}