using System.Net;
using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit;

namespace NarrowIntegrationTests
{
    public static class ResponseAssertions
    {
        public static void Assert409Conflict(IActionResult actionResult)
        {
            var httpCode = GetHttpStatusCode(actionResult);
            var conflictObject = actionResult is ConflictObjectResult;
            var conflictResult = actionResult is ConflictResult;
            Assert.True(conflictObject || conflictResult,
                $"Expected Conflict (409) but was {httpCode} ({(int)httpCode})");
        }

        public static void Assert400BadRequest(IActionResult actionResult)
        {
            var httpCode = GetHttpStatusCode(actionResult);
            var badRequestObject = actionResult is BadRequestObjectResult;
            var badRequest = actionResult is BadRequestResult;
            Assert.True(badRequestObject || badRequest,
                $"Expected BadRequest (400) but was {httpCode} ({(int)httpCode})");
        }

        public static void Assert404NotFound(IActionResult actionResult)
        {
            var httpCode = GetHttpStatusCode(actionResult);
            var notFoundObject = actionResult is NotFoundObjectResult;
            var notFound = actionResult is NotFoundResult;
            Assert.True(notFoundObject || notFound,
                $"Expected NotFound (404) but was {httpCode} ({(int)httpCode})");
        }

        public static void AssertSuccessStatusCode(IActionResult actionResult)
        {
            if (actionResult is FileStreamResult fileStreamResult)
            {
                Assert.NotNull(fileStreamResult.FileStream);
                Assert.NotNull(fileStreamResult.FileDownloadName);
                Assert.NotEmpty(fileStreamResult.FileDownloadName);
                Assert.Equal(MediaTypeNames.Application.Pdf, fileStreamResult.ContentType);
            }
            else
            {
                var httpCode = GetHttpStatusCode(actionResult);
                Assert.True(new HttpResponseMessage(httpCode).IsSuccessStatusCode,
                    $"Expected SuccessStatusCode (2xx) but was {httpCode} ({(int)httpCode})");
            }
        }

        private static HttpStatusCode GetHttpStatusCode(IActionResult functionResult)
        {
            try
            {
                return (HttpStatusCode)functionResult
                    .GetType()
                    .GetProperty("StatusCode")!
                    .GetValue(functionResult, null)!;
            }
            catch
            {
                return HttpStatusCode.InternalServerError;
            }
        }

        public static T GetContentOfType<T>(this IActionResult result)
        {
            return (T)((ObjectResult)result).Value;
        }

        public static T DeserialiseContentStreamOfType<T>(this IActionResult result,
            JsonSerializerSettings settings = default)
        {
            var stream = (Stream)((ObjectResult)result).Value;
            Assert.NotNull(stream);
            using var streamReader = new StreamReader(stream);
            using var jsonTextReader = new JsonTextReader(streamReader);
            var loaded = JToken.Load(jsonTextReader);
            var content = loaded.ToObject<T>(JsonSerializer.Create(settings));
            Assert.NotNull(content);
            return content;
        }
    }
}