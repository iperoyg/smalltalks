using Microsoft.Extensions.DependencyInjection;
using Serilog;
using SmallTalks.Core.Services;
using SmallTalks.Core.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace SmallTalks.Core
{
    public class ContainerProvider
    {
        public static IServiceCollection RegisterTypes(IServiceCollection serviceCollection)
        {
            serviceCollection
                .AddSingleton<ISmallTalksDetector, SmallTalksDetector>()
                .AddSingleton<IFileService, FileService>()
                .AddSingleton<IConversionService, ModelConversionService>()
                .AddSingleton<IDetectorDataProviderService, DetectorDataProviderService>()
                .AddSingleton<ISourceProviderService, LocalSourceProviderService>()
                .AddSingleton<IWordDetectorFactory, WordDetectorFactory>()
                .AddSingleton<ILogger>((new LoggerConfiguration()).CreateLogger())
                .AddTransient<IWordsDetector, WordsDetectorBase>();

                
            
            return serviceCollection;
        }
    }
}
