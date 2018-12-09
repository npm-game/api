using Microsoft.AspNetCore.Builder;

namespace NPMGame.API.Extensions.Api
{
    public static class HubMappingExtensions
    {
        public static void UseHubMappings(this IApplicationBuilder builder)
        {
            builder.UseSignalR(routes =>
            {
                //routes.MapHub<ChatHub>("/chatHub");
            });
        }
    }
}