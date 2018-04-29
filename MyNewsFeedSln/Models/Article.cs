using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyNewsFeedSln.Models
{
    public class Article
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Author { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Url { get; set; }
        public string UrlImage { get; set; }
        public string PublishedAt { get; set; }
    }
}