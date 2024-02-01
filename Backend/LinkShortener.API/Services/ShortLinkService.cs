using HashidsNet;

namespace LinkShortener.API.Services
{
    public class ShortLinkService(ILogger<ShortLinkService> logger, LinkContext context) : IShortLinkService
    {
        public async Task<string> CreateShortLink(string fullLink)
        {
            var hashids = new Hashids("salt", 6);

            var random = new Random();
            var number = random.Next();

            var suffix = hashids.Encode(number);
            try
            {

                var existingLink = await context.Links
                    .AsNoTracking()
                    .FirstOrDefaultAsync(l => l.Suffix == suffix);
                if (existingLink != null)
                {
                    return await CreateShortLink(fullLink);
                }
            }catch (Exception ex)
            {
                logger.LogError("Error while finding suffix in db");
                throw new InternalServerException("Internal server error", ex);
            }

            return suffix;
        }

    }
}
