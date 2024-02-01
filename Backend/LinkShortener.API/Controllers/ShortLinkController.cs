using System.ComponentModel;

namespace LinkShortener.API.Controllers
{
	[ApiController]
	[Route("[Controller]")]
	public class ShortLinkController(LinkContext context, ILogger<ShortLinkController> logger, IShortLinkService shortLinkService) : Controller
	{
		private const string defaultTimeToLiveUnit = "DAYS";
		private const int defaultTimeToLive = 1;

		[HttpPost]
		public async Task<IActionResult> CreateShortLink(
			[FromBody] string fullLink,
			[FromBody] string? timeToLiveUnit,
			[FromBody] int? timeToLive)
		{
			if (string.IsNullOrWhiteSpace(fullLink))
			{
				return BadRequest("Full link is required");
			}

            TimeSpan? duration;

            if (string.IsNullOrWhiteSpace(timeToLiveUnit))
			{
				timeToLiveUnit = defaultTimeToLiveUnit;
			}

			if (timeToLive == null)
			{
				timeToLive = defaultTimeToLive;
			}

			switch (timeToLiveUnit)
			{
				case "SECONDS": 
					duration = new TimeSpan(0, 0, 0, defaultTimeToLive); 
					break;
				case "MINUTES":
					duration = new TimeSpan(0, 0, defaultTimeToLive);
					break;
				case "HOURS":
					duration = new TimeSpan(0, defaultTimeToLive, 0);
					break;
				case "DAYS":
					duration = new TimeSpan(defaultTimeToLive, 0, 0);
					break;
				default:
					return BadRequest("Time unit doesn't exist, use one of the approved (SECONDS | MINUTES | HOURS | DAYS)");
			}

			try
			{
				var existingFullLink = await context.Links
					.AsNoTracking()
					.FirstOrDefaultAsync(context => fullLink.Equals(context.FullLink));
				if (existingFullLink != null)
				{
					return Ok(existingFullLink.Suffix);
				}

				var link = new Link
				{
					Id = default,
					Suffix = await shortLinkService.CreateShortLink(fullLink),
					FullLink = fullLink,
					ExpirationDate = DateTimeOffset.Now.Add(duration)
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