using System;
using System.Diagnostics;
﻿using System.Threading.Tasks;
using SantanaLib.Threading.Tasks;

namespace SantanaLib.Diagnostics
{
    public static class ProcessExtensions
    {
        public static Task WaitForExitAsync(this Process @this)
        {
            @this.EnableRaisingEvents = true;

            var tcs = new TaskCompletionSource();
            @this.Exited += (s, e) => tcs.TrySetResult();
            return tcs.Task;
        }
    }
}
