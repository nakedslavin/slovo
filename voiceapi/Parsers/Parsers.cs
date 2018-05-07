using System;
using System.Linq;
using voiceapi.Models;
using CsQuery;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Globalization;

namespace voiceapi.Services {
    public class Parsers {
        private static IParser[] parsers = new IParser[] { 
                new EchoMskParser(), 
                new YoutubeParser(), 
                new GenericParser() 
        };
        public static IParser GetParser(string host) {
            return parsers.SingleOrDefault(p => p.Host == host || p.Host == host.Replace("www.",""));
        }

    }
    
    public interface IParser
    {
        string Host { get; set; }
        Podcast Parse(string url);
    }
}