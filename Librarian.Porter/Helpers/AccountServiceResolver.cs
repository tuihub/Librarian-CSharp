using Librarian.ThirdParty.Contracts;

namespace Librarian.Porter.Helpers
{
    public class AccountServiceResolver
    {
        private readonly Dictionary<string, IAccountService> _services;

        public AccountServiceResolver(IEnumerable<IAccountService> services)
        {
            _services = new Dictionary<string, IAccountService>();
            foreach (var service in services)
            {
                var type = service.GetType();
                if (type.Namespace == null) continue;

                var platform = type.Namespace.Split('.').Last();

                if (platform == "Steam")
                {
                    _services["steam"] = service;
                }
            }
        }

        public IAccountService? GetAccountService(string platform)
        {
            return _services.TryGetValue(platform, out var service) ? service : null;
        }
    }
}