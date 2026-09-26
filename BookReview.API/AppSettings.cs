using System.Collections.Generic;

namespace BookReview.API
{
    public class AppSettings
    {
        public string ConnString { get; set; } = string.Empty;
        public bool SeedOnStart { get; set; }

        public IEnumerable<string> ApiKeys { get; set; }
            = new List<string>();

        public JwtSettings JwtSettings { get; set; }
            = new JwtSettings();

        public EmailSettings EmailSettings { get; set; }
            = new EmailSettings();
    }

    public class EmailSettings
    {
        public string FromEmail { get; set; } = string.Empty;
        public string AppPassword { get; set; } = string.Empty;
    }

    public class JwtSettings
    {
        public string SecretKey { get; set; } = string.Empty;
        public string Issuer { get; set; } = string.Empty;
        public int DurationSeconds { get; set; }
        public int RefreshTokenHours { get; set; }
    }
}