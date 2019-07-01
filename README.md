# DotNet Go Lynx

Golinks implementation in .NET!

### What

Go links are a popular internal corporate url shortener system. Instead of going to https://confluence.internal.domain.corp.local you can use go/wiki instead!

### Why?

My company didn't have one and it sounded like a fun learning experiment.

### What can it do?

Like any URL shortener, it can take a valid URL string (pretty much any characters other than ). This includes emoji! You can add and delete URLs as much as you like.

## Contributing

PRs accepted! Please make sure your code follows the editorconfig in the repository and otherwise adheres to standard C# development.

## Running

GoLynx is designed to be deployed as a docker container with a SQLite database file located nearby, preferably in a mounted volume of some sort. Add a DNS CNAME entry for your default DNS resolution domain for `go`, such as `go.contoso.local`, directing to wherever you've hosted the app. At that point you're off to the races.

Keep in mind that this is effectively a wide open DNS resolver. **You really should avoid running this on unsecured networks.** It can be used for all sorts of phishing and reflection attacks, so keep that in mind.

* Future support for other database backends is on the to-do list.
* External authentication is also on the to-do list.
* Custom domain support (so that clicking the copy to clipboard works as expected) is also coming.
