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
    public class Bucket
    {
        private string bucket_name;
        private Settings settings;
        private string access_token;
        
        public Bucket(string _bucket_name, string _access_token, Settings _settings)
        {
            // TODO check for missing settings
            bucket_name = _bucket_name;
            settings = _settings;
            access_token = _access_token;
        }

        public string Name
        {
            get
            {
                return bucket_name;
            }
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
        /// Get
        /// </summary>
        /// 
        public event EventHandler<BucketEventArgs<GetResult>> GetCompleted;
        public void Get(string object_id, int version = -1)
        {   
            WebClient client = getWebClient();
            client.DownloadStringCompleted += GetRemoteRequestCompleted;
            string uri = getObjectUriString(object_id, version);
            client.DownloadStringAsync(new Uri(uri));
        }

        void GetRemoteRequestCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                GetResult result = new GetResult(extractVersionFromHeaders(sender as WebClient), e.Result);
                OnGetCompleted(new BucketEventArgs<GetResult>(result));
            }
            else
                OnGetCompleted(new BucketEventArgs<GetResult>(e.Error));
        }

        protected virtual void OnGetCompleted(BucketEventArgs<GetResult> e)
        {
            if (GetCompleted != null)
                GetCompleted(this, e);
        }

        /// <summary>
        /// Set
        /// </summary>
        public event EventHandler<BucketEventArgs<SetResult>> SetCompleted;
        public void Set(string object_id, string data, int version = -1, 
            bool response = false, bool replace = false, string clientid = null, int ccid = -1)
        {
            WebClient client = getWebClient();
            client.UploadStringCompleted += SetRemoteRequestCompleted;

            string uri = getObjectUriString(object_id, version);
            
           
            Dictionary<string,string> parameters = new Dictionary<string,string> ();
            if (response)
                parameters.Add("response", "1");
            if (replace)
                parameters.Add("replace", "1");
            if (clientid != null)
                parameters.Add("clientid", clientid);
            if (ccid >= 0)
                parameters.Add("ccid", "" + ccid);
            uri += appendQueryParameters(parameters);
            

            client.UploadStringAsync(new Uri(uri), data);
        }

        void SetRemoteRequestCompleted(object sender, UploadStringCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                SetResult result = new SetResult(extractVersionFromHeaders(sender as WebClient), e.Result);
                OnSetCompleted(new BucketEventArgs<SetResult>(result));
            }
            else
                OnSetCompleted(new BucketEventArgs<SetResult>(e.Error));
        }

        protected virtual void OnSetCompleted(BucketEventArgs<SetResult> e)
        {
            if (SetCompleted != null)
                SetCompleted(this, e);
        }

        /// <summary>
        /// Deletes
        /// </summary>
        public event EventHandler<BucketEventArgs<DeleteResult>> DeleteCompleted;
        
        public void Delete(string object_id, int version, string clientid = null, int ccid = -1)
        {
            WebClient client = getWebClient();
            client.UploadStringCompleted += DeleteRemoteRequestCompleted;
            string uri = getObjectUriString(object_id, version);
            
            
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            if (clientid != null)
                parameters.Add("clientid", clientid);
            if (ccid >= 0)
                parameters.Add("ccid", "" + ccid);
            uri += appendQueryParameters(parameters);
            
            client.UploadStringAsync(new Uri(uri), "DELETE", "");   
        }

        protected virtual void OnDeleteCompleted(BucketEventArgs<DeleteResult> e)
        {
            if (DeleteCompleted != null)
                DeleteCompleted(this, e);
        }

        void DeleteRemoteRequestCompleted(object sender, UploadStringCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                DeleteResult result = new DeleteResult(extractVersionFromHeaders(sender as WebClient));
                OnDeleteCompleted(new BucketEventArgs<DeleteResult>(result));
            }
            else
                OnDeleteCompleted(new BucketEventArgs<DeleteResult>(e.Error));
        }


        /// <summary>
        /// Index
        /// </summary>
        public void Index()
        {

        }

        /// <summary>
        /// Changes
        /// </summary>
        public void Changes()
        {

        }

        /// <summary>
        /// Helper Functions
        /// </summary>
        /// 

        private WebClient getWebClient()
        {
            WebClient client = new WebClient();
            client.Headers["User-Agent"] = settings.USER_AGENT;
            client.Headers["X-Simperium-Token"] = access_token;
            return client;
        }

        private string getObjectUriString(string object_id, int version)
        {
            string uri = "https://api.simperium.com/" + settings.API_VERSION + "/" +
                settings.APP_ID + "/" + bucket_name + "/i/" + object_id;
            if (version >= 0)
                uri += "/v/" + version;
            return uri;
        }

        private int extractVersionFromHeaders(WebClient client)
        {
           string s = client.ResponseHeaders["X-Simperium-Version"];
           int num = 0;
           bool ok = Int32.TryParse(s, out num);
           if (ok)
               return num;
           return -1;
        }

        private string appendQueryParameters(Dictionary<string, string> parameters)
        {
            if (parameters.Count == 0)
                return null;
            string query = "?";
            foreach (KeyValuePair<string, string> pair in parameters)
                query += pair.Key + "=" + pair.Value + "&";
            query = query.Remove(query.Length - 1);
            return query;
        }

        public static string GenerateId()
        {
            return Guid.NewGuid().ToString();
        }

    }

    public class GetResult
    {
        public int version { get; private set; }
        public string data { get; private set; }
        public GetResult(int _version, string _data)
        {
            version = _version;
            data = _data;
        }
    }

    // yes, this is exactly the same as GetResult
    public class SetResult
    {
        public int version { get; private set; }
        public string data { get; private set; }
        public SetResult(int _version, string _data)
        {
            version = _version;
            data = _data;
        }
    }

    public class DeleteResult
    {
        public int version { get; private set; }
        public DeleteResult(int _version)
        {
            version = _version;
        }
    }

    public class BucketEventArgs<T> : EventArgs
    {
        public T Result { get; private set; }

        public Exception Error { get; private set; }

        public BucketEventArgs(Exception e)
        {
            Error = e;
        }

        public BucketEventArgs(T r)
        {
            Result = r;
        }
    }
}
