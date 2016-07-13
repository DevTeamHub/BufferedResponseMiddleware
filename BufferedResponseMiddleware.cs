using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Owin;

namespace DevTeam.BufferedResponseMiddleware
{
    public class BufferedResponseMiddleware
    {
        private readonly Func<IDictionary<string, object>, Task> _next;

        public BufferedResponseMiddleware(Func<IDictionary<string, object>, Task> next)
        {
            _next = next;
        }

        public virtual async Task Invoke(IDictionary<string, object> environment)
        {
            var context = new OwinContext(environment);

            var originalStream = context.Response.Body;
            var bufferedResponse = new MemoryStream();
            context.Response.Body = bufferedResponse;

            await _next(environment);

            bufferedResponse.Seek(0, SeekOrigin.Begin);
            await bufferedResponse.CopyToAsync(originalStream);
        }
    }
}
