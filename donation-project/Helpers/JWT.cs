﻿namespace donation_project.Helpers
{
    public class JWT
    {
        public string SecurityKey { get; set; }
        public string AudienceIP { get; set; }
        public string IssureIP { get; set; }
        public double DurationInDays { get; set; }
    }
}
