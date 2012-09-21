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
using System.Collections.Generic;

namespace Simperium
{
    /// <summary>
    /// "Meta" operations on data buckets that require an admin access token
    /// </summary>
    public class Metadata : Data
    {

        public Metadata(string admin_access_token, Settings settings) : base(admin_access_token, settings) { }

        /// <summary>
        /// ChangesSince
        /// </summary>
        public void ChangesSince(string bucket_name, string since = null, string username = null, bool returnData = false)
        {
            // TODO test Index before pasting code here
        }

        /// <summary>
        /// ChangesSince
        /// </summary>
        public event EventHandler<DataEventArgs<ListBucketsResult>> ListBucketsCompleted;
        public void ListBuckets()
        {
            WebClient client = getWebClient();
            client.DownloadStringCompleted += ListBucketsRemoteRequestCompleted;
            string uri = getBaseUriString() + "/buckets";
            client.DownloadStringAsync(new Uri(uri));
        }

        void ListBucketsRemoteRequestCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                ListBucketsResult result = null; // = new GetResult(extractVersionFromHeaders(sender as WebClient), e.Result);
                OnListBucketsCompleted(new DataEventArgs<ListBucketsResult>(result));
            }
            else
                OnListBucketsCompleted(new DataEventArgs<ListBucketsResult>(e.Error));
        }

        protected virtual void OnListBucketsCompleted(DataEventArgs<ListBucketsResult> e)
        {
            if (ListBucketsCompleted != null)
                ListBucketsCompleted(this, e);
        }

    }

    public class ListBucketsResult
    {
        public List<string> names { get; private set; }
        public ListBucketsResult(List<string> names)
        {
            this.names = names;
        }
    }

}
