using Healthcare.Domain.Exceptions;
using System.Net;
using System.Text.Json;

namespace Healthcare.Presentation.Middlewares
{
	public class GlobalExceptionMiddleware
	{
		private readonly RequestDelegate _next;
		private readonly ILogger<GlobalExceptionMiddleware> _logger;

		public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
		{
			_next = next;
			_logger = logger;
		}

		public async Task InvokeAsync(HttpContext context)
		{
			try
			{
				await _next(context);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "An unhandled exception occurred: {Message}", ex.Message);
				await HandleExceptionAsync(context, ex);
			}
		}

		private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
		{
			var isJsonRequest = context.Request.Headers["Accept"].ToString().Contains("application/json") ||
								context.Request.Headers["X-Requested-With"] == "XMLHttpRequest";

			var statusCode = HttpStatusCode.InternalServerError;
			var message = "An error occurred while processing your request.";
			object? errors = null;

			switch (exception)
			{
				case NotFoundException notFoundEx:
					statusCode = HttpStatusCode.NotFound;
					message = notFoundEx.Message;
					break;

				case ValidationException validationEx:
					statusCode = HttpStatusCode.BadRequest;
					message = validationEx.Message;
					errors = validationEx.Errors;
					break;

				case BusinessException businessEx:
					statusCode = HttpStatusCode.BadRequest;
					message = businessEx.Message;
					break;

				case DomainException domainEx:
					statusCode = HttpStatusCode.BadRequest;
					message = domainEx.Message;
					break;

				case UnauthorizedAccessException:
					statusCode = HttpStatusCode.Unauthorized;
					message = "You are not authorized to perform this action.";
					break;
			}

			context.Response.StatusCode = (int)statusCode;

			if (isJsonRequest)
			{
				context.Response.ContentType = "application/json";
				var response = new
				{
					status = (int)statusCode,
					message,
					errors
				};

				await context.Response.WriteAsync(JsonSerializer.Serialize(response));
			}
			else
			{
				context.Response.ContentType = "text/html";
				await context.Response.WriteAsync($"<!DOCTYPE html><html><head><title>Error {(int)statusCode}</title><style>body {{ font-family: sans-serif; margin: 40px; }} .card {{ border: 1px solid #ddd; padding: 20px; border-radius: 8px; max-width: 500px; }} h2 {{ color: #e53e3e; }}</style></head><body><div class='card'><h2>Error {(int)statusCode}</h2><p>{WebUtility.HtmlEncode(message)}</p><a href='/'>Return to Home</a></div></body></html>");
			}
		}
	}
}
