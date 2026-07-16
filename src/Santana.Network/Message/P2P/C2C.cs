using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Runtime.CompilerServices;
using SantanaLib.IO;
using SantanaLib.Serialization;
using Santana.Network.Data.P2P;
using ProudNetSrc.Serialization;
namespace Santana.Network.Message.P2P
{
  [Packet(1, PacketType.P2P)]
  public class PlayerSpawnReqMessage
  {
    public PlayerSpawnReqMessage()
    {
      Character = new CharacterDto();
    }
    public PlayerSpawnReqMessage(CharacterDto character)
    {
      Character = character;
    }
    public CharacterDto Character { get; set; }
  }
  [Packet(2, PacketType.P2P)]
  public class PlayerSpawnAckMessage
  {
    public PlayerSpawnAckMessage()
    {
      Character = new CharacterDto();
    }
    public PlayerSpawnAckMessage(CharacterDto character)
    {
      Character = character;
    }
    public CharacterDto Character { get; set; }
  }
  [Packet(3, PacketType.P2P)]
  public class AbilitySyncMessage
  {
    public AbilitySyncMessage()
    {
      Values = Array.Empty<ValueDto>();
    }
    [Compressed] public float Unk { get; set; }
    public ValueDto[] Values { get; set; }
  }
  [Packet(4, PacketType.P2P)]
  public class EquippingItemSyncMessage
  {
    public EquippingItemSyncMessage()
    {
      Costumes = Array.Empty<ItemDto>();
      Skills = Array.Empty<ItemDto>();
      Weapons = Array.Empty<ItemDto>();
      Values = Array.Empty<ItemDto>();
    }
    public ItemDto[] Costumes { get; set; }
    public ItemDto[] Skills { get; set; }
    public ItemDto[] Weapons { get; set; }
    public ItemDto[] Values { get; set; }
  }
  [SantanaContract(typeof(Serializer))]
  public class DamageInfoMessage
  {
    public DamageInfoMessage()
    {
      Target = 0;
      Source = 0;
      Position = Vector3.Zero;
      Rotation = Vector2.Zero;
      Unk5 = 18;
      Unk6 = 22.0625f;
      Flag1 = 2;
      Flag2 = 2;
    }
    public PeerId Target { get; set; }
    public AttackAttribute AttackAttribute { get; set; }
    public uint GameTime { get; set; }
    public PeerId Source { get; set; }
    public byte Unk5 { get; set; }
    public Vector2 Rotation { get; set; }
    public Vector3 Position { get; set; }
    public float Unk6 { get; set; }
    public float Damage { get; set; }
    public short Unk8 { get; set; }
    public ushort Unk9 { get; set; }
    public byte Flag1 { get; set; }
    public byte Flag2 { get; set; }
    public byte Flag3 { get; set; }
    public byte Flag4 { get; set; }
    public byte Flag5 { get; set; }
    public byte Flag6 { get; set; }
    public byte Flag7 { get; set; }
    public byte IsCritical { get; set; }
    public byte Flag9 { get; set; }
    internal class Serializer : ISerializer<DamageInfoMessage>
    {
      public bool CanHandle(Type type)
      {
        return type == typeof(DamageInfoMessage);
      }
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public void Serialize(BinaryWriter writer, DamageInfoMessage value)
      {
        writer.Write(value.Target);
        writer.WriteEnum(value.AttackAttribute);
        writer.Write(value.GameTime);
        writer.Write(value.Source);
        writer.Write(value.Unk5);
        writer.WriteRotation(value.Rotation);
        writer.WriteCompressed(value.Position);
        writer.WriteCompressed(value.Unk6);
        writer.WriteCompressed(value.Damage);
        writer.Write(value.Unk8);
        writer.Write(value.Unk9);
        var ls = new List<byte>();
        var bw = new BitStreamWriter(ls);
        bw.Write(value.Flag1, 3);
        bw.Write(value.Flag2, 2);
        bw.Write(value.Flag3, 1);
        bw.Write(value.Flag4, 1);
        bw.Write(value.Flag5, 1);
        bw.Write(value.Flag6, 1);
        bw.Write(value.Flag7, 7);
        bw.Write(value.IsCritical, 4);
        bw.Write(value.Flag9, 4);
        writer.Write(ls);
      }
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public DamageInfoMessage Deserialize(BinaryReader reader)
      {
        var message = new DamageInfoMessage();
        message.Target = reader.ReadUInt16();
        message.AttackAttribute = reader.ReadEnum<AttackAttribute>();
        message.GameTime = reader.ReadUInt32();
        message.Source = reader.ReadUInt16();
        message.Unk5 = reader.ReadByte();
        message.Rotation = reader.ReadRotation();
        message.Position = reader.ReadCompressedVector3();
        message.Unk6 = reader.ReadCompressedFloat();
        message.Damage = reader.ReadCompressedFloat();
        message.Unk8 = reader.ReadInt16();
        message.Unk9 = reader.ReadUInt16();
        var br = new BitStreamReader(reader.ReadBytes(3));
        message.Flag1 = br.ReadByte(3);
        message.Flag2 = br.ReadByte(2);
        message.Flag3 = br.ReadByte(1);
        message.Flag4 = br.ReadByte(1);
        message.Flag5 = br.ReadByte(1);
        message.Flag6 = br.ReadByte(1);
        message.Flag7 = br.ReadByte(7);
        message.IsCritical = br.ReadByte(4);
        message.Flag9 = br.ReadByte(4);
        return message;
      }
    }
  }
  [SantanaContract(typeof(Serializer))]
  public class DamageRemoteInfoMessage
  {
    public DamageRemoteInfoMessage()
    {
      Target = 0;
      Source = 0;
      Position = Vector3.Zero;
      Rotation = Vector2.Zero;
    }
    public PeerId Target { get; set; }
    public AttackAttribute AttackAttribute { get; set; }
    public uint GameTime { get; set; }
    public PeerId Source { get; set; }
    public Vector2 Rotation { get; set; }
    public Vector3 Position { get; set; }
    public float Unk { get; set; }
    public float Damage { get; set; }
    public byte Flag1 { get; set; }
    public byte Flag2 { get; set; }
    public byte Flag3 { get; set; }
    public byte Flag4 { get; set; }
    public byte Flag5 { get; set; }
    public byte Flag6 { get; set; }
    public byte Flag7 { get; set; }
    internal class Serializer : ISerializer<DamageRemoteInfoMessage>
    {
      public bool CanHandle(Type type)
      {
        return type == typeof(DamageRemoteInfoMessage);
      }
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public void Serialize(BinaryWriter writer, DamageRemoteInfoMessage value)
      {
        writer.Write(value.Target);
        writer.WriteEnum(value.AttackAttribute);
        writer.Write(value.GameTime);
        writer.Write(value.Source);
        writer.WriteRotation(value.Rotation);
        writer.WriteCompressed(value.Position);
        writer.WriteCompressed(value.Unk);
        writer.WriteCompressed(value.Damage);
        var ls = new List<byte>();
        var bw = new BitStreamWriter(ls);
        bw.Write(value.Flag1, 2);
        bw.Write(value.Flag2, 1);
        bw.Write(value.Flag3, 1);
        bw.Write(value.Flag4, 1);
        bw.Write(value.Flag5, 3);
        bw.Write(value.Flag6, 4);
        bw.Write(value.Flag7, 4);
        writer.Write(ls);
      }
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      public DamageRemoteInfoMessage Deserialize(BinaryReader reader)
      {
        var message = new DamageRemoteInfoMessage();
        message.Target = reader.ReadUInt16();
        message.AttackAttribute = reader.ReadEnum<AttackAttribute>();
        message.GameTime = reader.ReadUInt32();
        message.Source = reader.ReadUInt16();
        message.Rotation = reader.ReadRotation();
        message.Position = reader.ReadCompressedVector3();
        message.Unk = reader.ReadCompressedFloat();
        message.Damage = reader.ReadCompressedFloat();
        var br = new BitStreamReader(reader.ReadBytes(2));
        message.Flag1 = br.ReadByte(2);
        message.Flag2 = br.ReadByte(1);
        message.Flag3 = br.ReadByte(1);
        message.Flag4 = br.ReadByte(1);
        message.Flag5 = br.ReadByte(3);
        message.Flag6 = br.ReadByte(4);
        message.Flag7 = br.ReadByte(4);
        return message;
      }
    }
  }
  [Packet(7, PacketType.P2P)]
  public class SnapShotMessage
  {
    public SnapShotMessage()
    {
      Position = Vector3.Zero;
      Rotation = Vector2.Zero;
    }
    public SnapShotMessage(uint time, Vector3 position, Vector2 rotation, byte unk)
    {
      Time = time;
      Unk = unk;
      Position = position;
      Rotation = rotation;
    }
    public uint Time { get; set; }
    public byte Unk { get; set; }
    public Vector3 Position { get; set; }
    public Vector2 Rotation { get; set; }
  }
  [Packet(8, PacketType.P2P)]
  public class StateSyncMessage
  {
    public StateSyncMessage()
    {
    }
    public StateSyncMessage(ActorState state)
    {
      State = state;
    }
    public StateSyncMessage(ActorState state, uint gameTime, int value, byte currentWeapon)
    {
      State = state;
      GameTime = gameTime;
      Value = value;
      CurrentWeapon = currentWeapon;
    }
    public uint GameTime { get; set; }
    public int Value { get; set; }
    public ActorState State { get; set; }
    public byte CurrentWeapon { get; set; }
  }
  [Packet(10, PacketType.P2P)]
  public class BGEffectMessage
  {
    public BGEffectMessage()
    {
      Position = Vector3.Zero;
    }
    public int Unk1 { get; set; }
    public byte Unk2 { get; set; }
    public Vector3 Position { get; set; }
    public byte Unk3 { get; set; }
    public byte Unk4 { get; set; }
    public byte Unk5 { get; set; }
    public short Unk6 { get; set; }
    public byte Unk7 { get; set; }
    public byte Unk8 { get; set; }
  }
  [Packet(11, PacketType.P2P)]
  public class DefensivePowerMessage
  {
    public DefensivePowerMessage()
    {
      PeerId = 0;
    }
    public PeerId PeerId { get; set; }
    [Compressed] public float Value { get; set; }
  }
  [Packet(12, PacketType.P2P)]
  public class BlastObjectDestroyMessage
  {
    public BlastObjectDestroyMessage()
    {
      Player = 0;
      Unk = Array.Empty<int>();
    }
    public PeerId Player { get; set; }
    public int[] Unk { get; set; }
  }
  [Packet(13, PacketType.P2P)]
  public class BlastObjectRespawnMessage
  {
    public BlastObjectRespawnMessage()
    {
      Unk = Array.Empty<int>();
    }
    public int[] Unk { get; set; }
  }
  [Packet(15, PacketType.P2P)]
  public class MindEnergyMessage
  {
    public MindEnergyMessage()
    {
      Target = 0;
    }
    public byte Unk1 { get; set; }
    public PeerId Target { get; set; }
    public short Unk3 { get; set; }
    [Compressed] public float Unk4 { get; set; }
    [Compressed] public float Unk5 { get; set; }
    public byte Unk6 { get; set; }
  }
  [Packet(16, PacketType.P2P)]
  public class DamageShieldMessage
  {
    [Compressed] public float Unk { get; set; }
  }
  [Packet(17, PacketType.P2P)]
  public class AimedPointMessage
  {
    public AimedPointMessage()
    {
      Unk1 = Vector3.Zero;
      Unk2 = Vector3.Zero;
    }
    public Vector3 Unk1 { get; set; }
    public Vector3 Unk2 { get; set; }
  }
  [Packet(18, PacketType.P2P)]
  public class OnOffMessage
  {
    public byte Action { get; set; }
    public bool IsEnabled { get; set; }
    public byte Value { get; set; }
  }
  [Packet(19, PacketType.P2P)]
  public class SentryGunSpawnMessage
  {
    public LongPeerId Id { get; set; }
    public float Unk2 { get; set; }
    public float Unk3 { get; set; }
    public float Unk4 { get; set; }
    public Vector2 Rotation { get; set; }
    public byte Unk5 { get; set; }
    public int Unk6 { get; set; }
    [Compressed] public float Unk7 { get; set; }
    [Compressed] public float Unk8 { get; set; }
    [Compressed] public float Unk9 { get; set; }
  }
  [Packet(20, PacketType.P2P)]
  public class SentryGunStateMessage
  {
    public PeerId Id { get; set; }
    public byte Unk1 { get; set; }
    public PeerId Unk2 { get; set; }
  }
  [Packet(21, PacketType.P2P)]
  public class SentryGunDestructionMessage
  {
    public PeerId Id { get; set; }
  }
  [Packet(22, PacketType.P2P)]
  public class SentryGunDestruction2Message
  {
    public int Unk1 { get; set; }
    public int Unk2 { get; set; }
  }
  [Packet(23, PacketType.P2P)]
  public class GrenadeSpawnMessage
  {
    public GrenadeSpawnMessage()
    {
      Id = 0;
      Owner = 0;
      Position = Vector3.Zero;
      Unk4 = Vector3.Zero;
      Unk7 = "";
    }
    public PeerId Id { get; set; }
    public PeerId Owner { get; set; }
    public Vector3 Position { get; set; }
    public Vector3 Unk4 { get; set; }
    [Compressed] public float Unk5 { get; set; }
    [Compressed] public float Unk6 { get; set; }
    public string Unk7 { get; set; }
  }
  [Packet(24, PacketType.P2P)]
  public class GrenadeSnapShotMessage
  {
    public GrenadeSnapShotMessage()
    {
      Id = 0;
      Position = Vector3.Zero;
      Unk2 = Vector3.Zero;
    }
    public PeerId Id { get; set; }
    public Vector3 Position { get; set; }
    public Vector3 Unk2 { get; set; }
    public byte Unk3 { get; set; }
  }
  [Packet(25, PacketType.P2P)]
  public class GrenadeSnapShot2Message
  {
    public GrenadeSnapShot2Message()
    {
      Id = 0;
      Position = Vector3.Zero;
    }
    public PeerId Id { get; set; }
    public Vector3 Position { get; set; }
  }
  [Packet(26, PacketType.P2P)]
  public class ObstructionSpawnMessage
  {
    public ObstructionSpawnMessage()
    {
      Owner = 0;
      Id = 0;
      Position = Vector3.Zero;
      Rotation = Vector2.Zero;
    }
    public PeerId Owner { get; set; }
    public PeerId Id { get; set; }
    public Vector3 Position { get; set; }
    public Vector2 Rotation { get; set; }
    public int Unk2 { get; set; }
    public int Unk3 { get; set; }
    public byte Unk4 { get; set; }
  }
  [Packet(27, PacketType.P2P)]
  public class ObstructionDestroyMessage
  {
    public ObstructionDestroyMessage()
    {
    }
    public ObstructionDestroyMessage(PeerId id)
    {
      Id = id;
    }
    public PeerId Id { get; set; }
  }
  [Packet(28, PacketType.P2P)]
  public class ObstructionDamageMessage
  {
    public PeerId Id { get; set; }
    public uint Damage { get; set; }
  }
  [Packet(29, PacketType.P2P)]
  public class SyncObjectObstructionMessage
  {
    public SyncObjectObstructionMessage()
    {
      Owner = 0;
      Id = 0;
      Position = Vector3.Zero;
      Rotation = Vector2.Zero;
    }
    public SyncObjectObstructionMessage(PeerId owner, PeerId id, uint gameTime, Vector3 position, Vector2 rotation,
        uint count, uint hp, uint time)
    {
      Owner = owner;
      Id = id;
      GameTime = gameTime;
      Position = position;
      Rotation = rotation;
      Count = count;
      HP = hp;
      Time = time;
    }
    public PeerId Owner { get; set; }
    public PeerId Id { get; set; }
    public uint GameTime { get; set; }
    public Vector3 Position { get; set; }
    public Vector2 Rotation { get; set; }
    public uint Count { get; set; }
    public uint HP { get; set; }
    public uint Time { get; set; }
  }
  [Packet(30, PacketType.P2P)]
  public class BlastObjectSyncMessage
  {
    public BlastObjectSyncMessage()
    {
      Unk = Array.Empty<int>();
    }
    public int[] Unk { get; set; }
  }
  [Packet(31, PacketType.P2P)]
  public class BallSyncMessage
  {
    public BallSyncMessage()
    {
      Position = Vector3.Zero;
      Unk = Vector3.Zero;
    }
    public BallSyncMessage(PeerId player, Vector3 position)
    {
      Player = player;
      Position = position;
    }
    public PeerId Player { get; set; }
    public Vector3 Position { get; set; }
    public Vector3 Unk { get; set; }
  }
  [Packet(32, PacketType.P2P)]
  public class BallSnapShotMessage
  {
    public BallSnapShotMessage()
    {
      Position = Vector3.Zero;
      Unk = Vector3.Zero;
    }
    public BallSnapShotMessage(Vector3 position)
    {
      Position = position;
      Unk = position;
    }
    public Vector3 Position { get; set; }
    public Vector3 Unk { get; set; }
  }
  [Packet(34, PacketType.P2P)]
  public class ArcadeFinMessage
  {
    public byte Unk { get; set; }
  }
  [Packet(36, PacketType.P2P)]
  public class AttachArcadeItemMessage
  {
    public PeerId Id { get; set; }
    public int Unk { get; set; }
  }
  [Packet(37, PacketType.P2P)]
  public class HPSyncMessage
  {
    public HPSyncMessage()
    {
    }
    public HPSyncMessage(float value, float max)
    {
      Value = value;
      Max = max;
    }
    [Compressed] public float Value { get; set; }
    [Compressed] public float Max { get; set; }
  }
  [Packet(38, PacketType.P2P)]
  public class Unk38Message
  {
    public PeerId Unk1 { get; set; }
    public PeerId Unk2 { get; set; }
    public int Unk3 { get; set; }
    [Compressed] public float Unk4 { get; set; }
  }
  [Packet(39, PacketType.P2P)]
  public class ExposeClubMarkMessage
  {
    public byte Unk1 { get; set; }
    public byte Unk2 { get; set; }
  }
  [Packet(40, PacketType.P2P)]
  public class ReflectRateMessage
  {
    public PeerId Unk1 { get; set; }
    [Compressed] public float Unk2 { get; set; }
  }
  [Packet(41, PacketType.P2P)]
  public class ConditionInfoMessage
  {
    public ConditionInfoMessage()
    {
      Data = Array.Empty<byte>();
    }
    public ConditionInfoMessage(PeerId target, Condition condition, byte[] data)
    {
      Unused = new PeerId(0, 11, PlayerCategory.Unused);
      Target = target;
      Condition = condition;
      Data = data;
    }
    public PeerId Unused { get; set; }
    public PeerId Target { get; set; }
    public Condition Condition { get; set; }
    [Scalar] public byte[] Data { get; set; }
  }
  [Packet(42, PacketType.P2P)]
  public class AbilityChangeSyncMessage
  {
    public int Unk1 { get; set; }
    [Compressed] public float Unk2 { get; set; }
    [Compressed] public float HP { get; set; }
    [Compressed] public float Unk3 { get; set; }
    [Compressed] public float MP { get; set; }
    [Compressed] public float Unk4 { get; set; }
  }
  [Packet(43, PacketType.P2P)]
  public class HealHPMessage
  {
    public PeerId Unk1 { get; set; }
    [Compressed] public float Unk2 { get; set; }
  }
}
