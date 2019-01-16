using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Formatters;
using Newtonsoft.Json;

namespace DotEasy.Rpc.ApiGateway.AuthMiddleware
{
    public class TokenMiddleware
    {
        private readonly RequestDelegate _next;

        public TokenMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        // ReSharper disable once UnusedMember.Global
        public async Task Invoke(HttpContext context)
        {
            EnableReadAsync(context.Response);
            if (context.Request.Path.ToString() == "/connect/token")
            {
                context.Response.OnCompleted(async o =>
                {
                    if (o is HttpContext c)
                    {
                        var token = JsonConvert.DeserializeObject<Dictionary<string, string>>(await ReadBodyAsync(c.Response)
                            .ConfigureAwait(false));
                        var accessToken = token["access_token"];
                        Console.WriteLine(accessToken);
                    }
                }, context);
            }

            await _next(context);
        }

        private async Task<string> ReadBodyAsync(HttpResponse response)
        {
            if (response.Body.Length <= 0) return null;
            response.Body.Seek(0, SeekOrigin.Begin);
            var encoding = GetEncoding(response.ContentType);
            var retStr = await ReadStreamAsync(response.Body, encoding, false).ConfigureAwait(false);
            return retStr;
        }

        private static Encoding GetEncoding(string contentType)
        {
            var mediaType = contentType == null ? default(MediaType) : new MediaType(contentType);
            var encoding = mediaType.Encoding;
            if (encoding == null)
            {
                encoding = Encoding.UTF8;
            }

            return encoding;
        }

        private static void EnableReadAsync(HttpResponse response)
        {
            if (!response.Body.CanRead || !response.Body.CanSeek)
            {
                response.Body = new MemoryWrappedHttpResponseStream(response.Body);
            }
        }

        private async Task<string> ReadStreamAsync(Stream stream, Encoding encoding, bool forceSeekBeginZero = true)
        {
            using (var sr = new StreamReader(stream, encoding, true, 1024, true))
            {
                var str = await sr.ReadToEndAsync();
                if (forceSeekBeginZero)
                {
                    stream.Seek(0, SeekOrigin.Begin);
                }

                return str;
            }
        }
    }
}