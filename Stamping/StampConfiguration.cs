using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stamping
{
    public class StampConfiguration
    {
        public HorizontalStampAlignment HorizontalAlignment = HorizontalStampAlignment.Center;
        public VerticalStampAlignment VerticalAlignment = VerticalStampAlignment.Middle;
        public double Opacity { get; set; } = 0.5;
        public double Rotation { get; set; } = 0;
        public int[] Pages { get; set; }
        public int FontSize { get; set; } = 36;
        public string FontName { get; set; } = "Arial";
        public string FontColor { get; set; } = "#FF0000";
    }
    public enum HorizontalStampAlignment
    {
        Left,
        Center,
        Right
    }
    public enum VerticalStampAlignment
    {
        Top,
        Middle,
        Bottom
    }
}
