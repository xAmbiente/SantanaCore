using SantanaLib.Serialization;
using Santana.Network.Serializers;
using ProudNetSrc.Serialization;

namespace Santana.Network.Message.Relay
{

  public class RelayMessageFactory : MessageFactory
  {
    static RelayMessageFactory()
    {
      Serializer.AddCompiler(new PeerIdSerializer());
    }

    public RelayMessageFactory()
    {
      foreach (var (op, type) in Packet.TypesFor("relay"))
        RegisterType(type, op);
    }
  }
}
