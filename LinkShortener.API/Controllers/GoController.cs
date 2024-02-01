namespace LinkShortener.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class GoController(ILogger<GoController> logger, LinkContext context) : Controller
    {
        [HttpGet("{Suffix}")]
        public async Task<IActionResult> GoToLongLink(string Suffix)
        {
            if (string.IsNullOrEmpty(Suffix))
            {
                return BadRequest("Suffix is required");
            }

            try
            {
                var link = await context.Links.FirstOrDefaultAsync(l => l.Suffix == Suffix);
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
