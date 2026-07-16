using ProudNetSrc.Serialization;

namespace Santana.Network.Message.GameRule
{

    public class GameRuleMessageFactory : MessageFactory
    {
        public GameRuleMessageFactory()
        {
            foreach (var (op, type) in Packet.TypesFor("gamerule"))
                RegisterType(type, op);
        }
    }
}
