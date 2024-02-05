namespace LinkShortener.API.Controllers
{
    [ApiController]
    [Route("[Controller]")]
    public class ShortLinkController(
        IDbContextFactory<LinkContext> contextFactory,
        IShortLinkService shortLinkService,
        IConfiguration configuration) : Controller
    {
        [HttpPost]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesErrorResponseType(typeof(ErrorDto))]
        public async Task<IActionResult> CreateShortLink(
            [FromBody] string fullLink,
            CancellationToken cancellationToken)
        {
            var _baseGoUrl = configuration.GetValue<string>("baseGoControllerUrl");
            using var context = await contextFactory.CreateDbContextAsync(cancellationToken);
            if (string.IsNullOrWhiteSpace(fullLink))
            {
                return BadRequest("Full link is required");
            }

            var existingFullLink = await context
                .Links
                .AsNoTracking()
                .FirstOrDefaultAsync(context => fullLink.Equals(context.FullLink), cancellationToken);
                if (existingFullLink != null)
                {
                    return Ok($"{_baseGoUrl}/{existingFullLink.Suffix}");
                }

            var link = new Link
            {
                Id = default,
                Suffix = await shortLinkService.CreateShortLink(fullLink),
                FullLink = fullLink,
            };

            await context.Links.AddAsync(link, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
            return Ok($"{_baseGoUrl}/{link.Suffix}");
        }
    }
}