﻿/*
 * Settings for sending emails, stored in configuration file (appsettings.json).
 */

namespace CinemaA.Settings
{
    public class MailSettings
    {
        public string Mail { get; set; }
        public string DisplayName { get; set; }
        public string Password { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
    }
}
