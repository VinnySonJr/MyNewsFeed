using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyNewsFeedSln.Models
{
    public class Article
    {
        public string id { get; set; }
        public string name { get; set; }
        public string author { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public string url { get; set; }
        public string urlImage { get; set; }
        public string publishedAt { get; set; }
    }
}