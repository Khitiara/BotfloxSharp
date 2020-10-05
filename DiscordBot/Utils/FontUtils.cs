using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Reflection;
using System.Resources;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Botflox.Bot.Utils
{
    public sealed class FontUtils : IDisposable
    {
        private readonly List<CoTaskMemHandle> FontUnloaders = new List<CoTaskMemHandle>();

        public void Dispose() {
            foreach (CoTaskMemHandle coTaskMemHandle in FontUnloaders) {
                coTaskMemHandle.Dispose();
            }

            FontUnloaders.Clear();
        }

        private class CoTaskMemHandle : IDisposable
        {
            public readonly  IntPtr       Handle;
            private readonly IDisposable? _wrapped;

            public CoTaskMemHandle(IntPtr handle, IDisposable? wrapped = null) {
                Handle = handle;
                _wrapped = wrapped;
            }

            public void Dispose() {
                _wrapped?.Dispose();
                Marshal.FreeCoTaskMem(Handle);
            }

            public static implicit operator IntPtr(CoTaskMemHandle handle) {
                return handle.Handle;
            }
        }

        public async ValueTask<FontFamily> LoadFontResource(string resourcePath) {
            IntPtr handle;
            int length;

            await using (Stream? stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourcePath)) {
                if (stream == null) {
                    throw new FileNotFoundException("Could not find specified font resource",
                        $"application://{resourcePath}");
                }

                length = (int) stream.Length;
                handle = Marshal.AllocCoTaskMem(length);

                UnmanagedMemoryStream handleStream;
                unsafe {
                    handleStream = new UnmanagedMemoryStream((byte*) handle.ToPointer(), length, length, FileAccess.ReadWrite);
                }

                await using (handleStream) {
                    await stream.CopyToAsync(handleStream);
                }
            }

            PrivateFontCollection fontCollection = new PrivateFontCollection();
            await Task.Run(() => fontCollection.AddMemoryFont(handle, length));
            FontUnloaders.Add(new CoTaskMemHandle(handle, fontCollection));
            return fontCollection.Families[0];
        }
    }
}