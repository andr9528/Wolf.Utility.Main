﻿using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Wolf.Utility.Core.Extensions.Methods
{
    public static class HttpClientExtensions
    {
        /// <summary>
        /// Ensures the call was successful, meaning it will crash, if it failed.
        /// </summary>
        /// <param name="httpClient"></param>
        /// <param name="url"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public static async Task<string> GetStringViaPostAsync(this HttpClient httpClient, string url, HttpContent content = null)
        {
            var response = await httpClient.PostAsync(url, content);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
        }

        public static Task<HttpResponseMessage> PostAsJsonAsync<T>(this HttpClient httpClient, string url, T data)
        {
            var dataAsString = JsonConvert.SerializeObject(data, Formatting.Indented,
                new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.Objects,
                    TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple
                });


            var content = new StringContent(dataAsString);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            return httpClient.PostAsync(url, content);
        }

        public static Task<HttpResponseMessage> PutAsJsonAsync<T>(this HttpClient httpClient, string url, T data)
        {
            var dataAsString = JsonConvert.SerializeObject(data, Formatting.Indented,
                new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.Objects,
                    TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple
                });

            var content = new StringContent(dataAsString);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            return httpClient.PutAsync(url, content);
        }

        public static Task<HttpResponseMessage> DeleteByJsonAsync<T>(this HttpClient httpClient, string url, T data)
        {
            var dataAsString = JsonConvert.SerializeObject(data, Formatting.Indented,
                new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.Objects,
                    TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple
                });

            var content = new StringContent(dataAsString);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            HttpRequestMessage request = new HttpRequestMessage()
            {
                Content = content,
                Method = HttpMethod.Delete,
                RequestUri = new Uri(httpClient.BaseAddress + url)
            };
            return httpClient.SendAsync(request);
        }

        public static Task<HttpResponseMessage> GetByJsonAsync<T>(this HttpClient httpClient, string url, T data)
        {
            var dataAsString = JsonConvert.SerializeObject(data, Formatting.Indented,
                new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.Objects,
                    TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple
                });

            var content = new StringContent(dataAsString);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            HttpRequestMessage request = new HttpRequestMessage()
            {
                Content = content,
                Method = HttpMethod.Get,
                RequestUri = new Uri(httpClient.BaseAddress + url)
            };
            return httpClient.SendAsync(request);
        }

        /// <summary>
        /// Creates a query request to the base address of the client, at the controller specified in the url input, with any 'ValueType's in the data given.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="httpClient"></param>
        /// <param name="url"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static Task<HttpResponseMessage> GetByQueryAsync<T>(this HttpClient httpClient, string url, T data)
        {
            var builder = new UriBuilder($"{httpClient.BaseAddress}{url}?");
            var type = typeof(T);
            var properties = type.GetProperties();

            foreach (var info in properties)
            {
                if(!info.PropertyType.IsValueType) continue;

                builder.Query += $"{info.Name}={info.GetValue(data)}&";
            }

            builder.Query = builder.Query.TrimEnd('&');

            HttpRequestMessage request = new HttpRequestMessage()
            {
                Method = HttpMethod.Get,
                RequestUri = builder.Uri
            };

            return httpClient.SendAsync(request);
        }
    }
}
