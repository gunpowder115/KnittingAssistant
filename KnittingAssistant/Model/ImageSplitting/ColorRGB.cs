using System.Windows.Media;

namespace KnittingAssistant.Model.ImageSplitting
{
    //not used yet - Color structure from System.Windows.Media instead
    public class ColorRGB
    {
        public byte R { get; set; }
        public byte G { get; set; }
        public byte B { get; set; }

        public ColorRGB(in Color color) : this(color.R, color.G, color.B) { }
        public ColorRGB() : this(new Color()) { }
        public ColorRGB(ColorRGB color) : this(color.R, color.G, color.B) { }
        public ColorRGB(byte R, byte G, byte B)
        {
            this.R = R;
            this.G = G;
            this.B = B;
        }

        public void WriteColorRGB(Color color)
        {
            R = color.R;
            G = color.G;
            B = color.B;
        }

        public static explicit operator Color(ColorRGB param) => Color.FromRgb(param.R, param.G, param.B);
    }
}
