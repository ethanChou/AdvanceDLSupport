﻿using System;
using System.Runtime.InteropServices;

namespace AdvancedDLSupport
{
    /// <summary>
    /// Acts as the base for platform loaders.
    /// </summary>
    public abstract class PlatformLoaderBase : IPlatformLoader
    {
        /// <inheritdoc />
        public T LoadFunction<T>(IntPtr library, string symbolName)
        {
            var symbolPtr = LoadSymbol(library, symbolName);
            return Marshal.GetDelegateForFunctionPointer<T>(symbolPtr);
        }

        /// <inheritdoc />
        public abstract IntPtr LoadLibrary(string path);

        /// <inheritdoc />
        public abstract IntPtr LoadSymbol(IntPtr library, string symbolName);

        /// <inheritdoc />
        public abstract bool CloseLibrary(IntPtr library);

        /// <summary>
        /// Selects the appropriate platform loader based on the current platform.
        /// </summary>
        /// <returns>A platform loader for the current platform..</returns>
        /// <exception cref="PlatformNotSupportedException">Thrown if the current platform is not supported.</exception>
        public static IPlatformLoader SelectPlatformLoader()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return new WindowsPlatformLoader();
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return new LinuxPlatformLoader();
            }

            /*
                Temporary hack until BSD is added to RuntimeInformation. OSDescription should contain the output from
                "uname -srv", which will report something along the lines of FreeBSD or OpenBSD plus some more info.
            */
            bool isBSD = RuntimeInformation.OSDescription.ToUpperInvariant().Contains("BSD");
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX) || isBSD)
            {
                return new BSDPlatformLoader();
            }

            throw new PlatformNotSupportedException($"Cannot load native libraries on this platform: {RuntimeInformation.OSDescription}");
        }
    }
}
