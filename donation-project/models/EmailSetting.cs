﻿namespace donation_project.models
{
    public class EmailSetting
    {
        public int Port { get; set; }
        public string Host { get; set; }
        public bool EnableSsl { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
    }
}
