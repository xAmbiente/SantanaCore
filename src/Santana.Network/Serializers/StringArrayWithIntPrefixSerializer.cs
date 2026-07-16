using ProudNetSrc.Serialization.Serializers;

namespace Santana.Network.Serializers
{
    public class StringArrayWithIntPrefixSerializer : ArrayWithIntPrefixSerializer
    {
        public StringArrayWithIntPrefixSerializer()
            : base(typeof(StringSerializer))
        {
        }
    }
}
