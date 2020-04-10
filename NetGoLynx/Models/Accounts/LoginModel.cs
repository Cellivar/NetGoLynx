namespace NetGoLynx.Models.Accounts
{
    public class LoginModel
    {
        public LoginModel() { }

        public LoginModel(
            bool isGoogleEnabled = false,
            bool isGitHubEnabled = false,
            bool isOktaEnabled = false)
        {
            IsGoogleEnabled = isGoogleEnabled;
            IsGitHubEnabled = isGitHubEnabled;
            IsOktaEnabled = isOktaEnabled;
        }

        public LoginModel(string[] schemas)
        {
            foreach (var schema in schemas)
            {
                switch (schema)
                {
                    case "GitHub":
                        IsGitHubEnabled = true;
                        break;
                    case "Google":
                        IsGoogleEnabled = true;
                        break;
                    case "Okta":
                        IsOktaEnabled = true;
                        break;
                }
            }
        }

        public bool IsGoogleEnabled { get; }

        public bool IsGitHubEnabled { get; }

        public bool IsOktaEnabled { get; }
    }
}
