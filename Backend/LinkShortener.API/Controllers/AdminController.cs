namespace LinkShortener.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AdminController(ILogger<AdminController> logger, LinkContext context) : Controller
    {
        [HttpGet("Links")]
        public async Task<List<Link>> GetLinks([FromBody]string secretkey)
        {
            try
            {
                return await context
                .Links
                .IgnoreQueryFilters()
                .AsNoTracking()
                .ToListAsync();
                
            }
            catch (Exception ex)
            {
                logger.LogError("Error while getiing links");
                throw new InternalServerException("An error occurred while getting from the database. Please check the database connection and try again", ex);
            }
        }

        [HttpPut("Links")]
        public async Task<IActionResult> RestoreSoftDeletedLinks([FromBody]string secretKey, Guid id)
        {
            try
            {
                var existingLink = await context
                    .Links
                    .IgnoreQueryFilters()
                    .FirstOrDefaultAsync(l => l.Id == id);
                if (existingLink == null)
                {
                    logger.LogError($"Link with id = {id} doesn't exist");
                    return NotFound($"Link with id = {id} Not found");
                }
                if (existingLink.IsDeleted == false)
                {
                    logger.LogError($"Link with id = {id} isn't deleted");
                    return BadRequest($"Link with id = {id} isn't deleted");
                }

                existingLink.Undo();
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                logger.LogError("Error while restoring link");
                throw new InternalServerException("An error occurred while getting from the database. Please check the database connection and try again", ex);
            }
            return Ok($"Restored link id = {id}");
        }

        [HttpDelete("Links")]
        public async Task<IActionResult> DeleteLinks([FromBody]string secretKey, Guid id, bool IsHard = false)
        {
            try
            {
                var existingLink = await context
                    .Links
                    .IgnoreQueryFilters()
                    .FirstOrDefaultAsync(l => l.Id == id);
                if (existingLink == null)
                {
                    logger.LogError($"link with id = {id} doesn't exist");
                    return NotFound($"Link with id = {id} Not found");
                }

                if (IsHard)
                {
                    context.Links.Remove(existingLink);
                }
                else
                {
                    existingLink.IsDeleted = true;
                    existingLink.DeletedAt = DateTimeOffset.Now;
                }

                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                logger.LogError("Error while deleting link");
                throw new InternalServerException("An error occurred while getting from the database. Please check the database connection and try again", ex);
            }

            return Ok($"Deleted link id = {id}");
        }
    }
}
