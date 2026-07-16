using SantanaLib.Threading.Tasks;

namespace ProudNetSrc.Handlers
{
  using System;
  using System.Threading;
  using System.Threading.Tasks;
  using DotNetty.Transport.Channels;
  using ProudNetSrc.Serialization;
  using ProudNetSrc.Serialization.Messages.Core;

  internal class SessionHandler : ChannelHandlerAdapter
  {
    private readonly ProudServer _server;

    public SessionHandler(ProudServer server)
    {
      _server = server;
    }

    public override void ChannelActive(IChannelHandlerContext context)
    {
      var hostId = _server.Configuration.HostIdFactory.New();
      var session = _server.Configuration.SessionFactory.Create(hostId, context.Channel, _server);
      context.Channel.GetAttribute(ChannelAttributes.Session).Set(session);

      var log = _server.Configuration.Logger?
          .ForContext("HostId", hostId)
          .ForContext("EndPoint", context.Channel.RemoteAddress.ToString());
      log?.Debug("Accepted a socket, assigned id {HostId}, remote side {EndPoint}");

      var config = new NetConfigDto
      {
        EnableServerLog = _server.Configuration.EnableServerLog,
        FallbackMethod = _server.Configuration.FallbackMethod,
        MessageMaxLength = _server.Configuration.MessageMaxLength,
        TimeoutTimeMs = _server.Configuration.IdleTimeout.TotalMilliseconds,
        DirectP2PStartCondition = _server.Configuration.DirectP2PStartCondition,
        OverSendSuspectingThresholdInBytes = _server.Configuration.OverSendSuspectingThresholdInBytes,
        EnableNagleAlgorithm = _server.Configuration.EnableNagleAlgorithm,
        EncryptedMessageKeyLength = _server.Configuration.EncryptedMessageKeyLength,
        AllowServerAsP2PGroupMember = _server.Configuration.AllowServerAsP2PGroupMember,
        EnableP2PEncryptedMessaging = _server.Configuration.EnableP2PEncryptedMessaging,
        UpnpDetectNatDevice = _server.Configuration.UpnpDetectNatDevice,
        UpnpTcpAddrPortMapping = _server.Configuration.UpnpTcpAddrPortMapping,
        EnablePingTest = _server.Configuration.EnablePingTest,
        EmergencyLogLineCount = _server.Configuration.EmergencyLogLineCount
      };
      session.SendAsync(new NotifyServerConnectionHintMessage(config, _server.Rsa.ExportParameters(false)));

      var cts = new CancellationTokenSource(_server.Configuration.ConnectTimeout);
      session.HandhsakeEvent.WaitAsync(cts.Token).ContinueWith(t =>
      {
        cts.Dispose();
        if (!t.IsCanceled || !session.IsConnected)
          return;

        log?.Debug("Peer {HostId} never finished negotiating in time, dropping it");
        session.SendAsync(new ConnectServerTimedoutMessage());
        session.CloseAsync();
      }, TaskScheduler.Default);

      base.ChannelActive(context);
    }

    public override void ChannelInactive(IChannelHandlerContext context)
    {
      var session = context.Channel.GetAttribute(ChannelAttributes.Session).Get();
      if (session == null)
      {
        base.ChannelInactive(context);
        return;
      }

      _server.Configuration.Logger?
          .ForContext("HostId", session.HostId)
          .ForContext("EndPoint", context.Channel.RemoteAddress.ToString())
          .Debug("Client({HostId}) disconnected");

      session.P2PGroup?.Leave(session.HostId);
      session.Dispose();
      _server.RemoveSession(session);
      _server.Configuration.HostIdFactory.Free(session.HostId);
       base.ChannelInactive(context);
    }
  }
}
