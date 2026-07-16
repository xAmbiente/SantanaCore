using System.IO;

namespace Santana.Resource
{
  internal static class ResourceGuard
  {
    public const int MaxEntries = 65536;
    public const int MaxEntryBytes = 4096;
    public const int MaxSceneChunks = 65536;
    public const int MaxCollectionCount = 1048576;
    public const int MaxDecompressedBytes = 64 * 1024 * 1024;

    public static int EnsureCount(int count, string source, int max = MaxCollectionCount)
    {
      if (count < 0)
        throw new InvalidDataException($"{source}: negative count {count}");

      if (count > max)
        throw new InvalidDataException($"{source}: count {count} exceeds max {max}");

      return count;
    }

    public static uint EnsureCount(uint count, string source, uint max = MaxCollectionCount)
    {
      if (count > max)
        throw new InvalidDataException($"{source}: count {count} exceeds max {max}");

      return count;
    }

    public static int EnsureByteCount(int count, string source, int max)
    {
      if (count < 0)
        throw new InvalidDataException($"{source}: negative byte count {count}");

      if (count > max)
        throw new InvalidDataException($"{source}: byte count {count} exceeds max {max}");

      return count;
    }
  }
}
