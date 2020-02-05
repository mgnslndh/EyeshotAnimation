using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EyeshotAnimation
{
    class RandomColor
    {
        private static string[] colors = new[]
        {            
            "#C0C0C0",
            "#808080",
            "#000000",
            "#FF0000",
            "#800000",
            "#FFFF00",
            "#808000",
            "#00FF00",
            "#008000",
            "#00FFFF",
            "#008080",
            "#0000FF",
            "#000080",
            "#FF00FF",
            "#800080",

        };

        private static Random random = new Random(Environment.TickCount);

        public static System.Drawing.Color Next()
        {
            var colorIndex = random.Next(0, colors.Length - 1);
            var color = colors[colorIndex];           
            return ColorTranslator.FromHtml(color);
        }
    }
}
