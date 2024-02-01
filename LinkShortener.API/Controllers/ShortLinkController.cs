namespace LinkShortener.API.Controllers
{
    [ApiController]
    [Route("[Controller]")]
    public class ShortLinkController(LinkContext context, ILogger<ShortLinkController> logger, IShortLinkService shortLinkService) : Controller
    {
        [HttpPost]
        public async Task<IActionResult> CreateShortLink([FromBody]string fullLink)
        {
            if (string.IsNullOrWhiteSpace(fullLink))
            {
                return BadRequest("Full link is required");
            }  
           
            try
            {
                var existingFullLink = await context.Links.FirstOrDefaultAsync(context => context.FullLink == fullLink);
                if (existingFullLink != null)
                    return Ok(existingFullLink.Suffix);

                var link = new Link
                {
                    Id = default,
                    Suffix = await shortLinkService.CreateShortLink(fullLink),
                    FullLink = fullLink,
                    ExpirationDate = DateTime.Today.AddDays(7)
                };

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
