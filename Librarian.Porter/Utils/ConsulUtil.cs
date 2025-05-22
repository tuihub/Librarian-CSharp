using Consul;
using Librarian.Porter.Models;

namespace Librarian.Porter.Utils
{
    public static class ConsulUtil
    {
        public static void RegisterConsul(IConsulClient consulClient, GlobalContext globalContext)
        {
            var consulConfig = globalContext.ConsulConfig;
            var instanceContext = globalContext.InstanceContext;
            var registration = new AgentServiceRegistration()
            {
                ID = instanceContext.ConsulRegId.ToString(),
                Name = consulConfig.ServiceName,
                Address = consulConfig.ServiceAddress,
                Port = consulConfig.ServicePort,
                Tags = instanceContext.SupportedAppInfoSources.ToArray(),
                Check = new AgentServiceCheck
                {
                    HTTP = consulConfig.HealthCheckUrl,
                    Interval = TimeSpan.FromSeconds(10),
                    Timeout = TimeSpan.FromSeconds(5)
                }
            };
            consulClient.Agent.ServiceRegister(registration).Wait();
        }
        public static void DeregisterConsul(IConsulClient consulClient, GlobalContext globalContext)
        {
            consulClient.Agent.ServiceDeregister(globalContext.InstanceContext.ConsulRegId.ToString()).Wait();
        }
    }
}
