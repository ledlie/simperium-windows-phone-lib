/*
 * Copyright 2012 Jonathan Ledlie
 *
 *  This file is part of the Simperium Windows Phone Library.
 *
 *  The Simperium Windows Phone Library is free software: you can
 *  redistribute it and/or modify it under the terms of the GNU Lesser
 *  General Public License as published by the Free Software Foundation,
 *  either version 3 of the License, or (at your option) any later
 *  version.
 * 
 *  The Simperium Windows Phone Library is distributed in the hope that it
 *  will be useful, but WITHOUT ANY WARRANTY; without even the implied
 *  warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See
 *  the GNU Lesser General Public License for more details.
 * 
 *  You should have received a copy of the GNU Lesser General Public
 *  License along with Simperium Windows Phone Library.  If not, see
 *  <http://www.gnu.org/licenses/>.
 */

﻿using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.IO;
using System.Text;

namespace Simperium
{
    public class UserAuth
    {
        Settings settings;

        public UserAuth(Settings _settings)
        {
            string missingSettings = _settings.Validate();
            if (missingSettings != null)
                throw new Exception(missingSettings);
            settings = _settings;
        }

        public void Authorize(string username, string password)
        {
            WebClient client = getWebClient();
            client.UploadStringCompleted += AuthorizeCompleted;
            Uri uri = new Uri("https://auth.simperium.com/" + settings.API_VERSION + "/" + settings.APP_ID + "/authorize/");
            // roll your own json...
            string content = "{\"username\":\"" + username + "\", \"password\":\"" + password + "\"}";
            client.UploadStringAsync(uri, content);
        }

        private WebClient getWebClient()
        {
            WebClient client = new WebClient();
            client.Headers["User-Agent"] = settings.USER_AGENT;
            client.Headers["X-Simperium-API-Key"] = settings.API_KEY;
            return client;
        }

        public event EventHandler<UserAuthEventArgs<AuthorizeResult>> AuthorizeUserCompleted;
        JsonConverter<AuthorizeResult> authorizeConverter = new JsonConverter<AuthorizeResult>();
        void AuthorizeCompleted(object sender, UploadStringCompletedEventArgs e)
        {
            if (e.Error == null)
                OnAuthorizeUserCompleted(new UserAuthEventArgs<AuthorizeResult>(authorizeConverter.ConvertJsonToObject(e.Result)));
            else
                OnAuthorizeUserCompleted(new UserAuthEventArgs<AuthorizeResult>(e.Error));
        }   

        protected virtual void OnAuthorizeUserCompleted(UserAuthEventArgs<AuthorizeResult> e)
        {
            if (AuthorizeUserCompleted != null)
                AuthorizeUserCompleted(this, e);
        }   


    }

    [DataContractAttribute]
    public class AuthorizeResult
    {
        [DataMember]
        public string access_token { get; set; }

        [DataMember]
        public string userid { get; set; }

        [DataMember]
        public string username { get; set; }

        public AuthorizeResult() { }

    }

    public class UserAuthEventArgs<T> : EventArgs
    {
        public T Result { get; private set; }

        public Exception Error { get; private set; }

        public UserAuthEventArgs(Exception e)
        {
            Error = e;
        }

        public UserAuthEventArgs(T r)
        {
            Result = r;
        }
    }

}

