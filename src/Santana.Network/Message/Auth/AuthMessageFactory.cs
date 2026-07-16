using ProudNetSrc.Serialization;

namespace Santana.Network.Message.Auth
{

  public class AuthMessageFactory : MessageFactory
  {
    public AuthMessageFactory()
    {
      foreach (var (op, type) in Packet.TypesFor("auth"))
        RegisterType(type, op);
    }
  }
}
