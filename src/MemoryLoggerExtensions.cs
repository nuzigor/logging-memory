using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;

namespace Nuzigor.Extensions.Logging.Memory
{
    public static class MemoryLoggerExtensions
    {
        /// <summary>
        /// Adds a memory logger called 'Memory' to the factory.
        /// </summary>
        /// <param name="builder"> The <see cref="ILoggingBuilder"/> to use.</param>
        public static ILoggingBuilder AddMemory(this ILoggingBuilder builder)
        {
            builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<ILoggerProvider, MemoryLoggerProvider>());
            builder.Services.TryAddSingleton<IMemoryLoggerSink, MemoryLoggerSink>();
            return builder;
        }
    }
}
