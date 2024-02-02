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
                //если суффикс уже существует и не "протух", генерируем новый
                var existingSuffix = await context.Links
                    .AsNoTracking()
                    .FirstOrDefaultAsync(l => suffix.Equals(l.Suffix) && l.ExpirationDate > DateTimeOffset.Now);
                if (existingSuffix != null)
                {
                    return await CreateShortLink(fullLink);
                }
            }catch (Exception ex)
            {
                logger.LogError("Error while finding suffix in db");
                throw new InternalServerException("An error occurred while searching for the suffix in the database. Please check the database connection and try again", ex);
            }

            return suffix;
        }

    }
}
