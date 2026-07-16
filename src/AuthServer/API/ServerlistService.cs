using System;
using System.Threading.Tasks;
using AuthServer.ServiceModel;
using SantanaLib.DotNetty.SimpleRmi;

namespace Santana.API
{
    internal class ServerlistService : RmiService, IServerlistService
    {
        public async Task<RegisterResult> Register(ServerInfoDto serverInfo)
        {
            var channelState = CurrentContext.Channel.GetAttribute(ChannelAttributes.State).Get();
            channelState.LastActivity = DateTimeOffset.Now;

            if (serverInfo == null)
            {
                Console.WriteLine($"[NodeRegistry] enrollment aborted, descriptor payload was absent");
                return RegisterResult.WrongKey;
            }
            if (serverInfo.ApiKey != Config.Instance.API.ApiKey)
            {
                Console.WriteLine($"[NodeRegistry] enrollment aborted for node {serverInfo.Id}, credential token did not match");
                return RegisterResult.WrongKey;
            }
            if (string.IsNullOrWhiteSpace(serverInfo.Name) || serverInfo.Name.Length > 32)
            {
                Console.WriteLine($"[NodeRegistry] enrollment aborted for node {serverInfo.Id}, label empty or beyond 32 chars");
                return RegisterResult.WrongKey;
            }
            if (serverInfo.EndPoint == null || serverInfo.ChatEndPoint == null)
            {
                Console.WriteLine($"[NodeRegistry] enrollment aborted for node {serverInfo.Id}, game or chat address unset");
                return RegisterResult.WrongKey;
            }
            if (serverInfo.PlayerOnline > serverInfo.PlayerLimit)
            {
                Console.WriteLine($"[NodeRegistry] enrollment aborted for node {serverInfo.Id}, reported population exceeds its own capacity");
                return RegisterResult.WrongKey;
            }

            var outcome = (RegisterResult)Network.AuthServer.Instance.ServerManager.Add(serverInfo);
            if (outcome == RegisterResult.OK)
                channelState.ServerId = serverInfo.Id;

            return outcome;
        }

        public async Task<bool> Update(ServerInfoDto serverInfo)
        {
            var channelState = CurrentContext.Channel.GetAttribute(ChannelAttributes.State).Get();
            channelState.LastActivity = DateTimeOffset.Now;

            if (serverInfo == null)
            {
                Console.WriteLine($"[NodeRegistry] refresh aborted, descriptor payload was absent");
                return false;
            }
            if (serverInfo.ApiKey != Config.Instance.API.ApiKey)
            {
                Console.WriteLine($"[NodeRegistry] refresh aborted for node {serverInfo.Id}, credential token did not match");
                return false;
            }
            if (string.IsNullOrWhiteSpace(serverInfo.Name) || serverInfo.Name.Length > 32)
            {
                Console.WriteLine($"[NodeRegistry] refresh aborted for node {serverInfo.Id}, label empty or beyond 32 chars");
                return false;
            }
            if (serverInfo.EndPoint == null || serverInfo.ChatEndPoint == null)
            {
                Console.WriteLine($"[NodeRegistry] refresh aborted for node {serverInfo.Id}, game or chat address unset");
                return false;
            }
            if (serverInfo.PlayerOnline > serverInfo.PlayerLimit)
            {
                Console.WriteLine($"[NodeRegistry] refresh aborted for node {serverInfo.Id}, reported population exceeds its own capacity");
                return false;
            }

            if (channelState.ServerId != serverInfo.Id)
            {
                Console.WriteLine($"[NodeRegistry] refresh aborted, this link is bound to node {channelState.ServerId} yet the payload claims {serverInfo?.Id}");
                return false;
            }

            return Network.AuthServer.Instance.ServerManager.Update(serverInfo);
        }

        public async Task<bool> Remove(byte id)
        {
            var channelState = CurrentContext.Channel.GetAttribute(ChannelAttributes.State).Get();
            if (channelState.ServerId != id)
            {
                Console.WriteLine($"[NodeRegistry] withdrawal aborted, this link is bound to node {channelState.ServerId} yet the payload claims {id}");
                return false;
            }

            return Network.AuthServer.Instance.ServerManager.Remove(id);
        }
    }
}
