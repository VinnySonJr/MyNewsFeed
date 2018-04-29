using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyNewsFeedSln.Models
{
    public class Region
    {
        public string name { get; set; }
        public NewsSource[] newsSources { get; set; }
        public bool isFavorite { get; set; }
    }
}