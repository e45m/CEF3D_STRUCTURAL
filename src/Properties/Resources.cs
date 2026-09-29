namespace CEF.Properties;

using System;
using System.Drawing;
using System.IO;
using System.Reflection;

internal class Resources {
    
    private static readonly Assembly CurrentAssembly = typeof(Resources).Assembly;
    
    // Ajusta "TuCarpetaRecursos" al nombre exacto de la carpeta definida en el .csproj.
    // La estructura del nombre del recurso incrustado es: [RootNamespace].[NombreCarpeta].[NombreArchivo]
    private const string ResourcePrefix = "CEF.Resources.";

    internal Resources() {
    }

    private static Stream GetResourceStream(string fileName) {
        string resourceName = ResourcePrefix + fileName;
        Stream? stream = CurrentAssembly.GetManifestResourceStream(resourceName);
        if (stream == null) {
            throw new InvalidOperationException($"No se encontró el recurso incrustado: {resourceName}");
        }
        return stream;
    }

    private static Bitmap GetBitmap(string fileName) {
        using Stream stream = GetResourceStream(fileName);
        return new Bitmap(stream);
    }

    private static byte[] GetBytes(string fileName) {
        using Stream stream = GetResourceStream(fileName);
        using MemoryStream ms = new MemoryStream();
        stream.CopyTo(ms);
        return ms.ToArray();
    }

    internal static Bitmap Cripton => GetBitmap("Cripton.bmp");
    
    internal static Bitmap DocRecientes => GetBitmap("DocRecientes.bmp");
    
    internal static Bitmap Ejes => GetBitmap("Ejes.bmp");
    
    internal static Bitmap Escritorio => GetBitmap("Escritorio.bmp");
    
    internal static Bitmap GiraXmas => GetBitmap("GiraXmas.bmp");
    
    internal static Bitmap GiraXmenos => GetBitmap("GiraXmenos.bmp");
    
    internal static Bitmap GiraYmas => GetBitmap("GiraYmas.bmp");
    
    internal static Bitmap GiraYmenos => GetBitmap("GiraYmenos.bmp");
    
    internal static Bitmap GiraZmas => GetBitmap("GiraZmas.bmp");
    
    internal static Bitmap GiraZmenos => GetBitmap("GiraZmenos.bmp");
    
    internal static byte[] Icon1 => GetBytes("Icon1.ico");
    
    internal static Bitmap MiPc => GetBitmap("MiPc.bmp");
    
    internal static Bitmap MisDocs => GetBitmap("MisDocs.bmp");
    
    internal static Bitmap Nbarra => GetBitmap("Nbarra.bmp");
    
    internal static Bitmap PlanoMAs => GetBitmap("PlanoMAsX.bmp");
    
    internal static Bitmap PlanoMenos => GetBitmap("PlanoMenos.bmp");
    
    internal static Bitmap Refresh => GetBitmap("Refresh.bmp");
    
    internal static Bitmap Selbarra => GetBitmap("Selbarra.bmp");
    
    internal static Bitmap SelNudo => GetBitmap("SelNudo.bmp");
    
    internal static Bitmap ZoomMas => GetBitmap("ZoomMas.bmp");
    
    internal static Bitmap ZoomMenod => GetBitmap("ZoomMenodx.bmp");
}