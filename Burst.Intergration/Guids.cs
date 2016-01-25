// Guids.cs
// MUST match guids.h
using System;

namespace BurstStudio.Burst_Intergration
{
    static class GuidList
    {
        public const string guidBurst_IntergrationPkgString = "cc052425-de7a-4fb6-ba1f-add2d8dde6da";
        public const string guidBurst_IntergrationCmdSetString = "7f80de42-a8e0-4515-8c8c-525067a524d4";

        public static readonly Guid guidBurst_IntergrationCmdSet = new Guid(guidBurst_IntergrationCmdSetString);
    };
}