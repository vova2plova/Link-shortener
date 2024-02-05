using System.Collections;

namespace LinkShortener.API.Model
{
    public class ErrorDto
    {
        public required string ErrorMessage { get; init; }
        public IDictionary? Data { get; init; }
    }
}
