using Marten;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using NPMGame.API.Models.Config;

namespace NPMGame.API.Extensions
{
    public static class HubMappingExtensions
    {
        public static void UseHubMappings(this IApplicationBuilder builder)
        {
            builder.UseSignalR(routes =>
            {
                routes.MapHub<ChatHub>("/chatHub");
            });
        }
    }
}