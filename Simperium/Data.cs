using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.Generic;

namespace Simperium
{

    /** 
     * <summary>Superclass for Metadata and Bucket classes.</summary>
     */
    public class Data
    {
        Settings settings;
        string access_token;

        /**
         * <summary>Constructor requires a currently valid token and a Settings object.</summary>
         * <param name="access_token">currently valid token</param>
         */
        public Data(string access_token, Settings settings)
        {
            // TODO check for missing settings
            this.settings = settings;
            this.access_token = access_token;
        }

        public string AccessToken
        {
            get
            {
                return access_token;
            }
            set
            {
                if (value != access_token)
                    access_token = value;
            }
        }

        /// <summary>
        /// Helper Functions
        /// </summary>
        /// 
        protected WebClient getWebClient()
        {
            WebClient client = new WebClient();
            client.Headers["User-Agent"] = settings.USER_AGENT;
            client.Headers["X-Simperium-Token"] = access_token;
            return client;
        }


        protected string getBaseUriString()
        {
            return "https://api.simperium.com/" + settings.API_VERSION + "/" + settings.APP_ID;
        }



        protected int extractVersionFromHeaders(WebClient client)
        {
            string s = client.ResponseHeaders["X-Simperium-Version"];
            int num = 0;
            bool ok = Int32.TryParse(s, out num);
            if (ok)
                return num;
            return -1;
        }

        protected string appendQueryParameters(Dictionary<string, string> parameters)
        {
            if (parameters.Count == 0)
                return null;
            string query = "?";
            foreach (KeyValuePair<string, string> pair in parameters)
                query += pair.Key + "=" + pair.Value + "&";
            query = query.Remove(query.Length - 1);
            return query;
        }
    }

    public class JsonObjectResult
    {
        public string id { get; private set; }
        public int v { get; private set; }
        public string d { get; private set; }
        public JsonObjectResult() { }
    }

    public class DataEventArgs<T> : EventArgs
    {
        public T Result { get; private set; }

        public Exception Error { get; private set; }

        public DataEventArgs(Exception e)
        {
            Error = e;
        }

        public DataEventArgs(T r)
        {
            Result = r;
        }
    }
}
