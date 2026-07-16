using ProudNetSrc.Serialization;

namespace Santana.Network.Message.Event
{

  public class EventMessageFactory : MessageFactory
  {
    public EventMessageFactory()
    {
      foreach (var (op, type) in Packet.TypesFor("game"))
        RegisterType(type, op);
    }
  }
}
