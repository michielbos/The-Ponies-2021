// Credits to https://www.cs.rit.edu/~ncs/color/t_convert.html for the algorithms.

using UnityEngine;

namespace UI.Utils.ColorPicker
{
    /// <summary>
    /// Gives several methods to translate colors from one model to another.
    /// </summary>
    public static class ColorConverter
    {
        /// <summary>
        /// Converts an hsv color into a rgb color.
        /// </summary>
        /// <param name="hsvColor">The hsv color to convert.</param>
        /// <returns>The hsv color as an rgb color.</returns>
        public static Color HSVToRGB(HSVColor hsvColor)
        {
            float value = hsvColor.value;
            float saturation = hsvColor.saturation;

            // s == 0, the color is achromatic, so grey.
            if (saturation == 0)
                return new Color(value, value, value);

            float hue = hsvColor.hue / 60; // sector 0 to 5
            int i = Mathf.FloorToInt(hue);
            float f = hue - i; // Factorial part of h
            float p = value * (1 - saturation);
            float q = value * (1 - saturation * f);
            float t = value * (1 - saturation * (1 - f));

            Color color = new Color();

            switch (i)
            {
                case 0:
                    color.r = value;
                    color.g = t;
                    color.b = p;
                    break;
                case 1:
                    color.r = q;
                    color.g = value;
                    color.b = p;
                    break;
                case 2:
                    color.r = p;
                    color.g = value;
                    color.b = t;
                    break;
                case 3:
                    color.r = p;
                    color.g = q;
                    color.b = value;
                    break;
                case 4:
                    color.r = t;
                    color.g = p;
                    color.b = value;
                    break;
                case 5:
                    color.r = value;
                    color.g = p;
                    color.b = q;
                    break;
            }
            return color;
        }

        /// <summary>
        /// Converts an hsv color into an hex color.
        /// </summary>
        /// <param name="hsvColor">The hsv color to convert.</param>
        /// <returns>The hsv color as an hex color.</returns>
        public static HexColor HSVToHex(HSVColor hsvColor)
        {
            return RGBToHex(HSVToRGB(hsvColor));
        }

        /// <summary>
        /// Convert an hex color into an rgb color.
        /// </summary>
        /// <param name="hexColor">The hex color to convert.</param>
        /// <returns>The hex color as an RGB color.</returns>
        public static Color HexToRGB(HexColor hexColor)
        {
            if (ColorUtility.TryParseHtmlString(hexColor.hexValue, out Color rgbColor))
                return rgbColor;
            return new Color();
        }

        /// <summary>
        /// Convert an hex color into a HSV color.
        /// </summary>
        /// <param name="hexColor">The hex color to convert.</param>
        /// <returns>The hex color as a hsv color.</returns>
        public static HSVColor HexToHSV(HexColor hexColor)
        {
            return RGBToHSV(HexToRGB(hexColor));
        }

        /// <summary>
        /// Converts a rgb color into a hsv color.
        /// </summary>
        /// <param name="color">The rgb color to convert.</param>
        /// <returns>The rgb color as a hsv color.</returns>
        public static HSVColor RGBToHSV(Color color)
        {
            HSVColor hSVColor = new HSVColor();

            float r = color.r;
            float g = color.g;
            float b = color.b;

            float min = Mathf.Min(r, g, b);
            float max = Mathf.Max(r, g, b);
            float delta = max - min;

            hSVColor.value = max;

            if (max != 0)
                hSVColor.saturation = delta / max;
            else
            {
                hSVColor.saturation = 0;
                hSVColor.hue = -1;
                return hSVColor;
            }

            if (max == r)
                hSVColor.hue = (g - b) / delta;
            else if (max == g)
                hSVColor.hue = 2 + (b - r) / delta;
            else
                hSVColor.hue = 4 + (r - g) / delta;

            hSVColor.hue *= 60;
            if (hSVColor.hue < 0)
                hSVColor.hue += 360;

            return hSVColor;
        }

        /// <summary>
        /// Converts an rgb color into an hex color.
        /// </summary>
        /// <param name="color">The rgb color to convert.</param>
        /// <returns>The rgb color as an hex color.</returns>
        public static HexColor RGBToHex(Color color)
        {
            return new HexColor() { hexValue = ColorUtility.ToHtmlStringRGB(color) };
        }
    }
}