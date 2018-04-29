using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Web;
using Dapper;
using MyNewsFeedSln.Models;
using Newtonsoft.Json.Linq;

namespace MyNewsFeedSln.App_Data
{
    public class DBManager
    {
        String connectionString = @"Data Source=.\SQLEXPRESS;Initial Catalog=MyNewsFeedDB;Integrated Security=True";
        string newsApiKey = "106a58c18c96495a949f2649e077dd75";
        public List<NewsSource> ListSources()
        {
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                return db.Query<NewsSource>("Select * From NewsSource").AsList();
            }
        }

        public int AddNewSource(NewsSource source)
        {
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                string sqlQuery = "INSERT INTO NewsSource VALUES (@SourceName," +
                                                                "@SourceLink," +
                                                                "@SourceImageLink," +
                                                                "@SourceType," +
                                                                "@SourceLocation," +
                                                                "@SourceFavorite)";
                int rowsAffected = db.Execute(sqlQuery, source);
                return rowsAffected;
            }
        }

        public int UpdateSource(NewsSource source)
        {
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                string sqlQuery = "UPDATE NewsSource SET SourceName = @SourceName," +
                                                     "SourceLink = @SourceLink," +
                                                     "SourceImageLink = @SourceImageLink," +
                                                     "SourceType = @SourceType," +
                                                     "SourceLocation = @SourceLocation," +
                                                     "SourceFavorite = @SourceFavorite " +
                                                     "WHERE SourceId = @SourceId";
                int rowsAffected = db.Execute(sqlQuery, source);
                return rowsAffected;
            }
        }

        public int RemoveSource(string SourceId)
        {
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                string sqlQuery = "Delete From NewsSource " +
                    "WHERE SourceId = @SourceId";
                int rowsAffected = db.Execute(sqlQuery, new { SourceId });
                return rowsAffected;
            }
        }

        public NewsSource SearchSourceByName(string SourceName)
        {
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                return db.Query<NewsSource>("Select * From NewsSource " +
                    "WHERE SourceName = @SourceName", new { SourceName }).SingleOrDefault();
            }
        }

        public NewsSource SearchSourceById(string SourceId)
        {
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                return db.Query<NewsSource>("Select * From NewsSource " +
                    "WHERE SourceId = @SourceId", new { SourceId }).SingleOrDefault();
            }
        }  

        public List<Article> GetAllArticles()
        {
            string currentCountryCode = GetUserCountryLocation();
            string todayDate = GetTodayDateString();

            var url = "https://newsapi.org/v2/top-headlines?" +
            "country=" + currentCountryCode + "&" +
            "from=" + todayDate + "&" +
            "to=" + todayDate + "&" +

            "apiKey="+newsApiKey;

            return RequestArticles(url);
        }

        public List<Article> GetArticlesGivenCategory(string type)
        {
            string currentCountryCode = GetUserCountryLocation();
            string todayDate = GetTodayDateString();

            var url = "https://newsapi.org/v2/top-headlines?" +
            "from=" + todayDate + "&" +
            "to=" + todayDate + "&" +
            "category=" +type+"&"+
            "apiKey=" + newsApiKey;

            return RequestArticles(url);
        }

        public List<Article> GetArticlesGivenCategoryAndLocation(string type, string countryCode)
        {
            string todayDate = GetTodayDateString();

            var url = "https://newsapi.org/v2/top-headlines?" +
            "country=" + countryCode + "&" +
            "from=" + todayDate + "&" +
            "to=" + todayDate + "&" +
            "category=" + type + "&" +
            "apiKey=" + newsApiKey; 

            return RequestArticles(url);
        }

        private List<Article> RequestArticles(string url)
        {
            List<Article> articles = new List<Article>();

            WebClient myClient = new WebClient();
            var json = myClient.DownloadString(url);

            var jsonData = myClient.DownloadData(url);
            JObject jsonObject = JObject.Parse(json);

            foreach (var article in jsonObject["articles"])
            {
                Article aArticle = new Article
                {
                    id = (string)article["source"]["id"],
                    name = (string)article["source"]["name"],
                    author = (string)article["author"],
                    title = (string)article["title"],
                    description = (string)article["description"],
                    url = (string)article["url"],
                    urlImage = (string)article["urlToImage"],
                    publishedAt = (string)article["publishedAt"]
                };

                articles.Add(aArticle);
            }
            return articles;
        }

        public string GetUserCountryLocation()
        {
            string strHostName = "";
            strHostName = System.Net.Dns.GetHostName();

            IPHostEntry ipEntry = System.Net.Dns.GetHostEntry(strHostName);

            IPAddress[] addr = ipEntry.AddressList;

            string userIPAddress = addr[addr.Length - 1].ToString();

            string url = "http://freegeoip.net/json/";
            WebClient client = new WebClient();
            string jsonstring = client.DownloadString(url);
            JObject locationInfo = JObject.Parse(jsonstring);

            return ((string)locationInfo["country_code"]);
        }

        public string GetTodayDateString()
        {
            return DateTime.Today.ToString("yyyy-MM-dd");
        }

        public NewsSource GetNewsSourceGivenArticle(Article article)
        {
            NewsSource source = null;
            foreach (var item in this.ListSources())
            {
                if (article.id.ToLower().Equals(item.SourceName.ToLower()) ||
                    article.name.ToLower().Equals(item.SourceName.ToLower()))
                {
                    source = item;
                    break;
                }
            }
            return source;
        }
    }
}