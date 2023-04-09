using System;
using System.Windows.Media;

namespace KnittingAssistant.Model
{
    //not used yet - RGB instead
    public class ColorHSL
    {
        public double H { get; private set; }
        public double S { get; private set; }
        public double L { get; private set; }

        public ColorHSL(double H, double S, double L)
        {
            this.H = H;
            this.S = S;
            this.L = L;
        }
        public ColorHSL() : this(0, 0, 0) { }
        public ColorHSL(Color rgbColor)
        {
            ConvertFromRgb(rgbColor);
        }

        public void ConvertFromRgb(Color rgbColor)
        {
            double normalR = rgbColor.R / 255d;
            double normalG = rgbColor.G / 255d;
            double normalB = rgbColor.B / 255d;
            double Cmax = SelectMaxComponent(normalR, normalG, normalB);
            double Cmin = SelectMinComponent(normalR, normalG, normalB);
            double delta = Cmax - Cmin;

            if (delta == 0) H = 0;
            else if (Cmax == normalR)
                H = 60 * (((normalG - normalB) / delta) % 6);
            else if (Cmax == normalG)
                H = 60 * (((normalB - normalR) / delta) + 2);
            else if (Cmax == normalB)
                H = 60 * (((normalR - normalG) / delta) + 4);

            L = (Cmax + Cmin) / 2;

            if (delta == 0) S = 0;
            else S = delta / (1 - Math.Abs(2 * L - 1));
        }

        public Color ConvertToRgb()
        {
            double C = (1 - Math.Abs(2 * L - 1)) * S;
            double X = C * (1 - Math.Abs((H / 60) % 2 - 1));
            double m = L - C / 2;
            double normalR = 0, normalG = 0, normalB = 0;
            if (H >= 0 && H < 60)
            {
                normalR = C;
                normalG = X;
                normalB = 0;
            }
            else if (H >= 60 && H < 120)
            {
                normalR = X;
                normalG = C;
                normalB = 0;
            }
            else if (H >= 120 && H < 180)
            {
                normalR = 0;
                normalG = C;
                normalB = X;
            }
            else if (H >= 180 && H < 240)
            {
                normalR = 0;
                normalG = X;
                normalB = C;
            }
            else if (H >= 240 && H < 300)
            {
                normalR = X;
                normalG = 0;
                normalB = C;
            }
            else if (H >= 300 && H < 360)
            {
                normalR = C;
                normalG = 0;
                normalB = X;
            }
            byte R = (byte)((normalR + m) * 255);
            byte G = (byte)((normalG + m) * 255);
            byte B = (byte)((normalB + m) * 255);
            return Color.FromRgb(R, G, B);
        }

        public void GetNegativeHslColor()
        {
            H = H < 180 ? H + 180 : H - 180;
        }

        private double SelectMaxComponent(double R, double G, double B)
        {
            double maxComponent = R;
            if (G > maxComponent) maxComponent = G;
            if (B > maxComponent) maxComponent = B;
            return maxComponent;
        }

        private double SelectMinComponent(double R, double G, double B)
        {
            double minComponent = R;
            if (G < minComponent) minComponent = G;
            if (B < minComponent) minComponent = B;
            return minComponent;
        }
    }
}
