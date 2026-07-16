using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Santana.Network.Data.Auth;
using Santana.Network.Message.Auth;
using Serilog;
using Serilog.Core;

namespace Santana
{
  internal class ServerManager : IEnumerable<ServerInfoDto>
  {
    private static readonly ILogger Logger =
        Log.ForContext(Constants.SourceContextPropertyName, nameof(ServerManager));

    internal readonly ConcurrentDictionary<ushort, ServerEntry> ServerList =
        new ConcurrentDictionary<ushort, ServerEntry>();

    public IEnumerator<ServerInfoDto> GetEnumerator()
    {
      foreach (var entry in ServerList.Values)
        yield return entry.Game;

      foreach (var entry in ServerList.Values)
        yield return entry.Chat;
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return GetEnumerator();
    }

    public ServerInfoDto[] GetServers()
    {
      var result = new List<ServerInfoDto>();
      uint nextId = 0;
      foreach (var info in this)
      {
        info.Id = nextId;
        nextId++;
        result.Add(info);
      }
      return result.ToArray();
    }

    public byte Add(AuthServer.ServiceModel.ServerInfoDto request)
    {
      if (request == null || request.ApiKey == null || request.ApiKey != Config.Instance.API.ApiKey)
      {
        Logger.Information("Turning away a node announcement, the presented shared secret is absent or does not match");
        return 2;
      }

      if (request.EndPoint == null || request.ChatEndPoint == null || string.IsNullOrWhiteSpace(request.Name))
      {
        Logger.Information("Turning away node {id}, its descriptor lacks an address pair or a usable label", request.Id);
        return 2;
      }

      var chatInfo = new ServerInfoDto
      {
        IsEnabled = true,
        Id = request.Id,
        GroupId = request.Id,
        Type = ServerType.Chat,
        Name = "S4_Chat",
        PlayerLimit = request.PlayerLimit,
        PlayerOnline = request.PlayerOnline,
        EndPoint = request.ChatEndPoint
      };

      var gameInfo = new ServerInfoDto
      {
        IsEnabled = true,
        Id = request.Id,
        GroupId = request.Id,
        Type = ServerType.Game,
        Name = request.Name,
        PlayerLimit = request.PlayerLimit,
        PlayerOnline = request.PlayerOnline,
        EndPoint = request.EndPoint
      };

      if (!ServerList.TryAdd(request.Id, new ServerEntry(gameInfo, chatInfo)))
        return 1;

      Logger.Information($"Node {request.Name} (id {request.Id}) cleared the secret check and now appears in the published list");
      Network.AuthServer.Instance.Broadcast(new ServerListAckMessage(GetServers()));
      return 0;
    }

    public bool Update(AuthServer.ServiceModel.ServerInfoDto request)
    {
      if (request == null || request.ApiKey != Config.Instance.API.ApiKey)
        return false;

      if (!ServerList.TryGetValue(request.Id, out var entry))
        return false;

      entry.Game.PlayerLimit = request.PlayerLimit;
      entry.Game.PlayerOnline = request.PlayerOnline;

      entry.Chat.PlayerLimit = request.PlayerLimit;
      entry.Chat.PlayerOnline = request.PlayerOnline;

      entry.LastUpdate = DateTimeOffset.Now;

      return true;
    }

    public void Flush()
    {
      foreach (var pair in ServerList)
      {
        var elapsed = DateTimeOffset.Now - pair.Value.LastUpdate;
        if (elapsed >= Config.Instance.API.Timeout)
          Remove(pair.Key);
      }
    }

    public bool Remove(ushort id)
    {
      if (!ServerList.TryRemove(id, out var entry))
        return false;

      Logger.Information($"Dropped node {entry.Game.Name} (group {entry.Game.GroupId}) from the published list");
      Network.AuthServer.Instance.Broadcast(
          new ServerListAckMessage(Network.AuthServer.Instance.ServerManager.ToArray()));
      return true;
    }

    internal class ServerEntry
    {
      public ServerEntry(ServerInfoDto game, ServerInfoDto chat)
      {
        Game = game;
        Chat = chat;
        LastUpdate = DateTimeOffset.Now;
      }

      public ServerInfoDto Game { get; set; }
      public ServerInfoDto Chat { get; set; }
      public DateTimeOffset LastUpdate { get; set; }
    }
  }
}
