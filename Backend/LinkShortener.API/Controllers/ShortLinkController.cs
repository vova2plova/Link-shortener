namespace LinkShortener.API.Controllers
{
    [ApiController]
    [Route("[Controller]")]
    public class ShortLinkController(LinkContext context, ILogger<ShortLinkController> logger, IShortLinkService shortLinkService) : Controller
    {
        [HttpPost]
        public async Task<IActionResult> CreateShortLink(
            [FromBody] string fullLink,
            string timeToLiveUnit = "DAYS",
            int timeToLive = 1)
        {
            if (string.IsNullOrWhiteSpace(fullLink))
            {
                return BadRequest("Full link is required");
            }

            TimeSpan duration;

            switch (timeToLiveUnit)
            {
                case "SECONDS":
                    duration = TimeSpan.FromSeconds(timeToLive);
                    break;
                case "MINUTES":
                    duration = TimeSpan.FromMinutes(timeToLive);
                    break;
                case "HOURS":
                    duration = TimeSpan.FromHours(timeToLive);
                    break;
                case "DAYS":
                    duration = TimeSpan.FromDays(timeToLive);
                    break;
                default:
                    return BadRequest("Time unit doesn't exist, use one of the approved (SECONDS | MINUTES | HOURS | DAYS)");
            }

            try
            {
                var existingFullLink = await context
                    .Links
                    .AsNoTracking()
                    .IgnoreQueryFilters()
                    .FirstOrDefaultAsync(context => fullLink.Equals(context.FullLink));
                if (existingFullLink != null)
                {
                    //Если срок жизни ссылки истёк
                    if (existingFullLink.ExpirationDate < DateTimeOffset.Now)
                    {
                        existingFullLink.ExpirationDate = DateTimeOffset.Now.Add(duration);
                        await context.SaveChangesAsync();
                    }
                    //Если ссылка была удалена
                    if (existingFullLink.IsDeleted)
                    {
                        existingFullLink.IsDeleted = false;
                        await context.SaveChangesAsync();
                    }
                    return Ok(existingFullLink.Suffix);
                }

                var link = new Link
                {
                    Id = default,
                    Suffix = await shortLinkService.CreateShortLink(fullLink),
                    FullLink = fullLink,
                    ExpirationDate = DateTimeOffset.Now.Add(duration)
                };

                //Удалить протухшую или помеченную на удаление ссылку с таким же суффиксом
                var existingSuffix = await context
                    .Links
                    .IgnoreQueryFilters()
                    .FirstOrDefaultAsync(l => link.Suffix.Equals(l.Suffix));

                if (existingSuffix != null)
                {
                    context
                        .Links
                        .Remove(existingSuffix);
                }

                await context.Links.AddAsync(link);
                await context.SaveChangesAsync();
                return Ok(link.Suffix);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error saving short link to db");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}