namespace LinkShortener.API.Model
{
    public class Link : ISoftDelete
    {
        public Guid Id { get; set; }
        public required string Suffix { get; set; }
        public required string FullLink { get; set; }
        public DateTimeOffset ExpirationDate { get; set; }

        public bool IsDeleted { get; set; }
        public DateTimeOffset? DeletedAt { get; set; }
        public void Undo()
        {
            IsDeleted = false;
            DeletedAt = null;
        }
    }
}
