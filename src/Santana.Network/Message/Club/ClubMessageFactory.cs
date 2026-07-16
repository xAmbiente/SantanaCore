using SantanaLib.Serialization;
using Santana.Network.Serializers;
using ProudNetSrc.Serialization;

namespace Santana.Network.Message.Club
{

    public class ClubMessageFactory : MessageFactory
    {
        static ClubMessageFactory()
        {
            Serializer.AddCompiler(new ItemNumberSerializer());
        }

        public ClubMessageFactory()
        {
            foreach (var (op, type) in Packet.TypesFor("club"))
                RegisterType(type, op);
        }
    }
}
