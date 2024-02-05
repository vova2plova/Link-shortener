namespace LinkShortener.API.Services
{
    public interface IShortLinkService
    {
        public Task<string> CreateShortLink(string fullLink);
    }
}