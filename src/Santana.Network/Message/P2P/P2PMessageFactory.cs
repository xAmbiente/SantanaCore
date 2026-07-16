using ProudNetSrc.Serialization;

namespace Santana.Network.Message.P2P
{

  public class P2PMessageFactory : MessageFactory
  {
    public P2PMessageFactory()
    {
      foreach (var (op, type) in Packet.TypesFor("p2p"))
        RegisterType(type, op);

      RegisterType(typeof(DamageInfoMessage), 5);
      RegisterType(typeof(DamageRemoteInfoMessage), 6);
    }
  }
}
