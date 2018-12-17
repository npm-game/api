using Microsoft.AspNetCore.Builder;
using NPMGame.API.Hubs;

namespace NPMGame.API.Extensions.Api
{
    public static class HubMappingExtensions
    {
        public static void UseHubMappings(this IApplicationBuilder builder)
        {
            builder.UseSignalR(routes =>
            {
                routes.MapHub<GameSessionHub>("/games");
            });
        }
    }
}