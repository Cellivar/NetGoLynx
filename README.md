# DotNet Go Lynx

Go links implementation in .NET!

### What

Go links are a popular internal corporate url shortener system. Instead of going to https://confluence.internal.domain.corp.local/this/isan/obnoxious/url/tofind/each/time/youneed/it/ugh you can use go/wiki instead!

### Why?

My company didn't have one and it sounded like a fun learning experiment.

### What can it do?

Like any URL shortener, it can take a valid URL string (anything other than ``{} | ^ ~ [] ` ; /\ ? : @ # = % & <>`` and `spaces`). This includes emoji 👍! You can add and delete URLs as much as you like.

There's just one exception: the redirect `_` is reserved for the user interface and the API.

## Contributing

PRs accepted! Please make sure your code follows the editorconfig in the repository and otherwise adheres to standard C# development practices.

## Running

GoLynx is designed to be deployed as a docker container with a SQLite database file located nearby, preferably in a mounted volume of some sort. Add a DNS CNAME entry for your default DNS resolution domain for `go`, such as `go.contoso.local`, directing to wherever you've hosted the app. At that point you're off to the races.

⚠ Keep in mind that this is effectively a wide open DNS resolver. **You really should avoid running this on unsecured networks.** It can be used for all sorts of phishing and reflection attacks. You have been warned.

### I just want to run the thing to look at it!

Unfortunately you'll need to have an HTTPS cert to be able to log into the app. Chrome, Edge, and other browsers have a security requirement around cookies that makes OAuth fail if you try to run NetGoLynx on non-HTTPS connections.

Fortunately it's easy to get set up with a development certificate locally with dotnet dev-certs.

To run it quickly:

1. Clone the repo.
2. [Create a GitHub App](https://github.com/settings/apps/new) using the config guide below.
  * Your callback URL will be https://localhost:8001/_/api/v1/account/signin-github
3. Add the GitHub ClientId and ClientSecret to the appsettings.json file, it should look something like the configuration example below
4. Delete the "Google" and "Okta" sections out of the appsettings.json file.
5. From the NetGoLynx directory:

PowerShell on Windows:
```powershell
# Generate the certificate
$pass = "{TypeSomeRandomCharactersHere}"
dotnet dev-certs https -ep $env:USERPROFILE\.aspnet\https\NetGoLynx.pfx -p $pass
dotnet dev-certs https --trust

# Build the docker container
cd ./NetGoLynx
docker build . -t netgolynx-personal

# Run it with the dev cert
docker run --rm -it -p 8001:443 -e ASPNETCORE_URLS="https://+" -e ASPNETCORE_HTTPS_PORT=8001 -e ASPNETCORE_Kestrel__Certificates__Default__Password="$pass" -e ASPNETCORE_Kestrel__Certificates__Default__Path=/https/NetGoLynx.pfx -v $env:USERPROFILE\.aspnet\https:/https/ netgolynx-personal
```

Open https://localhost:8001 in your browser and you should be good to go. Log in, add a link, then use `https://localhost:8001/your-link` to resolve it. If you're feeling really fancy you can add a hosts file redirect for `go` on your machine to really give it a spin.

Keep in mind that as soon as you close that powershell terminal or stop the process you will lose all your links, you should only use this for testing!

#### Configuration Example

```json
{
  "ConnectionStrings": {
    "Sqlite": "Data Source=redirects.db"
  },
  "AllowedHosts": "*",
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "System": "Information",
      "Microsoft": "Information"
    }
  },
  "AccountSettings": {
    "AdminList": "your_github_email_address"
  },
  "Authentication": {
    "GitHub": {
      "ClientId": "Your_ClientID_from_step_2",
      "ClientSecret": "Your_Client_Secret_from_step_2",
      "AuthorizationEndpoint": "https://github.com/login/oauth/authorize",
      "TokenEndpoint": "https://github.com/login/oauth/access_token",
      "UserInformationEndpoint": "https://api.github.com/user",
      "UserEmailsEndpoint": "https://api.github.com/user/emails"
    }
  }
}
```

## Configuration

For more details on configuration see [the configuration docs](docs/configuration.md).

## Running on Nomad

For an example jobspec for Nomad see [the configuration example](docs/nomad_cluster.md).


### Health checks

When running in a cluster management of some sort you'll probably want to configure a healthcheck at `/_/health`. By default this will run a handful of fairly lightweight checks to ensure that the app can support redirecting URLs. If the app is in a state where it cannot resolve redirects it will show as unhealthy.

Why not use the more standard `/health` endpoint? Simple: that's a valid shortlink! It could be a link to your company's health practices, or maybe the gym membership signup information. All API endpoints are under the `/_/` path, including the health endpoint.