using Consul;
using Librarian.Porter.Configs;

namespace Librarian.Porter.Utils
{
    public static class Consultil
    {
        public static void RegisterConsul(IConsulClient consulClient, ConsulConfig consulConfig)
        {
            var registration = new AgentServiceRegistration()
            {
                ID = StaticContext.ConsulRegId.ToString(),
                Name = consulConfig.ServiceName,
                Address = consulConfig.ServiceAddress,
                Port = consulConfig.ServicePort,
                Tags = StaticContext.PorterTags.ToArray(),
                Check = new AgentServiceCheck
                {
                    HTTP = consulConfig.HealthCheckUrl,
                    Interval = TimeSpan.FromSeconds(10),
                    Timeout = TimeSpan.FromSeconds(5)
                }
            };
            consulClient.Agent.ServiceRegister(registration).Wait();
        }
        public static void DeregisterConsul(IConsulClient consulClient)
        {
            consulClient.Agent.ServiceDeregister(StaticContext.ConsulRegId.ToString()).Wait();
        }
    }
}
