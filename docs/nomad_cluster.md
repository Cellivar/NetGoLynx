# Running on Nomad

At home I have a Nomad cluster running on a few machines, this is where I run (and test!) NetGoLynx. Nomad is [a cluster management system](https://www.nomadproject.io/) built by HashiCorp that is generally lighter and easier to deal with than Kubernetes. I also use Consul and Vault for storage of configuration values and service discovery.

## General overview

NetGoLynx only needs a database of some sort, it doesn't need to mount a volume unless you're using that volume to store the database (like with sqlite). Using an external database is the preferred hosting method as it gives you better control over the backup of the database. For this I use Postgres running elsewhere on my cluster.

Outside of the database NetGoLynx can just be run somewhere. It's designed to have a load balancer do TLS termination so you can handle TLS certificates via your favorite service instead of having to import them into the running service. For this I use Fabio with a Let's Encrypt certificate stored in Vault. Fabio makes use of a specific set of tags in the Service stanza, which is why you see

Obviously NetGoLynx will require DNS entries. I have dnsmasq running as a service in my cluster that is managed automatically with Consul. You'll see the service stanza has some odd tags, this is why.

To update the job file I make use of Terraform. Terraform lets me store the configuration of various jobs in my cluster as templates in Git and as state in Vault. This example doesn't make use of Terraform to simplify things. You could simplify a number of the parameters if you used Terraform instead of this raw jobpsec.

### Nomad Job Spec

netgolnyx.nomad:

```hcl
job "netgolynx" {
    datacenters = ["your_datacenter_name"]

    group "main" {
        count = 1

        network {
            port "http" { to = 80 }
        }

        task "netgolynx-ui"{
            driver = "docker"
            config {
                image   = "netgolynx"
                ports   = ["http"]
                volumes = [
                    "secrets/appsettings.json:/app/appsettings.json"
                ]
            }

            # The vault policy you use should have read access to the secret path
            # used in the template below.
            vault {
                policies    = [ "vault_policy_for_secrets" ]
                change_mode = "restart"
            }

            constraint {
                attribute = "${attr.cpu.arch}"
                value     = "amd64"
            }

            # This is the appsettings file, templatized to use Consul-Template sytanx
            # The values will be populated from Vault, where I store the credentials
            # for various things, including this app.
            template {
                data = <<EOH
{
{{ with secret "secret/apps/netgolynx" }}
  "ConnectionStrings": {
    "PostgreSQL": "Host=my.postgres.server;Database=netgolynx;Username={{ .Data.data.connectionstrings_postgresql_username }};Password={{ .Data.data.connectionstrings_postgresql_password }}"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Warning"
    }
  },
  "AllowedHosts": "*",
  "AccountSettings": {
    "AdminList": "my_email_address@example.com"
  },
  "Authentication": {
    "GitHub": {
      "ClientId": "{{ .Data.data.github_clientid }}",
      "ClientSecret": "{{ .Data.data.github_clientsecret }}",
      "AuthorizationEndpoint": "https://github.com/login/oauth/authorize",
      "TokenEndpoint": "https://github.com/login/oauth/access_token",
      "UserInformationEndpoint": "https://api.github.com/user",
      "UserEmailsEndpoint": "https://api.github.com/user/emails"
    }
  },
  "ProxyNetworks": {
      "AllowedHosts": ["go.my.domain", "go"],
      "WebInterfaceHost": "https://go.my.domain"
  }
}
{{ end }}

EOH
                destination = "secrets/appsettings.json"
                change_mode = "restart"
            }

            # I've found NetGoLynx to be pretty lightweight!
            resources {
                cpu    = 25
                memory = 250
            }

            service {
                name = "netgolynx"
                port = "http"
                # The urlprefix- tags here are used by Fabio to add routing rules
                # to the load balancer. Fabio handles TLS termination so the app
                # doesn't need to.
                # In order they:
                # Redirect go/ to the proper https domain
                # Redirect go.my.domain/ to the proper https domain
                # Route https://go.my.domain to the running app instance.
                tags = [
                    "infrastructure",
                    "urlprefix-go/ redirect=301,https://go.my.domain$path",
                    "urlprefix-go.my.domain:80/ redirect=301,https://go.my.domain$path",
                    "urlprefix-go.my.domain/",
                    "hostname"
                    ]
                # This meta tag is used by dnsmasq's consul-template to populate
                # the
                meta {
                    hostname = "go.my.domain"
                }
                check {
                    name     = "alive"
                    type     = "http"
                    path     = "/_/health"
                    interval = "10s"
                    timeout  = "2s"
                }
            }
        }
    }
}
```

### Health checks

When running in a cluster management of some sort you'll probably want to configure a healthcheck at `/_/health`. By default this will run a handful of fairly lightweight checks to ensure that the app can support redirecting URLs. If the app is in a state where it cannot resolve redirects it will show as unhealthy.

Why not use the more standard `/health` endpoint? Simple: that's a valid shortlink! It could be a link to your company's health practices, or maybe the gym membership signup information. All API endpoints are under the `/_/` path, including the health endpoint.