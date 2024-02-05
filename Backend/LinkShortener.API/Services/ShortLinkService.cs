using HashidsNet;

namespace LinkShortener.API.Services
{
    public class ShortLinkService(LinkContext context) : IShortLinkService
    {
        public async Task<string> CreateShortLink(string fullLink)
        {
            var hashids = new Hashids("salt", 6);

            var random = new Random();
            var number = random.Next();

            var suffix = hashids.Encode(number);
            //Если суффикс существует генерируем новый
            var existingSuffix = await context.Links
                .AsNoTracking()
                .FirstOrDefaultAsync(l => suffix.Equals(l.Suffix));
            if (existingSuffix != null)
            {
                return await CreateShortLink(fullLink);
            }

            return suffix;
        }

    }
}
