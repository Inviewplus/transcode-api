using Microsoft.Extensions.DependencyInjection;
using Transcoder.Infrastructure.OciClient;
using Transcoder.Infrastructure.TranscoderService;

namespace Transcoder.Infrastructure
{
    public static class InjectionDependency
    {
        public static void AddInjection(this IServiceCollection selectionService)
        {
            selectionService.AddTransient<OracleProvider>();
            selectionService.AddTransient<IOracleClient, OracleClient>();
            selectionService.AddTransient<ITranscoder, TranscoderClient>();
        }
    }
}
