using System.Drawing;

namespace Vkm.Common;

internal struct TextDrawElement
{
    public readonly string Text;
    public readonly FontFamily FontFamily;
    public readonly Color Color;
    public readonly byte Percentage;
    public readonly StringAlignment Alignment;

    public TextDrawElement(FontFamily fontFamily, string text, Color color, byte percentage, StringAlignment alignment)
    {
        Text = text;
        FontFamily = fontFamily;
        Color = color;
        Percentage = percentage;
        Alignment = alignment;
    }
}