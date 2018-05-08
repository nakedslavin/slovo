using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ITB.Business.Repos;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using voiceapi.Models;
using voiceapi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Hosting;

namespace voiceapi.Controllers
{
    //[EnableCors("AllowAll")]
    public class HomeController : Controller
    {
        public IHostingEnvironment HostingEnv { get; }
        public HomeController(IHostingEnvironment env)
        {
            HostingEnv = env;
        }
        [HttpGet]
        public IActionResult All() {
            return new PhysicalFileResult(Path.Combine(HostingEnv.WebRootPath,"index.html"), "text/html");
        }
        public IActionResult Index()
        {
            return Json(new string[] {"GET"});
        }

        [HttpPost]
        public IActionResult Index([FromBody] JObject obj) {
            var session = new MongoSession<Podcast>();

            var address = obj["address"].ToString().Trim().TrimEnd('/');
            var name = obj["name"].ToString().ToLower().Trim();
            var category = obj["category"].ToString();

            if (Uri.TryCreate(address, UriKind.Absolute, out Uri uri)) {
                
                var storedCasts = session.Get(p => p.Link == address || p.Name == name);
                Podcast podcast;
                if (storedCasts.Count != 0)
                    podcast = storedCasts.First();
                else {
                    string host = uri.Host;
                    if (uri.Host == "youtu.be" || uri.Host == "youtube.com")
                        host = "www.youtube.com";

                    podcast = Parsers.GetParser(host).Parse(address);
                    podcast.Category = category;
                    podcast.Name = CleanName(name);
                    
                    session.Save(podcast);
                }

                //string xmlPodcast = XmlParser.Parse(podcast);
                return Json(podcast);
            }
            else {
                return Json(null);
            }
        }
        [HttpPost]
        public IActionResult Radio([FromBody] Podcast podcast)
        {
            var session = new MongoSession<Podcast>();
            var name = podcast.Name.ToLower().Trim();

            var pod = new Podcast();
            pod.Host = "slovo.io";
            pod.Title = podcast.Title;
            pod.Name = CleanName(name);
            pod.Link = "http://" + pod.Host + "/l/" + pod.Name;
            pod.Category = "Radio (NEW)";
            pod.Keywords = new List<string> { "radio" };
            pod.Podcasts = podcast.Podcasts;
            pod.Image = podcast.Image;
            pod.PubDate = Utils.ConvertToPubDate(DateTime.UtcNow);
            //pod.HumanId = podcast.HumanId;
            pod.HumanFullName = podcast.HumanFullName;
            pod.HumanName = podcast.HumanName;
            pod.HumanId = podcast.HumanId;
            pod.CreationTimestamp = DateTime.UtcNow;
            //pod.BuildTimestamp = DateTime.UtcNow;
            pod.Author = podcast.HumanName;
            pod.Copyright = podcast.HumanName;
            pod.Items = new List<Item>();

            session.Save(pod);

            return Json(pod);
        }
        [HttpPost]
        public IActionResult Update([FromBody] Podcast podcast) {
            if (!string.IsNullOrWhiteSpace(podcast.Id)) {
                var session = new MongoSession<Podcast>();
                var stored = session.Get(_ => _.Id == podcast.Id).FirstOrDefault();
                if (stored != null) {
                    stored.Name = CleanName(podcast.Name);
                    stored.Title = podcast.Title;
                    stored.Description = podcast.Description;
                    stored.Copyright = podcast.Copyright;
                    stored.Author = podcast.Author;
                    stored.Link = podcast.Link;
                    stored.Image = podcast.Image;
                    stored.Language = podcast.Language;
                    stored.Category = podcast.Category;
                    session.Save(stored);
                    return new StatusCodeResult(200);
                }
            }
            return new StatusCodeResult(400);
        }
        [HttpPost]
        [Authorize]
        public IActionResult Delete([FromBody] Podcast podcast) {
            if (!string.IsNullOrWhiteSpace(podcast.Id)) {
                var session = new MongoSession<Podcast>();
                var stored = session.Get(_ => _.Id == podcast.Id).FirstOrDefault();
                if (stored != null) {
                    session.Delete(stored);
                    return new StatusCodeResult(200);
                }
            }
            return new StatusCodeResult(400);
        }
        public IActionResult Audio(string fileName, string fileExtensionWithoutDot) {

            var client = new MongoSession<EnclosureLink>();
            var url = client.Get(_ => _.ShortUrl == fileName).FirstOrDefault();
            if (url == null)
                return new StatusCodeResult(404);
            else  {
                var realUrl = url.RealUrl;
                return Redirect(url.RealUrl);
            }
        }
        //l
        public IActionResult Link(string id) {
            if (!string.IsNullOrWhiteSpace(id)) {
                var session = new MongoSession<Podcast>();
                var podcast = session.Get(p => p.Name == id.ToLower().Trim()).FirstOrDefault();
                if (podcast == null) return new StatusCodeResult(404);
                podcast = UpdatePodcastItems(podcast,session);
                string xmlPodcast = XmlParser.Parse(podcast);
                return Content(xmlPodcast);
            }
            return new StatusCodeResult(200);
        }
        // s
        public IActionResult Show(string id) {
            if (!string.IsNullOrWhiteSpace(id)) {
                var session = new MongoSession<Podcast>();
                var podcast = session.Get(p => p.Name == id.ToLower().Trim()).FirstOrDefault();
                if (podcast == null) return new StatusCodeResult(404);
                podcast = UpdatePodcastItems(podcast,session);
                return Json(podcast);
            }
            return new StatusCodeResult(200);
        }
        
