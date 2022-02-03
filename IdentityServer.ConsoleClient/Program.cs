using System;
using System.Collections.Generic;
using System.Net.Http;
using IdentityModel.Client;

namespace IdentityServer.ConsoleClient
{
    abstract class Program
    {
        static void Main(string[] args)
        {
            using var httpclint = new HttpClient();
            var discoveryDoc = httpclint.GetDiscoveryDocumentAsync("https://localhost:5000/").Result;
            if (discoveryDoc.IsError)
            {
                Console.WriteLine("Error GetDiscoveryDocumentAsync");
                return;
            }

            using var requestClient = new HttpClient();
            var token = requestClient.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest()
            {
                Address = discoveryDoc.TokenEndpoint,
                ClientId = "m2m.postman",
                ClientSecret = "password",
                Scope = "api.read"
            }).Result;
            if (token.IsError)
            {
                Console.WriteLine("Error token");
                return;
            }
            Console.WriteLine(token.AccessToken);

            using var client = new HttpClient();
            client.SetBearerToken(token.AccessToken);
            var result = client.GetAsync("http://localhost:4000/api/people").Result;
            if (result.IsSuccessStatusCode)
            {
                var people =
                    Newtonsoft.Json.JsonConvert.DeserializeObject<List<Person>>(result.Content.ReadAsStringAsync()
                        .Result);
                foreach (var person in people!)
                {
                    Console.WriteLine($"FirstName {person.FirstName} - LastName {person.LastName} - age {person.Age}");
                }
            }
            else
            {
                Console.WriteLine("error" + result.StatusCode);
            }
        }

        private class Person
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public int Age { get; set; }
        }
    }
}