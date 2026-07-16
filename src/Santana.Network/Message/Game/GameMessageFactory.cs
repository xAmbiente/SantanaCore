using SantanaLib.Serialization;
using Santana.Network.Serializers;
using ProudNetSrc.Serialization;

namespace Santana.Network.Message.Game
{

    public class GameMessageFactory : MessageFactory
    {
        static GameMessageFactory()
        {
            Serializer.AddCompiler(new MatchKeySerializer());
            Serializer.AddCompiler(new LongPeerIdSerializer());
            Serializer.AddCompiler(new CharacterStyleSerializer());
        }

        public GameMessageFactory()
        {
            foreach (var (op, type) in Packet.TypesFor("game"))
                RegisterType(type, op);
        }
    }
}
