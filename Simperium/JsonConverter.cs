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
using System.Runtime.Serialization.Json;
using System.IO;
using System.Text;

namespace Simperium
{
    public class JsonConverter<T>
    {

        DataContractJsonSerializer serializer;
        public JsonConverter()
        {
            serializer = new DataContractJsonSerializer(typeof(T));
        }

        public T ConvertJsonToObject(string json)
        {
            try
            {
                MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(json));
                T obj = (T)serializer.ReadObject(ms);
                ms.Close();
                return obj;
            }
            catch (System.Runtime.Serialization.SerializationException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
                return default(T);
            }
        }


        public string ConvertObjectToJson(T obj)
        {
            try
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    serializer.WriteObject(ms, obj);
                    byte[] array = ms.ToArray();
                    string json = Encoding.UTF8.GetString(array, 0, array.Length);
                    ms.Close();
                    return json;
                }
            }
            catch (System.Runtime.Serialization.SerializationException ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
                return null;
            }
        }
    }
}
