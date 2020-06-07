using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace HDMA_Generator_Tool
{
    class Brightness_HDMA : HDMA
    {
        public enum Side { Top, Bottom, Both };
        public const int Register = 0x2100;

        public static Bitmap HDMA_To_Image(int Stretch, int Start_Brightness, Side Side)
        {
            Bitmap BM = new Bitmap(1, Scanlines);
            int Steps = 0x0F - Start_Brightness;
            if (Steps < 1 || Steps > 15 || Stretch < Steps)
                return BM;
            int Width = Stretch / Steps;

            for (int S = 0; S < Steps; S++)
                for (int W = 0; W < Width; W++)
                    BM.SetPixel(0,W + (Width * S),Color.FromArgb(255 - ((Start_Brightness + S) * 17),0,0,0));

            if(Side == Side.Bottom)
                BM.RotateFlip(RotateFlipType.RotateNoneFlipY);

            return BM;
        }

        public static Bitmap HDMA_To_Image(List<String[]> Table)
        {
            Bitmap BM = new Bitmap(1, Scanlines);
            int LineCounter = 0;

            foreach(String[] Arr in Table)
            {
                int ThisGradiant = Convert.ToInt32(Arr[0].Substring(1),16);
                if(ThisGradiant <= 0x80)
                    for (int SmallLines = 0; SmallLines < ThisGradiant; SmallLines++, LineCounter++)
                    {
                        if (LineCounter >= Scanlines)
                            break;
                        int Darkness = Convert.ToInt32(Arr[1].Substring(1),16);
                        BM.SetPixel(0, LineCounter, Color.FromArgb(255 - (Darkness * 17), Color.Black));
                    }
                else
                    for (int SmallLines = 0; SmallLines < ThisGradiant - 0x80; SmallLines++, LineCounter++)
                    {
                        if (SmallLines >= Scanlines  || SmallLines + 1 == Arr.Length)
                            break;
                        int Darkness = Convert.ToInt32(Arr[SmallLines + 1].Substring(1), 16);
                        BM.SetPixel(0, LineCounter, Color.FromArgb(255 - (Darkness * 17), Color.Black));
                    }
            }

            return BM;
        }

        public static Bitmap Merge(Bitmap Top, Bitmap Bottom)
        {
            Bitmap BM = new Bitmap(1,Scanlines);

            for (int h = 0; h < Scanlines; h++)
            {
                int ATop = Top.GetPixel(0, h).A;
                int ABottom = Bottom.GetPixel(0, h).A;
                BM.SetPixel(0, h, Color.FromArgb((ATop > ABottom ? ATop : ABottom), 0, 0, 0));
            }

            return BM;
        }

        public static String CodeFromImage(Bitmap BM, int Channel, String TableName)
        {
            List<String[]> Table = new List<string[]>();
            int LastOne = BM.GetPixel(0, 0).A;
            
            for (int h = 0, Counter = 0; h < BM.Height; h++)
            {                
                if (BM.GetPixel(0, h).A == LastOne && Counter < 0x80 && h < BM.Height - 1)
                    Counter++;
                else
                {
                    String[] Value = new string[2];
                    Value[0] = "$" + Counter.ToString("X2");
                    Value[1] = "$" + (0x0F - (LastOne / 17)).ToString("X2");
                    LastOne = BM.GetPixel(0, h).A;
                    Counter = 1;
                    Table.Add(Value);
                }
            }

            for (int i = Table.Count - 1; i >= 0; i--)
                if (Table[i][1] == "$0F")
                    Table.RemoveAt(i);
                else
                    break;

            int Base = 0x4330 + ((Channel - 3) * 0x10);
            int Mode = 0x00;
            String RegMode = (((Register % 0x100) << 8) + Mode).ToString("X4");
            String NewTable = "";

            foreach (String[] SA in Table)
            {
                String NewValue = "db ";
                for (int i = 0; i < SA.Length; i++)
                    NewValue += SA[i] + ",";
                NewValue = NewValue.Substring(0, NewValue.Length - 1) + "\n";
                NewTable += NewValue;
            }
            NewTable += "db $00\n";

            String Code = INIT_Label +
                "REP #$20\t\t;\\\n" +
                "LDA #$" + RegMode + "\t\t; | Use Mode " + Mode.ToString("X") + " on register " + Register.ToString("X") + "\n" +
                "STA $" + Base.ToString("X") + "\t\t; | 43" + Channel + "0 = Mode, 43" + Channel + "1 = Register\n" +
                "LDA #" + "." + TableName.TrimStart('.') + "\t\t; | Address of HDMA table\n" +
                "STA $" + (Base + 2).ToString("X") + "\t\t; | 43" + Channel + "2 = Low-Byte of table, 43" + Channel + "3 = High-Byte of table\n" +
                "LDY.b #" + "." + TableName.TrimStart('.') + ">>16\t; | Address of HDMA table, get bank byte\n" +
                "STY $" + (Base + 4).ToString("X") + "\t\t; | 43" + Channel + "4 = Bank-Byte of table\n" +
                "SEP #$20\t\t;/\n" +
                "LDA #$" + (0x08 << (Channel - 3)).ToString("X2") + "\t\t;\\\n" +
                "TSB $0D9F\t\t;/Enable HDMA channel " + Channel + "\n";


            Code += "RTS\n\n" +
                ";--------------------------\n" +
                "." + TableName.TrimStart('.') + "\n" + NewTable;

            return Code;
        }
    }
}
