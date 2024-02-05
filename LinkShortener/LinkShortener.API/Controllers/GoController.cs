namespace LinkShortener.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GoController(IDbContextFactory<LinkContext> contextFactory) : Controller
    {
        [HttpGet("{Suffix}")]
        public async Task<IActionResult> GoToLongLink(
            string Suffix,
            CancellationToken cancellationToken)
        {
            using var context = await contextFactory.CreateDbContextAsync(cancellationToken);
            if (string.IsNullOrWhiteSpace(Suffix))
            {
                return BadRequest("Suffix is required");
            }

            var link = await context.Links
                .AsNoTracking()
                .FirstOrDefaultAsync(l => Suffix.Equals(l.Suffix));
            if (link == null)
            {
                return NotFound($"{Suffix} is not exist");
            }
            return Redirect(link.FullLink);
        }
    }
}
