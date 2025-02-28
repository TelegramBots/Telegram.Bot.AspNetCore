using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.DependencyInjection;
using System.Text;
using System.Text.Json;
using Telegram.Bot.Requests.Abstractions;
using Telegram.Bot.Types;

namespace Telegram.Bot.AspNetCore
{
	/// <summary>Helpers for WebApp service configuration</summary>
	public static class Extensions
	{
		/// <summary>Configure ASP.NET MVC Json (de)serialization for Telegram.Bot types</summary>
		/// <param name="services">The IServiceCollection to add the services to.</param>
		public static IServiceCollection ConfigureTelegramBotMvc(this IServiceCollection services)
			=> services.Configure<MvcOptions>(options =>
			{
				options.InputFormatters.Insert(0, _inputFormatter);
				options.OutputFormatters.Insert(0, _outputFormatter);
			});

		private static readonly TelegramBotInputFormatter _inputFormatter = new();
		private static readonly TelegramBotOutputFormatter _outputFormatter = new();

		private class TelegramBotInputFormatter : TextInputFormatter
		{
			public TelegramBotInputFormatter()
			{
				SupportedEncodings.Add(Encoding.UTF8);
				SupportedMediaTypes.Add("application/json");
			}

			protected override bool CanReadType(Type type) => type == typeof(Update);

			public sealed override async Task<InputFormatterResult> ReadRequestBodyAsync(InputFormatterContext context, Encoding encoding)
			{
				var model = await JsonSerializer.DeserializeAsync(context.HttpContext.Request.Body, context.ModelType, JsonBotAPI.Options, context.HttpContext.RequestAborted);
				return await InputFormatterResult.SuccessAsync(model);
			}
		}

		private class TelegramBotOutputFormatter : TextOutputFormatter
		{
			public TelegramBotOutputFormatter()
			{
				SupportedEncodings.Add(Encoding.UTF8);
				SupportedMediaTypes.Add("application/json");
			}

			protected override bool CanWriteType(Type? type) => typeof(IRequest).IsAssignableFrom(type);

			public sealed override async Task WriteResponseBodyAsync(OutputFormatterWriteContext context, Encoding selectedEncoding)
			{
				var stream = context.HttpContext.Response.Body;
				await JsonSerializer.SerializeAsync(stream, context.Object, JsonBotAPI.Options, context.HttpContext.RequestAborted);
			}
		}
	}
}
