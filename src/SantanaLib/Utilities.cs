using System;

namespace SantanaLib
{
    public static class Utilities
    {
        private const double Terabyte = 0x10000000000;
        private const double Gigabyte = 0x40000000;
        private const double Megabyte = 0x100000;
        private const double Kilobyte = 0x400;

        public static bool IsMono { get; private set; }
#if !DNXCORE50
        public static OperatingSystem OperatingSystem { get; private set; }
#endif

        static Utilities()
        {
#if !DNXCORE50
            switch (Environment.OSVersion.Platform)
            {
                case PlatformID.Win32NT:
                    switch (Environment.OSVersion.Version.Major)
                    {
            #region 5

                        case 5:
                            switch (Environment.OSVersion.Version.Minor)
                            {
                                case 0:
                                    OperatingSystem = OperatingSystem.Win2000;
                                    break;

                                case 1:
                                    OperatingSystem = OperatingSystem.WinXP;
                                    break;

                                case 2:
                                    OperatingSystem = OperatingSystem.Win2003;
                                    break;

                                default:
                                    OperatingSystem = OperatingSystem.Unknown;
                                    break;
                            }
                            break;

            #endregion

            #region 6

                        case 6:
                            switch (Environment.OSVersion.Version.Minor)
                            {
                                case 0:
                                    OperatingSystem = OperatingSystem.WinVista;
                                    break;

                                case 1:
                                    OperatingSystem = OperatingSystem.Win7;
                                    break;

                                case 2:
                                    OperatingSystem = OperatingSystem.Win8;
                                    break;

                                case 3:
                                    OperatingSystem = OperatingSystem.Win81;
                                    break;

                                default:
                                    OperatingSystem = OperatingSystem.Unknown;
                                    break;
                            }
                            break;

            #endregion

            #region 10

                        case 10:
                            OperatingSystem = OperatingSystem.Win10;
                            break;

            #endregion

                        default:
                            OperatingSystem = OperatingSystem.Unknown;
                            break;
                    }
                    break;

                default:
                    OperatingSystem = OperatingSystem.Unknown;
                    break;
            }
#endif
            IsMono = Type.GetType("Mono.Runtime") != null;
        }

        internal static string ToFormattedSize(ulong value)
        {
            double divisor;
            string extension;
            if (value >= Terabyte)
            {
                extension = "TB";
                divisor = Terabyte;
            }
            else if (value >= Gigabyte)
            {
                extension = "GB";
                divisor = Gigabyte;
            }
            else if (value >= Megabyte)
            {
                extension = "MB";
                divisor = Megabyte;
            }
            else if (value >= Kilobyte)
            {
                extension = "KB";
                divisor = Kilobyte;
            }
            else
            {
                extension = "B";
                divisor = 1;
            }

            var result = value / divisor;
            return $"{result:0.##} {extension}";
        }

        internal static string ToFormattedSize(long value)
        {
            double divisor;
            string extension;
            if (value >= Terabyte)
            {
                extension = "TB";
                divisor = Terabyte;
            }
            else if (value >= Gigabyte)
            {
                extension = "GB";
                divisor = Gigabyte;
            }
            else if (value >= Megabyte)
            {
                extension = "MB";
                divisor = Megabyte;
            }
            else if (value >= Kilobyte)
            {
                extension = "KB";
                divisor = Kilobyte;
            }
            else
            {
                extension = "B";
                divisor = 1;
            }

            var result = value / divisor;
            return $"{result:0.##} {extension}";
        }

    }

    public enum OperatingSystem
    {
        Unknown,

        Win2000,
        WinXP,
        Win2003,
        WinVista,
        Win7,
        Win8,
        Win81,
        Win10
    }
}
