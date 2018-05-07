using System;
using System.Linq;
using voiceapi.Models;
using System.Net;
using CsQuery;
using System.Collections.Generic;
using System.Xml.Linq;
using Newtonsoft.Json.Linq;

namespace voiceapi.Services {
    public class RecipiesService {
        public static JObject GetRecipe(string host) {
            string githubRepo = $"https://raw.githubusercontent.com/nakedslavin/osp/master/{host}.json";
            string recipe = new WebClient().DownloadString(githubRepo);
            if (recipe == null)
            return null;
            var jobject = JObject.Parse(recipe);
            return jobject;
        }
        public static string GetTemplate() {
            string githubRepo = $"https://raw.githubusercontent.com/nakedslavin/osp/master/rss.template.xml";
            string template = new WebClient().DownloadString(githubRepo);
            //var xobject = XDocument.Parse(template);
            return template;
        }
    }
}