using Librarian.Porter.Configs;
using Librarian.Porter.Constants;
using Librarian.ThirdParty.Contracts;
using Librarian.ThirdParty.Steam;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Librarian.Porter.Services
{
    public class AppInfoServiceResolver
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly PorterConfig _porterConfig;
        public AppInfoServiceResolver(IServiceProvider serviceProvider, PorterConfig porterConfig)
        {
            _serviceProvider = serviceProvider;
            _porterConfig = porterConfig;
        }

        public IAppInfoService GetService(string source)
        {
            if (!IsSourceEnabled(source))
            {
                throw new InvalidOperationException("The source is not enabled.");
            }
            return source switch
            {
                WellKnownAppInfoSource.Steam => 
                    (IAppInfoService)_serviceProvider.GetRequiredService(typeof(SteamAPIService)),
                WellKnownAppInfoSource.Bangumi =>
                    (IAppInfoService)_serviceProvider.GetRequiredService(typeof(SteamAPIService)),
                WellKnownAppInfoSource.Vndb =>
                    (IAppInfoService)_serviceProvider.GetRequiredService(typeof(SteamAPIService)),
                _ => throw new ArgumentException("Unsupported app info source.")
            };
        }

        private bool IsSourceEnabled(string source)
        {
            return source switch
            {
                WellKnownAppInfoSource.Steam => _porterConfig.IsSteamEnabled,
                WellKnownAppInfoSource.Bangumi => _porterConfig.IsBangumiEnabled,
                WellKnownAppInfoSource.Vndb => _porterConfig.IsVndbEnabled,
                _ => false
            };
        }
    }
}
