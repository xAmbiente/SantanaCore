using SantanaLib.Serialization;
using Santana.Network.Serializers;
using ProudNetSrc.Serialization;

namespace Santana.Network.Message.Chat
{

  public class ChatMessageFactory : MessageFactory
  {
    static ChatMessageFactory()
    {
      Serializer.AddCompiler(new ItemNumberSerializer());
    }

    public ChatMessageFactory()
    {
      foreach (var (op, type) in Packet.TypesFor("chat"))
        RegisterType(type, op);
    }
  }
}
