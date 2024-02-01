namespace LinkShortener.API.Extension
{
    public static class HostApplicationBuilderExtension
    {
        public static void AddApplicationServices(this IHostApplicationBuilder builder)
        {
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddDbContext<LinkContext>(options =>
            {
                options.UseInMemoryDatabase("LinkDb");
            });

            builder.Services.AddScoped<IShortLinkService, ShortLinkService>();

        }
    }
}
