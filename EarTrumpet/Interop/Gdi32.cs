﻿using System;
using System.Runtime.InteropServices;

namespace EarTrumpet.Interop
{
    public class Gdi32
    {
        [DllImport("gdi32.dll", PreserveSig = true)]
        internal static extern bool DeleteObject(IntPtr objectHandle);
    }
}
