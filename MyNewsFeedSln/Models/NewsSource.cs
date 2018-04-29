using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyNewsFeedSln.Models
{
    public class NewsSource
    {
        public string SourceId { get; set; }
        public string SourceName { get; set; }
        public string SourceLink { get; set; }
        public string SourceImageLink { get; set; }
        public string SourceType { get; set; }
        public string SourceLocation { get; set; }
        public bool SourceFavorite { get; set; }
    }
}