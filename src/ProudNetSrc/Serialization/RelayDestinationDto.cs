using SantanaLib.Serialization;

namespace ProudNetSrc.Serialization
{
  [SantanaContract]
  internal class RelayDestinationDto
  {
    public RelayDestinationDto()
    {
    }

    public RelayDestinationDto(uint hostId, uint frameNumber)
    {
      HostId = hostId;
      FrameNumber = frameNumber;
    }

    [SantanaMember(0)] public uint HostId { get; set; }

    [SantanaMember(1)] public uint FrameNumber { get; set; }
  }
}
