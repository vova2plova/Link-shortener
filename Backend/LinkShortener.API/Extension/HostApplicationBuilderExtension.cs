using LinkShortener.API.Interceptors;

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
                options
                .UseInMemoryDatabase("TestLinkDb")
                .AddInterceptors(new SoftDeleteInterceptor());
            });

            builder.Services.AddScoped<IShortLinkService, ShortLinkService>();
        }
    }
}
