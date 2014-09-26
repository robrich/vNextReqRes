using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.AspNet;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Http;
using Microsoft.Framework.ConfigurationModel;

namespace KWebStartup
{
	public class Startup
	{
		public void Configure(IBuilder app)
		{
			var config = new Configuration();
			config.AddJsonFile("project.json");

			app.Use(async (context, next) => {
				// Write request details
				// https://github.com/aspnet/HttpAbstractions/blob/master/src/Microsoft.AspNet.Http/HttpRequest.cs
				// https://github.com/aspnet/HttpAbstractions/blob/master/src/Microsoft.AspNet.PipelineCore/DefaultHttpRequest.cs
				Console.WriteLine("REQUEST:");
				Console.WriteLine("----------------------------");
				Console.WriteLine("URL:");
				Console.WriteLine(context.Request.Method);
				Console.WriteLine(context.Request.Scheme);
				Console.WriteLine(context.Request.Host);
				Console.WriteLine(context.Request.PathBase);
				Console.WriteLine(context.Request.Path);
				Console.WriteLine(context.Request.QueryString);
				Console.WriteLine("HEADERS:");
				foreach (var h in context.Request.Headers) {
					Console.WriteLine(h.Key+"=");
					foreach (var v in h.Value) {
						Console.WriteLine("  "+v);
					}
				}
				// FRAGILE: consumes the stream, could rebuild it using http://stackoverflow.com/questions/21805362/rewind-request-body-stream/21805602#21805602
				StreamReader reader = new StreamReader(context.Request.Body, Encoding.UTF8);
				Console.WriteLine("BODY:");
				Console.WriteLine(reader.ReadToEnd());

				// Handoff to the next middleware
				await next();
			});
			app.Run(async context => {
				// Do something
				context.Response.ContentType = "text/plain";
				context.Response.Headers.Set("stuff","value");
				await context.Response.WriteAsync("This is the response body");

				// Write response details
				// https://github.com/aspnet/HttpAbstractions/blob/master/src/Microsoft.AspNet.Http/HttpResponse.cs
				// https://github.com/aspnet/HttpAbstractions/blob/master/src/Microsoft.AspNet.PipelineCore/DefaultHttpResponse.cs
				Console.WriteLine("RESPONSE");
				Console.WriteLine("----------------------------");
				Console.WriteLine(context.Response.StatusCode);
				Console.WriteLine("HEADERS:");
				foreach (var h in context.Response.Headers) {
					Console.WriteLine(h.Key+"=");
					foreach (var v in h.Value) {
						Console.WriteLine("  "+v);
					}
				}
				// Don't consume console.Response.Stream -- let the browser get it
				Console.WriteLine("============================");
			});

			Console.WriteLine("Running on port 2000 as listed in project.json");
			Console.WriteLine(" "+config.Get("commands:web"));
		}
	}
}
