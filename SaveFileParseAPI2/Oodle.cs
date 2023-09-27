using System.Reflection;
using System.Runtime.InteropServices;

public class OodleException : ParserException
{
    public OodleException(string? message = null, Exception? innerException = null) : base(message, innerException) { }
    public OodleException(FArchive reader, string? message = null, Exception? innerException = null) : base(reader, message, innerException) { }
}

public static class Oodle
{
    public unsafe delegate long OodleDecompress(byte* bufferPtr, long bufferSize, byte* outputPtr, long outputSize, int a, int b, int c, long d, long e, long f, long g, long h, long i, int threadModule);

    // private const string WARFRAME_CDN_HOST = "https://origin.warframe.com";
    // private const string WARFRAME_INDEX_PATH = "/origin/E926E926/index.txt.lzma";
    // private static string WARFRAME_INDEX_URL => WARFRAME_CDN_HOST + WARFRAME_INDEX_PATH;
    public const string OODLE_DLL_NAME = "oo2core"; //"oo2core_9_win64.dll";

    static unsafe Oodle()
    {
        NativeLibrary.SetDllImportResolver(typeof(Oodle).Assembly, ImportResolver);
    }

    private static IntPtr ImportResolver(string libraryName, Assembly assembly, DllImportSearchPath? searchpath)
    {
        IntPtr libHandle = IntPtr.Zero;
        if (libraryName == "oo2core")
        {
            // we manually load the assembly as the file name is not consistent between platforms thus the automatic searching will not work
            string assemblyFilename;
            bool is64Bit = Environment.Is64BitProcess;

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                assemblyFilename = $"oo2core_9_{(is64Bit ? "win64" : "win32")}.dll";
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                assemblyFilename = $"liboo2core{(is64Bit ? "linux64" : "linux")}.so.9";
            }
            else
            {
                throw new NotImplementedException();
            }
            IntPtr handle = NativeLibrary.Load(assemblyFilename, assembly, searchpath);
            libHandle = handle;
        }
        return libHandle;
    }

    public static unsafe void Decompress(byte[] compressed, int compressedOffset, int compressedSize,
                                            byte[] uncompressed, int uncompressedOffset, int uncompressedSize, FArchive? reader = null)
    {
        long decodedSize;

        fixed (byte* compressedPtr = compressed, uncompressedPtr = uncompressed)
        {
            decodedSize = OodleLZ_Decompress(compressedPtr + compressedOffset, compressedSize,
                                            uncompressedPtr + uncompressedOffset, uncompressedSize, 0, 0, 3, 0, 0, 0, 0, 0, 0, 3);
        }

        if (decodedSize <= 0)
        {
            if (reader != null) throw new OodleException(reader, $"Oodle decompression failed with result {decodedSize}");
            throw new OodleException($"Oodle decompression failed with result {decodedSize}");
        }

        if (decodedSize < uncompressedSize)
        {
            // Not sure whether this should be an exception or not
            Console.WriteLine("Oodle decompression just decompressed {0} bytes of the expected {1} bytes", decodedSize, uncompressedSize);
        }
    }

    [DllImport(OODLE_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
    static extern long OodleLZ_Decompress(byte[] buffer, long bufferSize, byte[] output, long outputBufferSize, int a, int b, int c, long d, long e, long f, long g, long h, long i, int threadModule);
    
    [DllImport(OODLE_DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
    static extern unsafe long OodleLZ_Decompress(byte* buffer, long bufferSize, byte* output, long outputBufferSize, int a, int b, int c, long d, long e, long f, long g, long h, long i, int threadModule);

}