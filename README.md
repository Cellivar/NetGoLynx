# DotNet Go Lynx

Go links implementation in .NET!

### What

Go links are a popular internal corporate url shortener system. Instead of going to https://confluence.internal.domain.corp.local/this/isan/obnoxious/url/tofind/each/time/youneed/it/ugh you can use go/wiki instead!

### Why?

My company didn't have one and it sounded like a fun learning experiment.

### What can it do?

Like any URL shortener, it can take a valid URL string (anything other than ``{} | ^ ~ [] ` ; /\ ? : @ # = % & <>`` and `spaces`). This includes emoji 👍! You can add and delete URLs as much as you like.

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
3. Add the GitHub ClientId and ClientSecret to the appsettings.json file.
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

Keep in mind that as soon as you close that window or stop the process you will lose all your links.

### Configuration

This app is configured via appsettings.json. You can find an example (with details you should fill out) provided in the repo that you can either edit directly, or provide an override file in the same directory called appsettings.release.json.

The basic structure looks like this and is explained section by section below.

```json
{
  "ConnectionStrings": {
    ...
  },
  "Logging": {
    "LogLevel": {
      "Default": "Warning"
    }
  },
  "AllowedHosts": "*",
  "Authentication": {
    ...
  },
  "ProxyNetworks": {
    ...
  }
}
```

#### ConnectionStrings

This section contains the connection string to connect to the appropriate SQL data store. There should only be one connection string at a time, the app doesn't know what to do with more than one.

The database provider is chosen based on the name of the connection string key provided. You should provide a connection string in the format appropriate for the provide you select. The key must match with one of the implemented providers:

* DefaultConnection: Alias for Sqlite.
* Sqlite: For a local (or volume-mapped) SQLite database. Good for home installations.
    * Example: `"Sqlite": "Data Source=redirects.db"`
* SqlServer: For an MS SQL Server connection.
    * Example: `"SQLServer": "server=127.0.0.1,1433;Database=netgolynx;User Id=SA;Password=yourStrong(!)Password"`
* Postgresql: For a PostgreSQL server connection.
    * Example: `"PostgreSQL": "Host=localhost;Database=netgolynx;Username=postgres;Password=password"`

NetGoLynx is configured to fire off database migrations automatically when the app starts to make sure the database is present and the schema is up to date. If there are any breaking schema changes that requires any kind of manual migration the release notes will walk you through the operations necessary.

There is no mechanism for migrating between database providers at this time, taking a backup of the existing provider and loading it into a new provider after running NetGoLynx once may just work. Alternatively pester me to implement full import/export or something.

#### Logging

Control how noisy the logging system is to the log file.

#### AllowedHosts

A semicolon separated list of hosts the app will run as. Using `*` allows anything, which is probably too broad for production.

If you wanted to host NetGoLynx on your domain at go.contoso.com you'd want to use:

```json
    "AllowedHosts": "go.contoso.local;go"
```

The list of hosts is semicolon separated and should not include port numbers. You'll probably want to have "go" in that list specifically if you want the go/link format to work.

If you're running this behind a TLS-terminating load balancer on a secure network setting it to "*" may be fine.

Note: This is seprate from the ProxyNetworks configuration, in case your load balancer configuration needs it. Modern load balancer proxy systems such as Azure Front Door or AWS ALBs should be able to use the same values for both.

#### Authentication

Configure the available authentication providers. You must have at least one configured. You can configure as many as you want to support.

##### GitHub

Authenticate using a GitHub app. You can configure this to be either GitHub.com or a GitHub Enterprise instance. Follow the [instructions for creating a GitHub App](https://docs.github.com/en/developers/apps/creating-a-github-app) and get a ClientId and ClientSecret. You'll need to update the URLs for the various endpoints if you're using GitHub Enterprise.

The only permission the app requires is Read-Only access to Email addresses. **You should not grant the app any other permissions**.

The GitHub App's redirect URL will be `<your host>/_/api/v1/account/signin-github`.

Example with the appropriate URLs for github.com:

```json
    "GitHub": {
      "ClientId": "Paste your client ID here",
      "ClientSecret": "Paste your client secret here",
      "AuthorizationEndpoint": "https://github.com/login/oauth/authorize",
      "TokenEndpoint": "https://github.com/login/oauth/access_token",
      "UserInformationEndpoint": "https://api.github.com/user",
      "UserEmailsEndpoint": "https://api.github.com/user/emails"
    }
```

##### Google

Authenticate using Google authentication. You'll need to follow the process to create a [Google OAuth App](https://developers.google.com/identity/sign-in/web/sign-in) and you _absolutely_ must use an HTTPS endpoint for this app. This can make supporting the go/ dns serch domain tricky. Good luck.

The Google App's redirect URL will be `<your host>/_/api/v1/account/signin-google`.

```json
    "Google": {
      "ClientId": "Paste your client ID here",
      "ClientSecret": "Paste your client secret here"
    }
```

##### Okta

Authenticate using an Okta account. You'll need to create an OAuth2 app in your Okta developer console (not the Admin console) following [the guide](https://developer.okta.com/docs/guides/implement-oauth-for-okta/create-oauth-app/). The only allowed grant type that is required is Authorization Code, the rest can be unchecked.

The Okta App's redirect URL will be `<your host>/_/api/v1/account/signin-okta`.

The OktaDomain needs to be your Okta account's Organization URL. You can find it by visiting your developer dashboard and looking in the top-right. Don't include a trailing slash!

If you are using Custom Authorization Servers this may not work out of the box for you. Please file an issue or otherwise let me know that this is a feature you need and I can easily add in support for it. [The API URLs change slightly when using a Custom Auth Server.](https://developer.okta.com/docs/reference/api/oidc/)

```json
    "Okta": {
      "ClientId": "Paste your client ID here",
      "ClientSecret": "Paste your client secret here",
      "OktaDomain": "https://some-okta-account.okta.com"
    }
```

#### ProxyNetworks

You'll probably be running NetGoLynx behind a TLS-terminating load balancer, such as in a Docker container in k8s or Nomad. When you do this you'll want to configure the app to listen to X-Forwarded headers so that it will properly handle TLS termination at your load balancer.

You will also want to limit the allowed hosts from X-Forwarded-Host, usually set to the domain that you're running it on. So if you plan to run NetGoLynx at go.contoso.com you'll want to limit the AllowedHosts to just that domain like so:

```json
    "ProxyNetworks": {
      "AllowedHosts":  ["go.contoso.local", "go"]
    },
```

You'll probably want to make sure `go` is in the list.

Note: This list is seprate from the AllowedHosts above to be more flexible for your proxy configuration. Modern proxy systems such as Azure Front Door or AWS ALBs should be able to use the same values for both.

#### Complete Example

An example configuration all together:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=redirects.db"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Warning"
    }
  },
  "AllowedHosts": "go.contoso.local;go",
  "Authentication": {
    "GitHub": {
      "ClientId": "This is very not real",
      "ClientSecret": "Also not real!",
      "AuthorizationEndpoint": "https://github.com/login/oauth/authorize",
      "TokenEndpoint": "https://github.com/login/oauth/access_token",
      "UserInformationEndpoint": "https://api.github.com/user",
      "UserEmailsEndpoint": "https://api.github.com/user/emails"
    },
    "Okta": {
      "ClientId": "This is not very real!",
      "ClientSecret": "Still not real at all!",
      "OktaDomain": "URL of your okta account with no trailing slash!"
    }
  },
  "ProxyNetworks": {
    "AllowedHosts": ["go.contoso.local", "go"],
  }
}
```
