using Bamboo.Configuration;

namespace Chameleon.Faas.Management.Infrastructure.Configs
{
    public class ConnectionStrings : GitConfigBase<ConnectionStrings>
    {
        public string ChameleonFaas { get; set; }
    }
}
