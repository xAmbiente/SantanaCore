using SantanaLib.Serialization;

namespace ProudNetSrc.Serialization
{
  [SantanaContract]
  internal class NetConfigDto
  {
    [SantanaMember(0)] public bool EnableServerLog { get; set; }

    [SantanaMember(1)] public FallbackMethod FallbackMethod { get; set; }

    [SantanaMember(2)] public uint MessageMaxLength { get; set; }

    [SantanaMember(3)] public double TimeoutTimeMs { get; set; }

    [SantanaMember(4)] public DirectP2PStartCondition DirectP2PStartCondition { get; set; }

    [SantanaMember(5)] public uint OverSendSuspectingThresholdInBytes { get; set; }

    [SantanaMember(6)] public bool EnableNagleAlgorithm { get; set; }

    [SantanaMember(7)] public int EncryptedMessageKeyLength { get; set; }

    [SantanaMember(8)] public uint FastEncryptedMessageKeyLength { get; set; }

    [SantanaMember(9)] public bool AllowServerAsP2PGroupMember { get; set; }

    [SantanaMember(10)] public bool EnableP2PEncryptedMessaging { get; set; }

    [SantanaMember(11)] public bool UpnpDetectNatDevice { get; set; }

    [SantanaMember(12)] public bool UpnpTcpAddrPortMapping { get; set; }

    [SantanaMember(13)] public uint EmergencyLogLineCount { get; set; }

    [SantanaMember(14)] public bool EnableLookaheadP2PSend { get; set; }

    [SantanaMember(15)] public bool EnablePingTest { get; set; }
  }
}
