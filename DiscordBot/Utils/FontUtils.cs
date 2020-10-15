using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Botflox.Bot.Utils
{
    public sealed class FontUtils : IDisposable
    {
        private readonly PrivateFontCollection _fontCollection = new PrivateFontCollection();
        private readonly List<IntPtr>          _fontUnloaders = new List<IntPtr>();
        private          bool                  _disposed;


        private readonly ConcurrentDictionary<string, TaskCompletionSource<FontFamily>> _completionSourceCache =
            new ConcurrentDictionary<string, TaskCompletionSource<FontFamily>>();

        public void Dispose() {
            if (_disposed) {
                return;
            }

            foreach (IntPtr coTaskMemHandle in _fontUnloaders) {
                Marshal.FreeCoTaskMem(coTaskMemHandle);
            }

            _fontCollection.Dispose();
            _fontUnloaders.Clear();

            _disposed = true;
        }

        public async ValueTask<FontFamily> LoadFontResource(string resourcePath) {
            if (_disposed)
                throw new ObjectDisposedException("FontUtils");
            TaskCompletionSource<FontFamily> tcs = new TaskCompletionSource<FontFamily>();
            if (!_completionSourceCache.TryAdd(resourcePath, tcs)) {
                return await _completionSourceCache[resourcePath].Task;
            }

            TaskCompletionSource<FontFamily> taskSource = tcs;
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
                    handleStream = new UnmanagedMemoryStream((byte*) handle.ToPointer(), length, length,
                        FileAccess.ReadWrite);
                }

                await using (handleStream) {
                    await stream.CopyToAsync(handleStream);
                }
            }

            await Task.Run(() => _fontCollection.AddMemoryFont(handle, length));
            _fontUnloaders.Add(handle);
            FontFamily family = _fontCollection.Families[_fontCollection.Families.Length - 1];
            taskSource.SetResult(family);
            return family;
        }
    }
}