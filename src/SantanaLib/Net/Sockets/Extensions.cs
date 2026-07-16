using SantanaLib.Threading.Tasks;
using System.Net;
using System;
using System.Net.Sockets;
﻿using System.Threading.Tasks;

namespace SantanaLib.Net.Sockets
{
    public static class SocketExtensions
    {
        public static Task ConnectTaskAsync(this Socket @this, IPEndPoint endPoint)
        {
            return Task.Factory.FromAsync(@this.BeginConnect, @this.EndConnect, endPoint, null);
        }

        public static Task<Socket> AcceptTaskAsync(this Socket @this)
        {
            return Task<Socket>.Factory.FromAsync(@this.BeginAccept, @this.EndAccept, null);
        }

        public static Task<int> SendTaskAsync(this Socket @this, byte[] buffer, int offset, int size, SocketFlags socketFlags)
        {
            return Task<int>.Factory.FromAsync(@this.BeginSend, @this.EndSend, buffer, offset, size, socketFlags);
        }

        public static Task<int> SendTaskAsync(this Socket @this, byte[] buffer, int size, SocketFlags socketFlags)
        {
            return @this.SendTaskAsync(buffer, 0, size, socketFlags);
        }

        public static Task<int> SendTaskAsync(this Socket @this, byte[] buffer, SocketFlags socketFlags)
        {
            return @this.SendTaskAsync(buffer, 0, buffer.Length, socketFlags);
        }

        public static Task<int> SendTaskAsync(this Socket @this, byte[] buffer)
        {
            return @this.SendTaskAsync(buffer, 0, buffer.Length, SocketFlags.None);
        }

        public static Task<int> ReceiveTaskAsync(this Socket @this, byte[] buffer, int offset, int size, SocketFlags socketFlags)
        {
            return Task<int>.Factory.FromAsync(@this.BeginReceive, @this.EndReceive, buffer, offset, size, socketFlags);
        }

        public static Task<int> ReceiveTaskAsync(this Socket @this, byte[] buffer, int size, SocketFlags socketFlags)
        {
            return @this.ReceiveTaskAsync(buffer, 0, size, socketFlags);
        }

        public static Task<int> ReceiveTaskAsync(this Socket @this, byte[] buffer, SocketFlags socketFlags)
        {
            return @this.ReceiveTaskAsync(buffer, 0, buffer.Length, socketFlags);
        }

        public static Task<int> ReceiveTaskAsync(this Socket @this, byte[] buffer)
        {
            return @this.ReceiveTaskAsync(buffer, 0, buffer.Length, SocketFlags.None);
        }

        public static Task<int> SendToTaskAsync(this Socket @this, byte[] buffer, int offset, int size, SocketFlags socketFlags, EndPoint remoteEP)
        {
            return Task<int>.Factory.FromAsync(@this.BeginSendTo, @this.EndSendTo, buffer, offset, size, socketFlags, remoteEP);
        }

        public static Task<int> SendToTaskAsync(this Socket @this, byte[] buffer, int size, SocketFlags socketFlags, EndPoint remoteEP)
        {
            return @this.SendToTaskAsync(buffer, 0, size, socketFlags, remoteEP);
        }

        public static Task<int> SendToTaskAsync(this Socket @this, byte[] buffer, SocketFlags socketFlags, EndPoint remoteEP)
        {
            return @this.SendToTaskAsync(buffer, 0, buffer.Length, socketFlags, remoteEP);
        }

        public static Task<int> SendToTaskAsync(this Socket @this, byte[] buffer, EndPoint remoteEP)
        {
            return @this.SendToTaskAsync(buffer, 0, buffer.Length, SocketFlags.None, remoteEP);
        }

        public static Task<UdpReceiveResult> ReceiveFromTaskAsync(this Socket @this, byte[] buffer, int offset, int size, SocketFlags socketFlags)
        {
            EndPoint endPoint = new IPEndPoint(IPAddress.Any, 0);
            var state = new
            {
                Socket = @this,
                Buffer = buffer,
                EndPoint = endPoint
            };
            var tcs = new TaskCompletionSource<UdpReceiveResult>(state);

            @this.BeginReceiveFrom(buffer, offset, size, socketFlags, ref endPoint, a =>
            {
                var t = (TaskCompletionSource<UdpReceiveResult>) a.AsyncState;
                dynamic d = t.Task.AsyncState;
                Socket s = d.Socket;
                byte[] b = d.Buffer;
                EndPoint ep = d.EndPoint;

                try
                {
                    var bytesRead = s.EndReceiveFrom(a, ref ep);
                    var tmp = new byte[bytesRead];
                    Array.Copy(b, 0, tmp, 0, bytesRead);

                    t.TrySetResult(new UdpReceiveResult(tmp, (IPEndPoint)ep));
                }
                catch (Exception ex) { t.TrySetException(ex); }

            }, tcs);
            return tcs.Task;
        }

        public static Task<UdpReceiveResult> ReceiveFromTaskAsync(this Socket @this, byte[] buffer, int size, SocketFlags socketFlags)
        {
            return @this.ReceiveFromTaskAsync(buffer, 0, size, socketFlags);
        }

        public static Task<UdpReceiveResult> ReceiveFromTaskAsync(this Socket @this, byte[] buffer, SocketFlags socketFlags)
        {
            return @this.ReceiveFromTaskAsync(buffer, 0, buffer.Length, socketFlags);
        }

        public static Task<UdpReceiveResult> ReceiveFromTaskAsync(this Socket @this, byte[] buffer)
        {
            return @this.ReceiveFromTaskAsync(buffer, 0, buffer.Length, SocketFlags.None);
        }
    }
}
