using System.IO;

namespace SantanaLib.IO
{
    public interface IManualSerializer
    {
        void Serialize(Stream stream);
        void Deserialize(Stream stream);
    }
}
