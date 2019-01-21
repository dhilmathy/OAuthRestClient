# OAuth REST Client

[![License](https://img.shields.io/pypi/l/sfctl.svg)](https://github.com/dhilmathy/OAuthRestClient/blob/master/LICENSE)

This is simple, light package created using Microsoft Authentication Library ([MSAL](https://github.com/AzureAD/microsoft-authentication-library-for-dotnet)) to access REST API using OAuth2 and OpenID Connect authentication.

## Get Started

```csharp

using MsalRestClient;
using System;

namespace MsalRestClientTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var configProvider = new ConfigProvider();
            var restClient = new RestClient(configProvider, null);
            Console.WriteLine(restClient.GetAsync("").Result);
            Console.ReadLine();
        }
    }
}


```

## API Documentation

This C# library provide a thin REST Client using OAuth authentication.
