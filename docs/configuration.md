
# Configuration

This app is configured via appsettings.json. You can find an example (with details you should fill out) provided in the repo that you can either edit directly, or provide an override file in the same directory called appsettings.release.json.

The basic structure looks like this and is explained section by section below. A complete example is at the end of the page.

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

## ConnectionStrings

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

## Logging

Control how noisy the logging system is to the log file.

## AllowedHosts

A semicolon separated list of hosts the app will run as. Using `*` allows anything, which is probably too broad for production.

If you wanted to host NetGoLynx on your domain at go.contoso.com you'd want to use:

```json
    "AllowedHosts": "go.contoso.local;go"
```

The list of hosts is semicolon separated and should not include port numbers. You'll probably want to have "go" in that list specifically if you want the go/link format to work.

If you're running this behind a TLS-terminating load balancer on a secure network setting it to "*" may be fine.

Note: This is seprate from the ProxyNetworks configuration, in case your load balancer configuration needs it. Modern load balancer proxy systems such as Azure Front Door or AWS ALBs should be able to use the same values for both.

## Authentication

Configure the available authentication providers. You must have at least one configured. You can configure as many as you want to support.

### GitHub

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

### Google

Authenticate using Google authentication. You'll need to follow the process to create a [Google OAuth App](https://developers.google.com/identity/sign-in/web/sign-in) and you _absolutely_ must use an HTTPS endpoint for this app. This can make supporting the go/ dns serch domain tricky. Good luck.

The Google App's redirect URL will be `<your host>/_/api/v1/account/signin-google`.

```json
    "Google": {
      "ClientId": "Paste your client ID here",
      "ClientSecret": "Paste your client secret here"
    }
```

### Okta

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

## ProxyNetworks

You'll probably be running NetGoLynx behind a TLS-terminating load balancer, such as in a Docker container in k8s or Nomad. When you do this you'll want to configure the app to listen to X-Forwarded headers so that it will properly handle TLS termination at your load balancer.

You will also want to limit the allowed hosts from X-Forwarded-Host, usually set to the domain that you're running it on. So if you plan to run NetGoLynx at go.contoso.com you'll want to limit the AllowedHosts to just that domain like so:

```json
    "ProxyNetworks": {
      "AllowedHosts":  ["go.contoso.local", "go"]
    },
```

You'll probably want to make sure `go` is in the list.

Note: This list is seprate from the AllowedHosts above to be more flexible for your proxy configuration. Modern proxy systems such as Azure Front Door or AWS ALBs should be able to use the same values for both.

### WebInterfaceHost

When running NetGoLynx under multiple DNS names you will generally want to redirect users to a single HTTPS web interface that you've configured for your various OAuth providers. This includes redirecting http://go to a proper HTTPS address.

NetGoLynx provides an internal mechanism for doing this. When you provide a WebInterfaceHost in the ProxyNetworks configuration it will redirect any UI or API requests that don't match the host and scheme you provide.

For example,

```json
    "ProxyNetworks": {
      "AllowedHosts":  ["go.contoso.local", "go"],
      "WebInterfaceHost": "https://go.contoso.local"
    },
```

will redirect `http://go/_/Account/Login` to `https://go.contoso.local/_Account/Login` automatically. This helps ensure users are using a secure connection when attempting to create links, or sign in.

It _will not_ redirect `http://go/some_redirect` to the HTTPS url, it will simply return the redirect to the ultimate URL instead. If you'd like to ensure go/links always route through HTTPS you might want to look into the [companion browser extension](https://github.com/Cellivar/NetGoLynx-Extension) which will internally handle the redirect from `http://go/` to `https://<your netgolynx server>`.

Note that you can provide a port here if you're, for some reason, running HTTPS over something other than 443.

## Complete Example

An example configuration all together:

```json
{
  "ConnectionStrings": {
    "Sqlite": "Data Source=redirects.db"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Warning"
    }
  },
  "AllowedHosts": "go.contoso.local;go",
  "Authentication": {
    "GitHub": {
      "ClientId": "Put in your ClientID here",
      "ClientSecret": "And put in your ClientSecret here",
      "AuthorizationEndpoint": "https://github.com/login/oauth/authorize",
      "TokenEndpoint": "https://github.com/login/oauth/access_token",
      "UserInformationEndpoint": "https://api.github.com/user",
      "UserEmailsEndpoint": "https://api.github.com/user/emails"
    },
    "Okta": {
      "ClientId": "Put in your Client ID here",
      "ClientSecret": "And your client secret here",
      "OktaDomain": "https://okta.contoso.com"
    }
  },
  "ProxyNetworks": {
    "AllowedHosts": ["go.contoso.local", "go"],
    "WebInterfaceHost": "https://go.contoso.local"
  }
}
```
