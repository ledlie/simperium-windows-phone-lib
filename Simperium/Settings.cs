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

namespace Simperium
{
    /// <summary>
    /// You should be able to create one Settings object shared across your Bucket, Metadata, Data, and UserAuth objects,
    /// as these settings should not change while you are using the library.
    /// </summary>
    public class Settings
    {
        public string APP_ID { get; set; }
        public string USER_AGENT { get; set; }
        public string API_VERSION { get; set; }
        
        public Settings () {
            API_VERSION = "1";
        }

        internal string Validate()
        {
            if (String.IsNullOrWhiteSpace(USER_AGENT))
                return "Missing Settings: USER_AGENT";
            if (String.IsNullOrWhiteSpace(API_VERSION))
                return "Missing Settings: API_VERSION";
            if (String.IsNullOrWhiteSpace(APP_ID))
                return "Missing Settings: APP_ID";
            return null;
        }
    }
}
