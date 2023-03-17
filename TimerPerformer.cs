using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SysAdmin_Remider
{
    internal class TimerPerformer
    {
        public static Color GetBackColor(int timerId)
        {
            switch (timerId)
            {
                case 0: return Color.Yellow;
                case 1: return Color.Orange;
                case 2: return Color.Red;
                case 3: return Color.Gray;
                default: return Color.White;
            }
        }

        public static Color GetForeColor(int timerId)
        {
            switch (timerId)
            {
                case 0:
                case 1:    
                    return Color.Black;
                case 2: 
                case 3: 
                    return Color.White;
                default: 
                    return Color.Black;
            }
        }
    }
}
