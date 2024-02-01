namespace LinkShortener.API.Model
{
	public class Link
	{
		public Guid Id { get; set; }
		public required string Suffix { get; set; }
		public required string FullLink { get; set; }
		public DateTime ExpirationDate { get; set; }
	}
}
