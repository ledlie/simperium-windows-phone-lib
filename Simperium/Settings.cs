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

namespace Simperium
{
    public class Settings
    {
        public string APP_ID { get; set; }
        public string API_KEY { get; set; }
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
