using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyNewsFeedSln.Models
{
    public class Region
    {
        public string Name { get; set; }
        public NewsSource[] NewsSources { get; set; }
        public bool IsFavorite { get; set; }
    }
}