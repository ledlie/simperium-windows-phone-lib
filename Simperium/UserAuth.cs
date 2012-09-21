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
        string api_key;
        Settings settings;

        public UserAuth(string api_key, Settings settings)
        {
            string missingSettings = settings.Validate();
            if (missingSettings != null)
                throw new Exception(missingSettings);
            this.settings = settings;
            this.api_key = api_key;
        }

        public void Authorize(string username, string password)
        {
            WebClient client = getWebClient();
            client.UploadStringCompleted += AuthorizeCompleted;
            String uri = getBaseUri() + "/authorize/";
            string json = usernamePasswordToJson(username, password);
            client.UploadStringAsync(new Uri(uri), json);
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


        public void Create(string username, string password) {
            WebClient client = getWebClient();
            client.UploadStringCompleted += CreateCompleted;
            String uri = getBaseUri() + "/create/";
            string json = usernamePasswordToJson(username, password);
            client.UploadStringAsync(new Uri(uri), json);
        }

        public event EventHandler<UserAuthEventArgs<CreateResult>> CreateUserCompleted;
        JsonConverter<CreateResult> createConverter = new JsonConverter<CreateResult>();
        void CreateCompleted(object sender, UploadStringCompletedEventArgs e)
        {
            if (e.Error == null)
                OnCreateUserCompleted(new UserAuthEventArgs<CreateResult>(createConverter.ConvertJsonToObject(e.Result)));
            else
                OnCreateUserCompleted(new UserAuthEventArgs<CreateResult>(e.Error));
        }

        protected virtual void OnCreateUserCompleted(UserAuthEventArgs<CreateResult> e)
        {
            if (CreateUserCompleted != null)
                CreateUserCompleted(this, e);
        }

        public void Update(string username, string password, string new_username, string new_password)
        {
            WebClient client = getWebClient();
            client.UploadStringCompleted += UpdateCompleted;
            String uri = getBaseUri() + "/update/";
            string json = usernamePasswordToJson(username, password, new_username, new_password);
            client.UploadStringAsync(new Uri(uri), json);
        }

        public event EventHandler<UserAuthEventArgs<UpdateResult>> UpdateUserCompleted;
        JsonConverter<UpdateResult> updateConverter = new JsonConverter<UpdateResult>();
        void UpdateCompleted(object sender, UploadStringCompletedEventArgs e)
        {
            if (e.Error == null)
                OnUpdateUserCompleted(new UserAuthEventArgs<UpdateResult>(updateConverter.ConvertJsonToObject(e.Result)));
            else
                OnUpdateUserCompleted(new UserAuthEventArgs<UpdateResult>(e.Error));
        }

        protected virtual void OnUpdateUserCompleted(UserAuthEventArgs<UpdateResult> e)
        {
            if (UpdateUserCompleted != null)
                UpdateUserCompleted(this, e);
        }



        public void ResetPassword(string username, string new_password)
        {
            WebClient client = getWebClient();
            client.UploadStringCompleted += ResetPasswordCompleted;
            String uri = getBaseUri() + "/reset_password/";
            string json = "{\"username\":\"" + username + "\", \"new_password\":\"" + new_password + "\"}";
            client.UploadStringAsync(new Uri(uri), json);
        }

        public event EventHandler<UserAuthEventArgs<UpdateResult>> ResetPasswordUserCompleted;
        JsonConverter<ResetPasswordResult> resetPasswordConverter = new JsonConverter<ResetPasswordResult>();
        void ResetPasswordCompleted(object sender, UploadStringCompletedEventArgs e)
        {
            if (e.Error == null)
                OnResetPasswordUserCompleted(new UserAuthEventArgs<ResetPasswordResult>(resetPasswordConverter.ConvertJsonToObject(e.Result)));
            else
                OnResetPasswordUserCompleted(new UserAuthEventArgs<ResetPasswordResult>(e.Error));
        }

        protected virtual void OnResetPasswordUserCompleted(UserAuthEventArgs<ResetPasswordResult> e)
        {
            if (ResetPasswordUserCompleted != null)
                ResetPasswordUserCompleted(this, e);
        }


        /// <summary>
        /// Helper functions
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>

        private string usernamePasswordToJson(string username, string password)
        {
            return "{\"username\":\"" + username + "\", \"password\":\"" + password + "\"}";
        }

        private string usernamePasswordToJson(string username, string password, string new_username, string new_password)
        {
            string json = "\"username\":\"" + username + "\", \"password\":\"" + password + "\"";
            if (!String.IsNullOrWhiteSpace(new_username))
                json += ", \"new_username\":\"" + new_username + "\"";
            if (!String.IsNullOrWhiteSpace(new_password))
                json += ", \"new_password\":\"" + new_password + "\"";
            json = "{" + json + "}";
            return json;
        }


        private string getBaseUri()
        {
            return "https://auth.simperium.com/" + settings.API_VERSION + "/" + settings.APP_ID;
        }


        private WebClient getWebClient()
        {
            WebClient client = new WebClient();
            client.Headers["User-Agent"] = settings.USER_AGENT;
            client.Headers["X-Simperium-API-Key"] = api_key;
            return client;
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
    
    // yes, this is the same as AuthorizeResult
    [DataContractAttribute]
    public class CreateResult
    {
        [DataMember]
        public string access_token { get; set; }

        [DataMember]
        public string userid { get; set; }

        [DataMember]
        public string username { get; set; }

        public CreateResult() { }

    }

    [DataContractAttribute]
    public class UpdateResult
    {
        public UpdateResult() { }

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

