using PdfSharp.Fonts;

namespace backend.Fonts
{
    public class InvoiceFontResolver : IFontResolver
    {
        public FontResolverInfo? ResolveTypeface(string familyName, bool isBold, bool isItalic)
        {
            if (!familyName.Equals("Arial", StringComparison.OrdinalIgnoreCase))
                return null;

            string faceName = (isBold, isItalic) switch
            {
                (true, true) => "LiberationSans-BoldItalic",
                (true, false) => "LiberationSans-Bold",
                (false, true) => "LiberationSans-Italic",
                _ => "LiberationSans-Regular"
            };

            return new FontResolverInfo(faceName, false, false);
        }

        public byte[]? GetFont(string faceName)
        {
            string fileName = faceName switch
            {
                "LiberationSans-Bold" => "liberationsans-bold.ttf",
                "LiberationSans-Italic" => "liberationsans-italic.ttf",
                "LiberationSans-BoldItalic" => "liberationsans-bolditalic.ttf",
                _ => "liberationsans.ttf"
            };

            var assembly = typeof(InvoiceFontResolver).Assembly;
            var resourceName = $"backend.Fonts.{fileName}";

            using var stream = assembly.GetManifestResourceStream(resourceName);
            if (stream is null)
                return null;

            using var memoryStream = new MemoryStream();
            stream.CopyTo(memoryStream);
            return memoryStream.ToArray();
        }
    }
}