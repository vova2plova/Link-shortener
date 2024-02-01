namespace LinkShortener.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GoController(ILogger<GoController> logger, LinkContext context) : Controller
    {
        [HttpGet("{Suffix}")]
        public async Task<IActionResult> GoToLongLink(string Suffix)
        {
            if (string.IsNullOrWhiteSpace(Suffix))
            {
                return BadRequest("Suffix is required");
            }

            try
            {
                var link = await context.Links
                    .AsNoTracking()
                    .FirstOrDefaultAsync(l => Suffix.Equals(l.Suffix));
                if (link == null)
                {
                    return NotFound($"{Suffix} is not exist");
                }
                return Redirect(link.FullLink);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error while finding in db");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
