namespace Librarian.Sephirah.Configs
{
    public partial class ConsulConfig
    {
        public bool IsEnabled { get; set; }
        public string ConsulAddress { get; set; } = null!;

        public void SetConfig(ConsulConfig config)
        {
            IsEnabled = config.IsEnabled;
            ConsulAddress = config.ConsulAddress;
        }
    }
}
