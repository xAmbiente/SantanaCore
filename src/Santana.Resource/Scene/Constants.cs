using System;

namespace Santana.Resource.Scene
{
  [Flags]
  public enum RenderState
  {
    None = 0,
    NoLight = 1,
    AlphaBlend1 = 2,
    NoCull = 8,
    AlphaBlend2 = 32,
    NoDepthWrite = 64,
    NoFog = 512,
    NoMipmap = 2048,
    Shadow = 8192
  }

  public enum ChunkType : uint
  {
    Box = 0x25ADF0D1,
    ModelData = 0x081098F8,
    Bone = 0x6D411AD1,
    SkyDirect1 = 0xC3E8BE62,
    BoneSystem = 0x5E74333F,
    Shape = 0xADEE38A2
  }
}