        private Podcast UpdatePodcastItems(Podcast podcast, MongoSession<Podcast> session) {
            
            if (DateTime.UtcNow.Subtract(podcast.BuildTimestamp).TotalMinutes > 30) {
                var items = new List<Item>();
                if (podcast.Podcasts != null && podcast.Podcasts.Count > 0) {
                    var podcasts = new List<Podcast>();
                    var pods = podcast.Podcasts.Select(pod => session.Get(_ => _.Id == pod).FirstOrDefault());
                    foreach(var pod in pods) {
                        if (pod == null) continue;
                        var p = UpdatePodcastItems(pod, session);
                        podcasts.Add(p);
                    }
                    // HACK
                    items = podcasts.SelectMany(p => p.Items).OrderByDescending(p => Utils.ConvertFromPubDate(p.PubDate)).ToList();     
                }
                else
                    items = Parsers.GetParser(podcast.Host).Parse(podcast.Link).Items;
                
                podcast.Items = items;
                podcast.BuildTimestamp = DateTime.UtcNow;
                session.Save(podcast);
            }

            return podcast;
        }
        private string CleanName(string name) {
            var rgx = new Regex("[^a-z0-9-]");
            name = rgx.Replace(name, "");
            name = string.Concat(name.Take(10));
            return name;
        }
        public IActionResult List(string filter, string sort = "{_id : 1 }") {
            var podcasts = new List<Podcast>();
            var session = new MongoSession<Podcast>();
            if (!string.IsNullOrWhiteSpace(filter))
            {
                podcasts = session.Get(filter, sort);
            }
            else
            {
                podcasts = session.Get(_ => true)
                    .OrderByDescending(_ => _.BuildTimestamp)
                    .ToList();
            }
            return Json(podcasts);
        }
        public IActionResult Send() {
            string note = null;
            using (var sr = new StreamReader(Request.Body))
                note = sr.ReadToEnd();
            if (!string.IsNullOrWhiteSpace(note) && note.Length > 5) {
                var msg = new Message();
                msg.Text = note;
                var session = new MongoSession<Message>();
                session.Save(msg);
            }
            return new StatusCodeResult(200);
        }
        public IActionResult Names() {
            var session = new MongoSession<Podcast>();
            return Json(session.Get(_ => true).Select(_ => _.Name));
        }
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
