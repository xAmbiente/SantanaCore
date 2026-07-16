namespace ProudNetSrc
{
  using System;

  public static class SecurityGuard
  {
    public const int MaxArrayElements = 4096;
    public const int MaxStringChars = 8192;

    public const int MaxDecompressedBytes = 16 * 1024 * 1024;

    public static void EnsureArrayLength(int length, string source)
    {
      if (length < 0)
        throw new ProudException($"{source}: negative array length {length}");

      if (length > MaxArrayElements)
        throw new ProudException($"{source}: array length {length} exceeds max {MaxArrayElements}");
    }

    public static void EnsureDecompressedLength(long totalBytes, string source)
    {
      if (totalBytes > MaxDecompressedBytes)
        throw new ProudException($"{source}: decompressed size {totalBytes} exceeds max {MaxDecompressedBytes} (zip-bomb?)");
    }

    public static string EnsureString(string value, string source)
    {
      if (value == null)
        value = string.Empty;

      if (value.Length > MaxStringChars)
        throw new ProudException($"{source}: string length {value.Length} exceeds max {MaxStringChars}");

      return value;
    }
  }
}
