using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Specialized;
using System.Web;

namespace URL_Splitter.DAL
{
    public static class DBAccess
    {
        private static IMongoClient _client;
        private static IMongoDatabase _database;
        const string connectionString = "mongodb://localhost:27017";
        private static object _object=new object();

        public static string AddDocument(string url, string tagName)
        {
            try {
                InitDB();
                _database = _client.GetDatabase("urldb");
                var collection = _database.GetCollection<BsonDocument>("urlqs");
                FilterDefinition<BsonDocument> filter = new BsonDocument(new BsonElement("url", url.ToLower()));

                var countTask = collection.CountAsync(filter);

                countTask.Wait();             
                if(countTask.Result>0)
                {
                    return "URL Rejected: Duplicate";
                }
                var document = new BsonDocument {
                                                    { "url",url.ToLower()}, {"tagname",tagName.ToLower()}, { "timestamp",DateTime.Now}
                                                };
                
                var qsVariables = ParseQueryString(url);
                foreach (string key in qsVariables)
                {
                    var value = qsVariables[key];
                    document.Add(new BsonElement(key.ToLower(), value.ToLower()));
                }

                var InsertTask =  collection.InsertOneAsync(document);
                InsertTask.Wait();
                return "URL Successfully Recorded";
            }
            catch(Exception ex)
            {
                return "Exception Occured";
            }
        }
        private static NameValueCollection ParseQueryString(string url)
        {
            String querystring;
            int iqs = url.IndexOf('?');
            if (iqs == -1)
            {
                return null;
            }
            else
            {
                querystring = (iqs < url.Length - 1) ? url.Substring(iqs + 1) : String.Empty;
                return HttpUtility.ParseQueryString(querystring);
            }
        }
        
        private static void InitDB()
        {
            if (_client == null)
            {
                lock(_object)
                {
                    if(_client==null)
                    {
                        _client = new MongoClient(connectionString);
                    }
                }
            }
        }
    } 
}