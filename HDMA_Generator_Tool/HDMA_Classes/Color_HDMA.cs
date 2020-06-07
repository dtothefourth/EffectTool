using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using Extansion.Int;

namespace HDMA_Generator_Tool
{
    [Flags]
    public enum Farbe { Red = 0x20, Green = 0x40, Blue = 0x80 };
    abstract class ColorGradient_HDMA : HDMA
    {
        public const int Register = 0x2132;
        public abstract Bitmap Draw(List<String[]> Table);

        /// <summary>
        /// Erstellt aus den Angageben Elemtenten einen Table, aus dem die ColorGradient_HDMA Klasse einen Gradiant generieren kann und speichert diesen im Übergeben Objekt
        /// </summary>
        /// <param name="Farbe">Für welche Farbe der Angageben Gradiant ist</param>
        /// <param name="LB">Listbox in der die Zeilen stehen, die vom Programm ignoriert werden</param>
        /// <param name="Text">Enthält die Zeilen aus denen der Gradiant berechnet werden soll</param>
        /// <param name="b8Bit">gibt an ob der Wert in 8 Bit (0-255) oder in 5 Bit (0-31) ist.</param>
        /// <param name="P">Panel auf dem der Gradiant abgebildet wird</param>
        /// <param name="C_HDMA">Der Berechnete Table wird im Objekt gespeichert</param>
        public static Bitmap Calculate(Farbe Farbe, ListBox LB, String Text, bool b8Bit, Panel P, ColorGradient_HDMA C_HDMA)
        {
            LB.Items.Clear();                               // Löscht die Listbox, damit die Werte nicht Doppelt vorkommen
            String[] PreTable = Text.Split('\n');       // Splitet den Inhalt der RichTextBox an den Absetzen
            List<String[]> Tables = new List<string[]>();   // Der Table, in dem die Arrays gespeichert werden
            int CheckIf80 = 0;                              // Globale Variable den letzten Scanline Wert merkt

            foreach (String s in PreTable)  //Durchläuft alle Zeilen der Richtextbox
            {
                try
                {
                    if (s == "")    // \ Wenn der String leer ist, braucht er garnicht bearbeitet werden
                        continue;   // / deswegen beginnen wir von vorne

                    String[] PreValues = s.Split(',');              //Splitet die Zeile an den Beistrichen
                    for (int i = 0; i < PreValues.Length; i++)      //Durchläuft alle Werte die durch den Split entstanden sind.
                        PreValues[i] = PreValues[i].TrimStart('d', 'b').Trim();         //und entfernt die Lehrzeichen
                    String[] Values = new String[PreValues.Length]; //Neues Feld, in dem die Abgeänderten wertegespeichert werden.

                    //------------------Anzahl der Scanlines-------------------------------------------------------------------------------------

                    if (PreValues[0].Contains('x')) //überprüft, ob der Wert als 0xXX (in hex) angegeben wurde
                    {
                        CheckIf80 = Convert.ToInt32(PreValues[0].Substring(PreValues[0].IndexOf('x') + 1), 16); // \ Hohlt die Zahl nach dem x in HEX format, diese wird nicht Dividiert
                        Values[0] = "$" + CheckIf80.ToString("X2");                                             // / Fügt die Entsprechenden bits hinzu und den Wert zu dem Array
                    }
                    else if (PreValues[0].Contains('$')) //überprüft, ob der Wert als $XX (in hex) angegeben wurde
                    {
                        CheckIf80 = Convert.ToInt32(PreValues[0].Substring(PreValues[0].IndexOf('$') + 1), 16); // \ Hohlt die Zahl nach dem $ in HEX format, diese wird nicht Dividiert
                        Values[0] = "$" + CheckIf80.ToString("X2");                                             // / Fügt die Entsprechenden bits hinzu und den Wert zu dem Array
                    }
                    else //Wenn keines der anderen, dann ist der Wert als normale dezimalzahl angegeben
                    {
                        CheckIf80 = Convert.ToInt32(PreValues[0]);  // \ Hohlt die Zahl, (nicht Dividiert)
                        Values[0] = "$" + CheckIf80.ToString("X2"); // / Fügt die Entsprechenden bits hinzu und den Wert zu dem Array
                    }

                    //------------------Die zu setzende Farbe--------------------------------------------------------------------------------------
                    for (int i = 1; i < PreValues.Length; i++)  //Durchläuft die Schleife für alle restlichen Werte

                        if (PreValues[i].Contains('x')) //überprüft, ob der Wert als 0xXX (in hex) angegeben wurde
                        {
                            int Value = (Convert.ToInt32(PreValues[i].Substring(PreValues[i].IndexOf('x') + 1), 16) / (b8Bit ? 8 : 1)); // \ Hohlt die Zahl nach dem x in HEX format, dividiert wenn in 8Bit
                            Values[i] = "$" + ((Value & 0xFF) | (byte)Farbe).ToString("X2");                                            // / Fügt die Entsprechenden bits hinzu und den Wert zu dem Array
                        }
                        else if (PreValues[i].Contains('$'))    //überprüft, ob der Wert als $XX (in hex) angegeben wurde
                        {
                            int Value = (Convert.ToInt32(PreValues[i].Substring(PreValues[i].IndexOf('$') + 1), 16) / (b8Bit ? 8 : 1)); // \ Hohlt die Zahl nach dem $ in HEX format, dividiert wenn in 8Bit
                            Values[i] = "$" + ((Value & 0xFF) | (byte)Farbe).ToString("X2");                                            // / Fügt die Entsprechenden bits hinzu und den Wert zu dem Array
                        }
                        else //Wenn keines der anderen, dann ist der Wert als normale dezimalzahl angegeben
                        {
                            int Value = (Convert.ToInt32(PreValues[i]) / (b8Bit ? 8 : 1));   // \ Hohlt die Zahl, 
                            Values[i] = "$" + ((Value & 0xFF) | (byte)Farbe).ToString("X2"); // / Fügt die Entsprechenden bits hinzu und den Wert zu dem Array
                        }

                    if (CheckIf80 > 0x80)
                    {
                        if (Values.Length != CheckIf80 - 0x7F)  //Berechnung ob das Array groß genug ist wenn eine Angabe der Scanlines über 0x80 kommt.
                        {
                            LB.Items.Add(s);    // \ Wenn der Wert nicht passt, wird er der ListBox hinzugefügt
                            continue;           // / Und der Table wird nicht hinzugefügt
                        }
                    }
                    else
                        if (Values.Length != 2) //Wenn es sich nicht um einen Wert über 0x80 handelt, darf der Table nicht mehr als 2 Werte enthalten
                        {
                            LB.Items.Add(s);    // \ Wenn der Wert nicht passt, wird er der ListBox hinzugefügt
                            continue;           // / Und der Table wird nicht hinzugefügt
                        }
                    Tables.Add(Values); //Fügt das Array (Zeile im Table) der List (Gesamt Table) hinzu
                }
                catch
                {
                    LB.Items.Add(s);
                }
            }
            C_HDMA.Tables = Tables;                             //Speichert den Table in dem Klassenobjekt
            P.BackgroundImage = C_HDMA.Draw(Tables);   //Genertiert die Bitmap für das Hintergrundbild aus dem Table
            P.Update();                                         //Updated das Panel
            return (Bitmap)P.BackgroundImage;
        }
    }

    class BG_ColorGradient_HDMA : ColorGradient_HDMA
    {
        public BG_ColorGradient_HDMA() { _tables = new List<string[]>(); }

        public Bitmap Draw(bool Red, bool Green, bool Blue, int Top, int Bottom, bool Centered)
        { return Draw(Red, Green, Blue, Top, Bottom, 1, Centered); }

        /// <summary>
        /// Zeichnet den gewünschten HDMA Effekt und trägt die werte in den Internen Table ein.
        /// </summary>
        /// <param name="Red">Gibt an, ob der HDMA Effekt einen Rot anteil hat</param>
        /// <param name="Green">Gibt an, ob der HDMA Effekt einen Grün anteil hat</param>
        /// <param name="Blue">Gibt an, ob der HDMA Effekt einen Blau anteil hat</param>
        /// <param name="Top">Der obere Wert des HDMA Effekts (0-255)</param>
        /// <param name="Bottom">Der untere Wert des HDMA Effekts (0-255)</param>
        /// <param name="Centered">Gibt an, ob der HDMA Effekt von oben nach unten verläuft, oder über die Mitte gespiegelt ist.
        /// True, wenn er gespiegelt werden soll</param>
        /// <returns>Gibt eine 1-Spalten-breites Bitmap zurück, dass genau 224 Pixel hoch ist. Es enthält den HDMA Effekt.</returns>
        public Bitmap Draw(bool Red, bool Green, bool Blue, int Top, int Bottom, int Divider, bool Centered)
        {
            Bitmap BM = new Bitmap(1, Scanlines);
            _tables = new List<string[]>();

            if (!Centered)
            {
                int Steps = Math.Abs(Top - Bottom) / 8 + 1;
                Steps = Steps / Divider;

                if (Steps <= 1)
                {
                    for (int x = 0; x < BM.Width; x++)
                        for (int y = 0; y < BM.Height; y++)
                            BM.SetPixel(x, y, Color.FromArgb(Red ? Bottom : 0, Green ? Bottom : 0, Blue ? Bottom : 0));

                    if (Top == 0 && Bottom == 0)
                        return BM;

                    int Gradient = Bottom / 8;
                    if (Red) Gradient |= 0x20;
                    if (Green) Gradient |= 0x40;
                    if (Blue) Gradient |= 0x80;
                    _tables.Add(new String[2] { "$70", "$" + Gradient.ToString("X2") });
                    _tables.Add(new String[2] { "$70", "$" + Gradient.ToString("X2") });
                }
                else
                {
                    int Dif = Math.Abs((Top - Bottom) / (Steps - 1));

                    int Width = (BM.Height / Steps);
                    if ((Width * Steps) < Scanlines)
                        Width++;

                    for (int S = 0; S < Steps; S++)
                    {
                        int Gradient = Top - (Dif * S * (Top > Bottom ? 1 : -1));

                        for (int y = 0; y < Width; y++)
                            for (int x = 0; x < BM.Width; x++)
                            {
                                if (y + (S * Width) > BM.Height - 1)
                                    break;
                                BM.SetPixel(x, y + (S * Width), Color.FromArgb(Red ? Gradient : 0,
                                    Green ? Gradient : 0,
                                    Blue ? Gradient : 0));
                            }

                        Gradient = Gradient / 8;
                        if (Red) Gradient |= 0x20;
                        if (Green) Gradient |= 0x40;
                        if (Blue) Gradient |= 0x80;

                        _tables.Add(new String[2] { "$" + Width.ToString("X2"), "$" + Gradient.ToString("X2") });
                    }
                }
            }
            else
            {
                if (Top == 0 && Bottom == 0)
                    return BM;
                int Steps = Math.Abs(Top - Bottom) / 8 + 2;
                Steps = Steps / Divider;


                if(Steps <= 1)
                {
                    for (int x = 0; x < BM.Width; x++)
                        for (int y = 0; y < BM.Height; y++)
                            BM.SetPixel(x, y, Color.FromArgb(Red ? Bottom : 0, Green ? Bottom : 0, Blue ? Bottom : 0));
                    
                    int Gradient = Bottom / 8;
                    if (Red) Gradient |= 0x20;
                    if (Green) Gradient |= 0x40;
                    if (Blue) Gradient |= 0x80;
                    _tables.Add(new String[2] { "$70", "$" + Gradient.ToString("X2") });
                    _tables.Add(new String[2] { "$70", "$" + Gradient.ToString("X2") });

                    return BM;
                }

                int Dif = Math.Abs((Top - Bottom) / (Steps / 2));

                int Width = (BM.Height / Steps);
                if ((Width * Steps) < Scanlines)
                    Width++;

                int LastS = 0; ;

                for (int S = 0; S < Steps; S++)
                {
                    int Gradient = 0;
                    for (int y = 0; y < Width; y++)
                        for (int x = 0; x < BM.Width; x++)
                        {
                            if (y + (S * Width) > BM.Height - 1)
                            {
                                Gradient = Bottom - (Dif * (S - LastS) * (Top > Bottom ? -1 : 1));
                                break;
                            }
                            if (y + (S * Width) < BM.Height / 2)
                            {
                                Gradient = Top - (Dif * S * (Top > Bottom ? 1 : -1));
                                BM.SetPixel(x, y + (S * Width), Color.FromArgb(Red ? Top - (Dif * S * (Top > Bottom ? 1 : -1)) : 0,
                                    Green ? Top - (Dif * S * (Top > Bottom ? 1 : -1)) : 0,
                                    Blue ? Top - (Dif * S * (Top > Bottom ? 1 : -1)) : 0));
                                LastS = S;
                            }
                            else
                            {
                                Gradient = Bottom - (Dif * (S - LastS) * (Top > Bottom ? -1 : 1));
                                BM.SetPixel(x, y + (S * Width), Color.FromArgb(Red ? Bottom - (Dif * (S - LastS) * (Top > Bottom ? -1 : 1)) : 0,
                                    Green ? Bottom - (Dif * (S - LastS) * (Top > Bottom ? -1 : 1)) : 0,
                                    Blue ? Bottom - (Dif * (S - LastS) * (Top > Bottom ? -1 : 1)) : 0));
                            }
                        }

                    Gradient = Gradient / 8;
                    if (Red) Gradient |= 0x20;
                    if (Green) Gradient |= 0x40;
                    if (Blue) Gradient |= 0x80;

                    _tables.Add(new String[2] { "$" + Width.ToString("X2"), "$" + Gradient.ToString("X2") });
                }
            }
            return BM;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Table"></param>
        /// <returns>Gibt eine 1-Spalten-breites Bitmap zurück, dass genau 224 Pixel hoch ist. Es enthält den HDMA Effekt.</returns>
        public override Bitmap Draw(List<String[]> Table)
        {
            int LineCounter = 0;
            Bitmap BM = new Bitmap(1,Scanlines);
            foreach(String[] SA in Table)
            {
                if(Convert.ToInt32(SA[0].Substring(1), 16) < 0x80)
                {
                    for (int forCounter = 0; forCounter < Convert.ToInt32(SA[0].Substring(1), 16); LineCounter++, forCounter++)
                    {
                        if (LineCounter >= Scanlines)
                            continue;

                        int Gradient = Convert.ToInt32(SA[1].Substring(1), 16);
                        if (Gradient > 255 || Gradient < 0)
                            throw new ArgumentOutOfRangeException("The table contains a value which is greater than 255 (0xFF) for a color");

                        bool Red = ((Gradient & 0x20) != 0);
                        bool Green = ((Gradient & 0x40) != 0);
                        bool Blue = ((Gradient & 0x80) != 0);

                        int Value = (Gradient & 0x1F) * 8;

                        Color PreValue = BM.GetPixel(0, LineCounter);
                        byte PreRed = PreValue.R;
                        byte PreGreen = PreValue.G;
                        byte PreBlue = PreValue.B;

                        BM.SetPixel(0, LineCounter, Color.FromArgb(Red ? Value : PreRed, Green ? Value : PreGreen, Blue ? Value : PreBlue));
                    }
                }
                else
                {
                    for (int forCounter = 0; forCounter < (Convert.ToInt32(SA[0].Substring(1), 16)) - 0x80; LineCounter++, forCounter++)
                    {
                        if (LineCounter >= Scanlines)
                            continue;
                        if (forCounter + 1 == SA.Length)
                            throw new InvalidOperationException("The Scanlinecount is set to " + SA[0] + 
                                ", which means it will write each following value in one scanline for the next " + 
                                ((Convert.ToInt32(SA[0].Substring(1), 16)) - 0x80) + 
                                " values\nPlease make sure you have at least this many values set.");

                        int Gradient = Convert.ToInt32(SA[forCounter + 1].Substring(1), 16);
                        if (Gradient > 255 || Gradient < 0)
                            throw new ArgumentOutOfRangeException("The table contains a value which is greater than 255 (0xFF) for a color");

                        bool Red = ((Gradient & 0x20) != 0);
                        bool Green = ((Gradient & 0x40) != 0);
                        bool Blue = ((Gradient & 0x80) != 0);

                        int Value = (Gradient & 0x1F) * 8;

                        Color PreValue = BM.GetPixel(0, LineCounter);
                        byte PreRed = PreValue.R;
                        byte PreGreen = PreValue.G;
                        byte PreBlue = PreValue.B;

                        BM.SetPixel(0, LineCounter, Color.FromArgb(Red ? Value : PreRed, Green ? Value : PreGreen, Blue ? Value : PreBlue));           
                    }
                }
            }
            return BM;
        }

        public String Code() { return Code(this._channel, this._tables, ""); }
        public String Code(int Channel) { return Code(Channel, this._tables, ""); }
        public String Code(int Channel, String Tablename) { return Code(Channel, this._tables, Tablename); }
        public static String Code(int Channel, List<String[]> Tables) { return Code(Channel, Tables, ""); }
        public static String Code(int _channel, List<String[]> _tables, String TableName)
        {
            if (_tables == null || _tables.Count == 0)
                return "";

            if (TableName == "")
                TableName = ".HDMA_Table";

            int Base = 0x4330 + ((_channel - 3) * 0x10);
            int Mode = 0x00;
            if (_tables[0].Length == 3)
                Mode = 0x02;
            String RegMode = (((Register % 0x100) << 8) + Mode).ToString("X4");

            String NewTable = "";

            foreach (String[] SA in _tables)
            {
                String NewValue = "db ";
                for (int i = 0; i < SA.Length; i++)
                    NewValue += SA[i] + ",";
                NewValue = NewValue.Substring(0, NewValue.Length - 1) + "\n";
                NewTable += NewValue;
            }
            NewTable += "db $00\n";

            String Code = 
                "REP #$20\t\t;\\\n" +
                "LDA #$" + RegMode + "\t\t; | Use Mode " + Mode.ToString("X") + " on register " + Register.ToString("X") + "\n" +
                "STA $" + Base.ToString("X") + "\t\t; | 43" + _channel + "0 = Mode, 43" + _channel + "1 = Register\n" +
                "LDA #" + "." + TableName.TrimStart('.') + "\t; | Address of HDMA table\n" +
                "STA $" + (Base + 2).ToString("X") + "\t\t; | 43" + _channel + "2 = Low-Byte of table, 43" + _channel + "3 = High-Byte of table\n" +
                "LDY.b #" + "." + TableName.TrimStart('.') + ">>16\t; | Address of HDMA table, get bank byte\n" +
                "STY $" + (Base + 4).ToString("X") + "\t\t; | 43" + _channel + "4 = Bank-Byte of table\n" +
                "SEP #$20\t\t;/\n";

            Code += "RTS\n\n" +
                ";--------------------------\n" +
                "." + TableName.TrimStart('.') + "\n" + NewTable;
            return Code;//.Replace("\n", "\r\n");
        }        
        public static String Code(BG_ColorGradient_HDMA HDMA1, BG_ColorGradient_HDMA HDMA2, BG_ColorGradient_HDMA HDMA3)
        {
            List<String[]> NewTable;
            int Channel1 = 3, Channel2 = 4;
            String CodeToReturn = "";

            if (HDMA1._tables.Count == 0 && HDMA2._tables.Count == 0 && HDMA3._tables.Count == 0)
                return "";

            else if (HDMA1._tables.Count == 0 && HDMA2._tables.Count == 0)
            {
                MessageBox.Show(Settings.OneChannel, "Saving Channels", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ChooseChannel CC = new ChooseChannel();
                CC.ShowDialog();
                Channel1 = CC.HighChannel;

                CodeToReturn =  HDMA3.Code(Channel1);
            }
            else if (HDMA3._tables.Count == 0 && HDMA2._tables.Count == 0)
            {
                MessageBox.Show(Settings.OneChannel, "Saving Channels", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ChooseChannel CC = new ChooseChannel();
                CC.ShowDialog();
                Channel1 = CC.HighChannel;

                CodeToReturn =  HDMA1.Code(Channel1);
            }
            else if (HDMA1._tables.Count == 0 && HDMA3._tables.Count == 0)
            {
                MessageBox.Show(Settings.OneChannel, "Saving Channels", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ChooseChannel CC = new ChooseChannel();
                CC.ShowDialog();
                Channel1 = CC.HighChannel;

                CodeToReturn =  HDMA2.Code(Channel1);
            }

            else if(Join((HDMA)HDMA1,(HDMA)HDMA2, out NewTable))
            {
                MessageBox.Show(Settings.TwoChannels, "Saving Channels", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ChooseChannel CC = new ChooseChannel();
                CC.ShowDialog();

                Channel1 = CC.HighChannel;
                Channel2 = CC.LowChannel;

                String PreCode = Code(Channel1, NewTable, ".HDMA_RedGre");
                if (HDMA3._tables.Count != 0)
                {
                    String FirstCode = String.Join("\n", PreCode.Split('\n'), 0, 7);
                    String FirstTable = String.Join("\n", PreCode.Split('\n'), 10, PreCode.Split('\n').Length - 10);
                    String NewCode = HDMA3.Code(Channel2, ".HDMA_Blue");
                    String SecondCode = String.Join("\n", NewCode.Split('\n'), 1, 7);
                    String SecondTable = String.Join("\n", NewCode.Split('\n'), 8, NewCode.Split('\n').Length - 8);
                    CodeToReturn =  (FirstCode + "\n" + SecondCode + "\n" + SecondTable + "\n" + FirstTable);
                }
                else
                    CodeToReturn =  PreCode;
            }
            else if (Join((HDMA)HDMA2, (HDMA)HDMA3, out NewTable))
            {
                MessageBox.Show(Settings.TwoChannels, "Saving Channels", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ChooseChannel CC = new ChooseChannel();
                CC.ShowDialog();

                Channel1 = CC.HighChannel;
                Channel2 = CC.LowChannel;

                String PreCode = Code(Channel1, NewTable, ".HDMA_GreBlu");
                if (HDMA1._tables.Count != 0)
                {
                    String FirstCode = String.Join("\n", PreCode.Split('\n'), 0, 7);
                    String FirstTable = String.Join("\n", PreCode.Split('\n'), 10, PreCode.Split('\n').Length - 10);
                    String NewCode = HDMA1.Code(Channel2, ".HDMA_Red");
                    String SecondCode = String.Join("\n", NewCode.Split('\n'), 1, 7);
                    String SecondTable = String.Join("\n", NewCode.Split('\n'), 8, NewCode.Split('\n').Length - 8);
                    CodeToReturn =  (FirstCode + "\n" + SecondCode + "\n" + SecondTable + "\n" + FirstTable);
                }
                else
                    CodeToReturn =  PreCode;
            }
            else if (Join((HDMA)HDMA1, (HDMA)HDMA3, out NewTable))
            {
                MessageBox.Show(Settings.TwoChannels, "Saving Channels", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ChooseChannel CC = new ChooseChannel();
                CC.ShowDialog();

                Channel1 = CC.HighChannel;
                Channel2 = CC.LowChannel;

                String PreCode = Code(Channel1, NewTable, ".HDMA_RedBlu");
                if (HDMA2._tables.Count != 0)
                {
                    String FirstCode = String.Join("\n", PreCode.Split('\n'), 0, 7);
                    String FirstTable = String.Join("\n", PreCode.Split('\n'), 10, PreCode.Split('\n').Length - 10);
                    String NewCode = HDMA2.Code(Channel2, ".HDMA_Green");
                    String SecondCode = String.Join("\n", NewCode.Split('\n'), 1, 7);
                    String SecondTable = String.Join("\n", NewCode.Split('\n'), 8, NewCode.Split('\n').Length - 8);
                    CodeToReturn =  (FirstCode + "\n" + SecondCode + "\n" + SecondTable + "\n" + FirstTable);
                }
                else
                    CodeToReturn =  PreCode;
            }
            else
            {
                bool useRed = (HDMA1._tables.Count > 0);
                bool useGreen = (HDMA2._tables.Count > 0);
                bool useBlue = (HDMA3._tables.Count > 0);
                
                bool Use3 = true;
                bool Use4 = true;
                bool Use5 = true;

                int[] Channels = new int[3] { 3, 4, 5 };

                if (!(useBlue && useGreen && useRed))
                {
                    MessageBox.Show("It appears that only 2 out of 3 channels are needed for this code. Please select the two channels you want to use.","Saving Channels",MessageBoxButtons.OK,MessageBoxIcon.Information);
                    ChooseChannel CC = new ChooseChannel();
                    CC.ShowDialog();

                    Channels[0] = CC.HighChannel;
                    Channels[1] = CC.LowChannel;

                    Use3 = (CC.HighChannel == 3 || CC.LowChannel == 3);
                    Use4 = (CC.HighChannel == 4 || CC.LowChannel == 4);
                    Use5 = (CC.HighChannel == 5 || CC.LowChannel == 5);
                }

                int Mode = 0x00;
                String RegMode = (((Register % 0x100) << 8) + Mode).ToString("X4");

                String RedTable = "";
                String GreenTable = "";
                String BlueTable = "";

                foreach (String[] SA in HDMA1._tables)
                {
                    String NewValue = "db ";
                    for (int i = 0; i < SA.Length; i++)
                        NewValue += SA[i] + ",";
                    NewValue = NewValue.Substring(0, NewValue.Length - 1) + "\n";
                    RedTable += NewValue;
                }
                RedTable += "db $00\n";
                foreach (String[] SA in HDMA2._tables)
                {
                    String NewValue = "db ";
                    for (int i = 0; i < SA.Length; i++)
                        NewValue += SA[i] + ",";
                    NewValue = NewValue.Substring(0, NewValue.Length - 1) + "\n";
                    GreenTable += NewValue;
                }
                GreenTable += "db $00\n";
                foreach (String[] SA in HDMA3._tables)
                {
                    String NewValue = "db ";
                    for (int i = 0; i < SA.Length; i++)
                        NewValue += SA[i] + ",";
                    NewValue = NewValue.Substring(0, NewValue.Length - 1) + "\n";
                    BlueTable += NewValue;
                }
                BlueTable += "db $00\n";

                String Code = 
                    "REP #$20\t\t;\\\n" +
                    "LDA #$" + RegMode + "\t\t; | Use Mode " + Mode.ToString("X") + " on register " + Register.ToString("X") + "\n" +
                    (Use3 ? "STA $4330\t\t; | 4330 = Mode, 4331 = Register for Channel 3\n" : "") +
                    (Use4 ? "STA $4340\t\t; | 4340 = Mode, 4341 = Register for Channel 4\n" : "") +
                    (Use5 ? "STA $4350\t\t; | 4350 = Mode, 4351 = Register for Channel 5\n" : "");

                int Counter = 0;

                if(useRed)
                {
                    Code += "LDA #.HDMA_Red\t\t; | Address of red HDMA table\n" +
                    "STA $43" + Channels[Counter] + "2\t\t; | 43" + Channels[Counter] + "2 = Low-Byte of table, 43" + Channels[Counter] + "3 = High-Byte of table for Channel " + Channels[Counter] + "\n" +
                    "LDY.b #.HDMA_Red>>16\t; | Address of red HDMA table, get bank byte\n" +
                    "STY $43" + Channels[Counter] + "4\t\t; | 43" + Channels[Counter] + "4 = Bank-Byte of table for Channel " + Channels[Counter] + "\n";
                    Counter++;
                }
                if(useGreen)
                {
                    Code += "LDA #.HDMA_Green\t; | Address of green HDMA table\n" +
                    "STA $43" + Channels[Counter] + "2\t\t; | 43" + Channels[Counter] + "2 = Low-Byte of table, 43" + Channels[Counter] + "3 = High-Byte of table for Channel " + Channels[Counter] + "\n" +
                    "LDY.b #.HDMA_Green>>16\t; | Address of green HDMA table, get bank byte\n" +
                    "STY $43" + Channels[Counter] + "4\t\t; | 43" + Channels[Counter] + "4 = Bank-Byte of table for Channel " + Channels[Counter] + "\n";
                    Counter++;
                }
                if(useBlue)
                {
                    Code += "LDA #.HDMA_Blue\t\t; | Address of blue HDMA table\n" +
                    "STA $43" + Channels[Counter] + "2\t\t; | 43" + Channels[Counter] + "2 = Low-Byte of table, 43" + Channels[Counter] + "3 = High-Byte of table for Channel " + Channels[Counter] + "\n" +
                    "LDY.b #.HDMA_Blue>>16\t; | Address of blue HDMA table, get bank byte\n" +
                    "STY $43" + Channels[Counter] + "4\t\t; | 43" + Channels[Counter] + "4 = Bank-Byte of table for Channel " + Channels[Counter] + "\n";
                }

                Code += "SEP #$20\t\t;/\n" +
                    "LDA #%00" + (Use5 ? "1" : "0") + (Use4 ? "1" : "0") + (Use3 ? "1" : "0") + "000\n" + 
                    "TSB $0D9F\n" +
                    "RTS\n\n" +
                    ";--------------------------\n";

                if (HDMA1._tables.Count > 0) Code += ".HDMA_Red\n" + RedTable + "\n\n";
                if (HDMA2._tables.Count > 0) Code += ".HDMA_Green\n" + GreenTable + "\n\n";
                if(HDMA3._tables.Count > 0) Code += ".HDMA_Blue\n" + BlueTable;

                return Code;
            }

            //Enable der Channel Hinzufügen
            bool UseChannel3 = CodeToReturn.Contains("$4330");
            bool UseChannel4 = CodeToReturn.Contains("$4340");
            bool UseChannel5 = CodeToReturn.Contains("$4350");

            String ReplaceCode = "LDA #%00" + (UseChannel5 ? "1" : "0") + (UseChannel4 ? "1" : "0") + (UseChannel3 ? "1" : "0") + "000\n" +
                    "TSB $0D9F\n" +
                    "RTS";

            return CodeToReturn.Replace("RTS", ReplaceCode);
        }

        public static Bitmap Merge(Bitmap Red, Bitmap Green, Bitmap Blue)
        {
            Bitmap Main = new Bitmap(1, Scanlines);

            for (int i = 0; i < HDMA.Scanlines; i++)
                Main.SetPixel(0, i, Color.FromArgb(Red.GetPixel(0, i).R, Green.GetPixel(0, i).G, Blue.GetPixel(0, i).B));
            return Main;
        }
    }

    class FG_ColorGradient_HDMA : ColorGradient_HDMA
    {
        public enum ColorMathStyle : byte { Addiditve = 0x00, Substracting = 0x80, AdditiveHalf = 0x40, SubstractingHalf = 0xC0 };
        private class TableCalculator
        {
            private List<int> Values;
            private int Width_Row;
            private int Reach;

            /// <summary>
            /// Inizialisierung für das Objekt.
            /// </summary>
            /// <param name="Width_Row">Die Breite einer einzelnen Spalte</param>
            /// <param name="Reach">Die Reichweite des Effekts.</param>
            public TableCalculator(int Width_Row, int Reach)
            {
                this.Width_Row = Width_Row;
                this.Reach = Reach;
                Values = new List<int>();
            }

            /// <summary>
            /// Ruft das Element an der angegebenen Stelle ab oder legt es fest.
            /// </summary>
            /// <param name="Index">Der nullbasierte Index der Vaiablenliste. Der Index kann nicht größer als 223</param>
            /// <returns>Der Wert an der entsprechenden stelle in der Liste</returns>
            public int this[int Index] 
            {
                get 
                {
                    if (this == null)
                        throw new ArgumentNullException();
                    if (Index >= Values.Count)
                        throw new IndexOutOfRangeException();
                    return Values[Index]; 
                }
                set
                {
                    if (this == null)
                        throw new ArgumentNullException();
                    if (Index >= Values.Count)
                        throw new IndexOutOfRangeException();
                    Values[Index] = value;
                }
            }

            public void Clear() { Values.Clear(); }
            public void Reverse() { Values.Reverse(); }
            public void Reverse(int Index, int Count) { Values.Reverse(Index, Count); }

            public void Add(int Value)
            {
                if (Values.Count > 224)
                    throw new OutOfMemoryException("You may not enter more than 224 values");
                Values.Add(Value);
            }

            private delegate List<String[]> WriteTable(List<int> ColorTable1, List<int> ColorTable2, int Width1, int Width2, Farbe Color);
            public static void GetTable(TableCalculator T1, TableCalculator T2, Farbe ColorToUse, ref List<String[]> Table)
            {
                WriteTable DelegeFunction = (T1Col, T2Col, T1Width, T2Width, Col) =>
                {
                    List<String[]> Ret = new List<string[]>();
                    int cnt = 0;

                    if (T1Col.Count == 0 && T2Col.Count == 0)
                        return Ret;

                    for (cnt = 0; cnt < T1Col.Count && T1Col[cnt] != 0; cnt++)
                        Ret.Add(new string[2] { "$" + T1Width.ToString("X2"), "$" + ((byte)Col | T1Col[cnt]).ToString("X2") });

                    if (cnt == Scanlines)
                        return Ret;
                    if (T2Col.Count == 0)
                    {
                        Ret.Add(new String[2] { "$01", "$" + ((int)Col).ToString("X2") });
                        return Ret;
                    }

                    int NumberOfZero = Scanlines - (T1Col.Count * T1Width) - (T2Col.Count * T2Width);
                    if(NumberOfZero != 0)
                        if (NumberOfZero <= 0x80)
                            Ret.Add(new string[2] { "$" + NumberOfZero.ToString("X2"), "$00" });
                        else
                        {
                            Ret.Add(new string[2] { "$80", "$" + ((int)Col).ToString("X2") });
                            Ret.Add(new string[2] { "$" + (NumberOfZero - 0x80).ToString("X2"), "$" + ((int)Col).ToString("X2") });
                        }

                    for (cnt = 0; cnt < T2Col.Count; cnt++)
                        Ret.Add(new string[2] { "$" + T2Width.ToString("X2"), "$" + ((byte)Col | T2Col[cnt]).ToString("X2") });
                    return Ret;
                };
                
                Table = DelegeFunction(T1.Values, T2.Values, T1.Width_Row, T2.Width_Row, ColorToUse);
            }
        }
        
        public Bitmap BaseImage = Properties.Resources.BanzaiBlack;

        public FG_ColorGradient_HDMA() { _tables = new List<string[]>(); }
        public FG_ColorGradient_HDMA(Farbe ColorToUse) : this()
        {
            this.ColorToUse = ColorToUse;
        }
        public FG_ColorGradient_HDMA(Farbe ColorToUse, bool Part) : this(ColorToUse)
        {
            PartOfTotal = Part;
        }

        /// <summary>
        /// Der Anfnagswert der Farbe in 8bit.
        /// </summary>
        public int TopValue, BottomValue;
        /// <summary>
        /// Die anzahl der Pixel wie weit der Effekt reichen soll. Die Anzahl wird womöglich nicht ganz erreicht,
        /// Wenn keine gleiche Verteilung möglich ist.
        /// </summary>
        public int TopReach, BottomReach;
        /// <summary>
        /// Welche Funktion der Color Math auf diesen Gradient angewendet werden soll.
        /// </summary>
        public ColorMathStyle Style = ColorMathStyle.Addiditve;
        /// <summary>
        /// Farbe des HDMA Gradient Effekts.
        /// </summary>
        public Farbe ColorToUse;

        public bool PartOfTotal = false;

        private TableCalculator TableTop;
        private TableCalculator TableBottom;


        public virtual String Code() { return Code(this._channel, this._tables, ".ColorData"); }
        public virtual String Code(int Channel) { return Code(Channel, this._tables, ".ColorData"); }
        public virtual String Code(int Channel, String Tablename) { return Code(Channel, this._tables, Tablename); }
        public virtual String Code(int Channel, List<String[]> Table, String TableName) { return Code(Channel, Table, TableName, this.Style); }

        /// <summary>
        /// Errechnet aus den Angaben den Code für den gewünschten HDMA FG Gradient Effeckt
        /// </summary>
        /// <param name="Channel">Der HDMA Channel, der benutzt werden soll</param>
        /// <param name="Table">Der Table, der im Code verwendet werden soll</param>
        /// <param name="TableName">Der Name des im Code verwendeten Tables. Es kann sowohl ein normaler, als auch um ein Sublabel verwendet werden.</param>
        /// <param name="Style">Der Style, der festlegt, wie die Color Math ausgeführt werden soll</param>
        /// <returns></returns>
        public static String Code(int Channel, List<String[]> Table, String TableName, ColorMathStyle Style)
        {
            int Base = 0x4330 + ((Channel - 3) * 0x10); //Base is die Basisadresse, also die $43x0, wobei x der Channel ist.
            int Mode = 0x00;                            // \
            if (Table[0].Length == 3)                   //  | Der Mode is bei normalen Tablen 00, aber bei vereinten tablen, die zwei werte pro scanline schreiben, brauchen wir Mode 02
                Mode = 0x02;                            // /

            //Für das Register werden die ersten zwei Zahlen weck-geschnitten werden, sodass nur die letzten zwei zahlen bleiben (21xx).
            //Die Zahlen werden um 8bit nach links verschoben und dazu wird der Modus addiert.
            //rrmm r=register m=mode
            String RegMode = (((Register % 0x100) << 8) + Mode).ToString("X4");

            // Der Layer Effekt (für die color math) is bei default 0x27. Wenn die ColorMath subtrahiert, statt addiert werden soll, wird noch das MSB gesetzt (bit 7)
            // Wenn das ergebnis halbiert weden soll, ungeachted ob add oder sub, dann muss noch Bit 6 gesetzt werden.
            // Der Enum lässt sich in int convertieren. Wenn additive, dann ist bit 0 gleich 0...
            // wenn halbiert, dann ist bit 3 gleich 1.
            int LayerEffect = 0x27 | (byte)Style;

            String CodeToReturn = INIT_Label +

                ";;=====================================================================================================================================\n" +
                "; Just a little not of importance:\n" +
                "; This code messes with the screens, in a way, that all the layers have a sort of higher proirity.\n" +
                "; What that basicly means is, that sprites which go behind layers will now go behind ALL the other layers. This includes Layer 2\n" +
                "; even if it is only used as normal background.\n " +
                "; Sprites for which this is would be the case are for example piranha plants or Mario when entering a pipe\n" +
                "; You have been warned\n"+
                ";;=====================================================================================================================================\n\n"+

                "LDA #$17\t\t;\\ Everything\n" +
	            "STA $212C\t\t; | is on main screen\n" +
	            "LDA #$00\t\t; | Nothing\n" +
	            "STA $212D\t\t; | is on sub screen\n" +
	            "LDA #$" + LayerEffect.ToString("X2") + "\t\t; | Affect layer 1,2,3\n" +
	            "STA $40\t\t\t;/ and back enable\n\n" +

                "REP #$20\t\t;\\\n" +
                "LDA #$" + RegMode + "\t\t; | Use Mode " + Mode.ToString("X") + " on register " + Register.ToString("X") + "\n" +
                "STA $" + Base.ToString("X") + "\t\t; | 43" + Channel + "0 = Mode, 43" + Channel + "1 = Register\n" +
                "LDA #" + "." + TableName.TrimStart('.') + "\t\t; | Address of HDMA table\n" +
                "STA $" + (Base + 2).ToString("X") + "\t\t; | 43" + Channel + "2 = Low-Byte of table, 43" + Channel + "3 = High-Byte of table\n" +
                "LDY.b #" + "." + TableName.TrimStart('.') + ">>16\t; | Address of HDMA table, get bank byte\n" +
                "STY $" + (Base + 4).ToString("X") + "\t\t; | 43" + Channel + "4 = Bank-Byte of table\n" +
                "SEP #$20\t\t;/\n\n" +

                "LDA #$" + (0x01 << Channel).ToString("X2") + "\t\t;\\\n" +
                "TSB $0D9F\t\t; |Enable HDMA on Channel " + Channel + "\n" +
                "RTS\t\t\t;/  Return\n";

            String TableCode = "";                      // \
            foreach (String[] SA in Table)              //  | Generieren des Table-codes.
            {                                           //  | Die Liste wird durchlaufen und jedes String Array darin ausgewertet
                TableCode += "\tdb " + SA[0];           //  | Es wird zuerst das 0te Element in die + einem tab mit darauffolgendem db Kommando geschrieben.
                for (int i = 1; i < SA.Length; i++)     //  | Das 0the elemtn enhält den Scanline Counter
                    TableCode += ", " + SA[i];          //  | Anschließend werden noch alle anderen elemente des Arrays hinzugefügt, und es wird ein ", " vorgestellt.
                TableCode += "\n";                      //  |
            }                                           //  | Abschließend wird der Table durch die Angabe von $00 Scanlinien beendet.
            TableCode += "\tdb $00";                    // /

            //Der Table wird nun noch an den Code angehängt. Inkusve des angegeben Tablenamens.
            //Handelt es sich bei dem Tablenamen um einen Sublabel (mit . beginnent), dann wird kein doppelpunkt angefängt.
            CodeToReturn += "\n\n" + "." + TableName.TrimStart('.') + "\n" + TableCode;
            
            return CodeToReturn;
        }
        /// <summary>
        /// Errechnet aus den Angaben den Code für den gewünschten HDMA FG Gradient Effeckt
        /// Der Effekt kann durch die benutzung unterschiedlicher FG_ColorGradient_HDMA unterschiedliche Farben und Längen aufweisen.
        /// Sollte sich herausstellen, dass weniger als 3 Channels benötigt werden, wird eine MessageBox angegebn und es öffnet sich ein Fenster, das zur angabe der Channels auffordert.
        /// </summary>
        /// <param name="HDMA1">Das erste Element, aus dem ein gemeinsammer Code errechnet werden soll</param>
        /// <param name="HDMA2">Das zweite Element, aus dem ein gemeinsammer Code errechnet werden soll</param>
        /// <param name="HDMA3">Das dritte Element, aus dem ein gemeinsammer Code errechnet werden soll</param>
        /// <returns></returns>
        public static String Code(FG_ColorGradient_HDMA HDMA1, FG_ColorGradient_HDMA HDMA2, FG_ColorGradient_HDMA HDMA3)
        {
            //Vordefinierte Variablen für den späteren gebrauch.
            int Channel1 = 3;
            String CodeToReturn = "";

            //Wenn auch nur eines der Elemente null ist, soll eine Exception ausgelöst werden.
            if (HDMA1 == null || HDMA2 == null || HDMA3 == null)
                throw new ArgumentNullException();

            //Wenn alle 3 Elemente keinen Tableinhalt haben, soll nichts zurückgegeben werden.
            if (HDMA1._tables.Count == 0 && HDMA2._tables.Count == 0 && HDMA3._tables.Count == 0)
                return "";

            //Wenn zwei der Table leer sind...
            else if (HDMA1._tables.Count == 0 && HDMA2._tables.Count == 0)
            {
                //Wird über eine MessageBox darauf hingewiesen, dass nur ein Channel benötigt wird.
                MessageBox.Show("The code only requires one HDMA Channel.\nPlease select the channel of your coice as the high-priority one.", "Saving Channels", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ChooseChannel CC = new ChooseChannel();   // \
                CC.ShowDialog();                        //  | Vordert user zur Angabe eines Channels auf und wertet diesen aus.
                Channel1 = CC.HighChannel;              // /

                CodeToReturn = HDMA3.Code(Channel1);    // Es wird einfach die Code methode des übrigen Objektes aufgerufen mit dem Angegeben Channel.
            }
            //Wenn zwei andere der 3 Table leer sind...
            else if (HDMA3._tables.Count == 0 && HDMA2._tables.Count == 0)
            {
                //Wird über eine MessageBox darauf hingewiesen, dass nur ein Channel benötigt wird.
                MessageBox.Show("The code only requires one HDMA Channel.\nPlease select the channel of your coice as the high-priority one.", "Saving Channels", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ChooseChannel CC = new ChooseChannel();   // \
                CC.ShowDialog();                        //  | Vordert user zur Angabe eines Channels auf und wertet diesen aus.
                Channel1 = CC.HighChannel;              // /

                CodeToReturn = HDMA1.Code(Channel1);    //Und es wird wieder die übrige Methode aufgerufen
            }
            //aller guten dinge sind 3
            else if (HDMA1._tables.Count == 0 && HDMA3._tables.Count == 0)
            {
                MessageBox.Show("The code only requires one HDMA Channel.\nPlease select the channel of your coice as the high-priority one.", "Saving Channels", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ChooseChannel CC = new ChooseChannel();
                CC.ShowDialog();
                Channel1 = CC.HighChannel;

                CodeToReturn = HDMA2.Code(Channel1);
            }

            else if (MergeHDMACode(HDMA1, HDMA2, HDMA3, ".GradRedGreen", ".GradBlue", out CodeToReturn)) // \
                return CodeToReturn;                                                                    //  | Probiert alle Kombinationen
            else if (MergeHDMACode(HDMA3, HDMA2, HDMA1, ".GradBlueGreen", ".GradRed", out CodeToReturn)) //  | von den Objekten aus,
                return CodeToReturn;                                                                    //  | ob sie vereint werden können
            else if (MergeHDMACode(HDMA1, HDMA3, HDMA2, ".GradRedBlue", ".GradRed", out CodeToReturn))   //  |
                return CodeToReturn;                                                                    // /

            // Wenn keine Vereinigung möglich ist und keine leeren Tabellen vorhanden
            else
            {
                int LayerEffect = 0x27 | (byte)HDMA1.Style;

                //Im gegensatz zu der anderen Methode, ist hier der Mode immer 0.
                String RegMode = ((Register % 0x100) << 8).ToString("X4");

                CodeToReturn = INIT_Label +

                ";=====================================================================================================================================\n" +
                "; Just a little not of importance:\n" +
                "; This code messes with the screens, in a way, that all the layers have a sort of higher prioirity.\n" +
                "; What that basicly means is, that sprites which go behind layers will now go behind ALL the other layers. This includes Layer 2\n" +
                "; even if it is only used as normal background.\n" +
                "; Sprites for which this is would be the case are for example piranha plants or Mario when entering a pipe\n" +
                "; You have been warned\n" +
                ";=====================================================================================================================================\n\n" +
                
                "LDA #$17\t\t;\\ Everything\n" +
	            "STA $212C\t\t; | is on main screen\n" +
	            "LDA #$00\t\t; | Nothing\n" +
	            "STA $212D\t\t; | is on sub screen\n" +
	            "LDA #$" + LayerEffect.ToString("X2") + "\t\t; | Affect layer 1,2,3\n" +
	            "STA $40\t\t\t;/ and back enable\n\n";

                int Base = 0x4330;
                String TableName = ".RedTable";

                CodeToReturn += "REP #$20\t\t;\\\n" +
                "LDA #$" + RegMode + "\t\t; | Use Mode 0 on register " + Register.ToString("X") + "\n" +
                "STA $" + Base.ToString("X") + "\t\t; | 4330 = Mode, 4331 = Register\n" +
                "LDA #" + TableName + "\t\t; | Address of HDMA table\n" +
                "STA $" + (Base + 2).ToString("X") + "\t\t; | 4332 = Low-Byte of table, 4333 = High-Byte of table\n" +
                "LDY.b #" + TableName + ">>16\t; | Address of HDMA table, get bank byte\n" +
                "STY $" + (Base + 4).ToString("X") + "\t\t; | 4334 = Bank-Byte of table\n\n";

                Base = 0x4340;
                TableName = ".GreenTable";

                CodeToReturn +=
                "LDA #$" + RegMode + "\t\t; | Use Mode 0 on register " + Register.ToString("X") + "\n" +
                "STA $" + Base.ToString("X") + "\t\t; | 4340 = Mode, 4341 = Register\n" +
                "LDA #" + TableName + "\t\t; | Address of HDMA table\n" +
                "STA $" + (Base + 2).ToString("X") + "\t\t; | 4342 = Low-Byte of table, 4343 = High-Byte of table\n" +
                "LDY.b #" + TableName + ">>16\t; | Address of HDMA table, get bank byte\n" +
                "STY $" + (Base + 4).ToString("X") + "\t\t; | 4344 = Bank-Byte of table\n\n";

                Base = 0x4350;
                TableName = ".BlueTable";

                CodeToReturn +=
                "LDA #$" + RegMode + "\t\t; | Use Mode 0 on register " + Register.ToString("X") + "\n" +
                "STA $" + Base.ToString("X") + "\t\t; | 4350 = Mode, 4351 = Register\n" +
                "LDA #" + TableName + "\t\t; | Address of HDMA table\n" +
                "STA $" + (Base + 2).ToString("X") + "\t\t; | 4352 = Low-Byte of table, 4353 = High-Byte of table\n" +
                "LDY.b #" + TableName + ">>16\t; | Address of HDMA table, get bank byte\n" +
                "STY $" + (Base + 4).ToString("X") + "\t\t; | 4354 = Bank-Byte of table\n" +
                "SEP #$20\t\t;/\n\n" +

                "LDA #$38\t\t;\\ Activate HDMA\n" +
                "TSB $0D9F\t\t; /on Channel 3,4 and 5\n" +
                "RTS\n\n";

                String CodeRed = HDMA1.Code(3,".RedTable");
                String CodeGreen = HDMA2.Code(4, ".GreenTable");
                String CodeBlue = HDMA3.Code(5, ".BlueTable");

                String RedTable = String.Join("\n", CodeRed.Split('\n'), 34, CodeRed.Split('\n').Length - 34);
                String GreenTable = String.Join("\n", CodeGreen.Split('\n'), 34, CodeGreen.Split('\n').Length - 34);
                String BlueTable = String.Join("\n", CodeBlue.Split('\n'), 34, CodeBlue.Split('\n').Length - 34);

                CodeToReturn += RedTable + "\n" + GreenTable + "\n" + BlueTable;
            }

            return CodeToReturn;
        }

        /// <summary>
        /// Funktion um zu überprüfen ob 2 der 3 FG_ColorGradient_HDMA Objekte ein einen gemeinsammen Table vereint werden können.
        /// Diese wenn möglich vereint und auch noch das 3 Object für den entgültigen Code berücksichtigt.
        /// </summary>
        /// <param name="Merge1">Das erste Objekt, dass auf eine Vereinigung überprüft werden soll</param>
        /// <param name="Merge2">Das zweite Objekt, dass auf eine Vereinigung überprüft werden soll</param>
        /// <param name="HDMA3">Das 3. FG_ColorGradient_HDMA Objekt, dass für den vollständigen Code herangezogen wird, aber nichts mit der Vereingigung zu tun hat.</param>
        /// <param name="Tablename_Merge">Der Name für den Table, des neuen, vereinten, Tables</param>
        /// <param name="Tablename_Single">Der Name für den Table des HDMA3 Objektes</param>
        /// <param name="CodeToReturn">Rückgabeparameter für den entgültigen code.</param>
        /// <returns>True, wenn HDMA1 und HDMA2 vereint werden können</returns>
        private static bool MergeHDMACode(FG_ColorGradient_HDMA Merge1, FG_ColorGradient_HDMA Merge2, FG_ColorGradient_HDMA HDMA3, String Tablename_Merge, String Tablename_Single, out String CodeToReturn)
        {
            List<String[]> NewTable;                //Variable für den Neuen Table
            if (Join(Merge1, Merge2, out NewTable)) //Vererbte Methode von HDMA classe. Überprüft auf vereinigung zweier HDMA objecte.
            {
                MessageBox.Show("It appears, that your HDMA can be generated without using all 3 channels.\n" +             // \
                    "You'd have to select a high-priority channel, which will be used if only one channel is needed " +     //  | MessageBox, die den User informiert, dass nicht alle Channels benötigt werden.
                    "and a low-priorety channel, in case two channels are needed.",                                         //  |
                    "Saving Channels", MessageBoxButtons.OK, MessageBoxIcon.Information);                                   // /

                ChooseChannel CC = new ChooseChannel();   // \
                CC.ShowDialog();                        //  | GUI um den User die auswahl eines (oder zwei) Channels zu ermöglichen
                int Channel1 = CC.HighChannel;          //  | Rückgabewerte werden gespeichert.
                int Channel2 = CC.LowChannel;           // /

                //PreCode wird mit dem Code der verbunden Tables und den angegen Channel sowie Tablenamen inizialisiert.
                String PreCode = Code(Channel1, NewTable, Tablename_Merge, Merge1.Style);

                if (HDMA3._tables.Count != 0) //Wenn das 3. Objekt leer ist, kann alles andere übersprungen werden.
                {
                    String FirstCode = String.Join("\n", PreCode.Split('\n'), 0, 27);                                   // \  Der code aus den vereinten tables wird aufgeteilt. Er beginnt von Anfang und geht bis vor das "SEP #$20"
                    String FirstTable = String.Join("\n", PreCode.Split('\n'), 34, PreCode.Split('\n').Length - 34);    //  | Danach wird aus dem code noch der Table extrahiert. Er beginnt bei dem Label und ended mit dem $00.
                    String NewCode = HDMA3.Code(Channel2, Tablename_Single);                                            //  | Der Code für das 3. Objekt wird berechnet.
                    String SecondCode = String.Join("\n", NewCode.Split('\n'), 21, 7);                                  //  | Genau wie zuvor, wird der Code gesplitet. Anders als FirstCode, enthält diese Variable aber weder den INITLabel, noch das REP, dafür aber das SEP.
                    String SecondTable = String.Join("\n", NewCode.Split('\n'), 34, NewCode.Split('\n').Length - 34);   // /  Für den zweiten Table gilt genau das gleiche wie für den ersten.

                    int ChannelToActivate = (0x01 << Channel1) + (0x01 << Channel2);                            // \  
                    String ActuvateChannelReturn = "LDA #$" + ChannelToActivate.ToString("X2") + "\t\t;\\\n" +  //  | Für die Aktivierung der Channels, muss ausgewerted werden, welche Channels ausgewählt wurden.
                        "TSB $0D9F\t\t; |Enable HDMA on Channel " + Channel1 + " and " + Channel2 + "\n" +      //  | Die Auswertung wird dann in den Code eingebaut und noch ein RTS angefügt.
                        "RTS\t\t\t;/  Return\n";                                                                // /  

                    //Der Entgältige Code ist dann einfach eine Zusammensetzung aus allem
                    CodeToReturn = (FirstCode + "\n" + SecondCode + "\n" + ActuvateChannelReturn + "\n" + SecondTable + "\n\n" + FirstTable);
                }
                else
                    CodeToReturn = PreCode;

                return true;
            }

            CodeToReturn = "";
            return false;
        }
        

        /// <summary>
        /// Setzt die Style Variable im Object entsprechend des gesetzten RadioButtons
        /// </summary>
        /// <param name="Add">Der RadioButton der gesetzt is wenn die Color Math additive ausgeführt werden soll</param>
        /// <param name="Sub">Der RadioButton der gesetzt is wenn die Color Math diverenzive ausgeführt werden soll</param>
        /// <param name="HalfAdd">Der RadioButton der gesetzt is wenn die Color Math additive und halbiert ausgeführt werden soll</param>
        /// <param name="HalfSub">Der RadioButton der gesetzt is wenn die Color Math diverenzive und halbiert ausgeführt werden soll</param>
        public virtual void SetStyle(RadioButton Add, RadioButton Sub, RadioButton HalfAdd, RadioButton HalfSub)
        {
            if (Add.Checked)
                this.Style = ColorMathStyle.Addiditve;
            else if (Sub.Checked)
                this.Style = ColorMathStyle.Substracting;
            else if (HalfAdd.Checked)
                this.Style = ColorMathStyle.AdditiveHalf;
            else
                this.Style = ColorMathStyle.SubstractingHalf;
        }

        /// <summary>
        /// Funktion zeichnet den Effekt und berechnet den internen Table
        /// </summary>
        /// <param name="ImageToOverwrite">Das Bild, auf das der Effekt gezeichnet werden soll</param>
        /// <param name="UseRed">Gibt an, ob der Effekt einen Rotanteil hat</param>
        /// <param name="UseGreen">Gibt an, ob der Effekt einen Grünanteil hat</param>
        /// <param name="UseBlue">Gibt an, ob der Effekt einen Blauanteil hat</param>
        /// <returns>Die Bitmap mit dem gezeichneten Effekt</returns>
        public virtual Bitmap Draw(Bitmap ImageToOverwrite, bool UseRed, bool UseGreen, bool UseBlue)
        {
            if (!UseRed && !UseGreen && !UseBlue)
                return ImageToOverwrite;
            Farbe ColorToUse = (UseRed ? Farbe.Red : (Farbe)0x00) | (UseGreen ? Farbe.Green : (Farbe)0x00) | (UseBlue ? Farbe.Blue : (Farbe)0x00);
            return Draw(ImageToOverwrite, ColorToUse);
        }
        /// <summary>
        /// Funktion zeichnet den Effekt und berechnet den internen Table
        /// </summary>
        /// <param name="ImageToOverwrite">Das Bild, auf das der Effekt gezeichnet werden soll</param>
        /// <param name="ColorToUse">Die Farben, die im Bild verwendet werden sollen</param>
        /// <returns>Die Bitmap mit dem gezeichneten Effekt</returns>
        public Bitmap Draw(Bitmap ImageToOverwrite, Farbe ColorToUse)
        {
            Bitmap Ret;
            if (((int)Style & 0x40) != 0)
            {
                ColorMathStyle Save = Style;
                Style = ColorMathStyle.Addiditve; // (Style == ColorMathStyle.AdditiveHalf) ? ColorMathStyle.Addiditve : ColorMathStyle.Substracting;

                Bitmap Empty = DrawBottom(DrawTop(new Bitmap(ImageToOverwrite.Width, Scanlines), this.TopValue, ColorToUse), this.BottomValue, ColorToUse);
                if (Save == ColorMathStyle.AdditiveHalf)
                    Ret = HalfAddImages(ImageToOverwrite, Empty);
                else
                    Ret = HalfSubImages(ImageToOverwrite, Empty);
                Style = Save;
            }
            else
                Ret = DrawBottom(DrawTop(ImageToOverwrite, this.TopValue, ColorToUse), this.BottomValue, ColorToUse);
            TableCalculator.GetTable(TableTop, TableBottom, ColorToUse, ref this._tables);

            return Ret;
        }
        /// <summary>
        /// Funktion zeichnet den Effekt und berechnet den internen Table
        /// </summary>
        /// <param name="ImageToOverwrite">Das Bild, auf das der Effekt gezeichnet werden soll</param>
        /// <returns>Die Bitmap mit dem gezeichneten Effekt</returns>
        public virtual Bitmap Draw(Bitmap ImageToOverwrite) { return Draw(ImageToOverwrite, this.ColorToUse); }

        public override Bitmap Draw(List<string[]> Table)
        {
            Bitmap Ret = new Bitmap(256, Scanlines);
            int ScanPointer = 0;

            foreach (String[] SA in Table)
            {
                int Scan = Convert.ToInt32(SA[0].Trim('$'), 16);
                int Col = Convert.ToInt32(SA[1].Trim('$'), 16);
                
                Col = (Col & 0x1F) * 8;
                int Red = ((int)ColorToUse & (int)Farbe.Red) == 0 ? 0 : Col;
                int Green = ((int)ColorToUse & (int)Farbe.Green) == 0 ? 0 : Col;
                int Blue = ((int)ColorToUse & (int)Farbe.Blue) == 0 ? 0 : Col;

                for(int y = 0; y < Scan && y + ScanPointer < Scanlines; y++)
                    for(int x = 0; x < Ret.Width; x++)
                        Ret.SetPixel(x, y + ScanPointer, Color.FromArgb(Red,Green,Blue));

                ScanPointer += Scan;

                if (ScanPointer >= Scanlines)
                    break;
            }

            return Ret;
        }
              
        public static Bitmap Draw(Bitmap ImageToOverwrite, FG_ColorGradient_HDMA Red, FG_ColorGradient_HDMA Green, FG_ColorGradient_HDMA Blue)
        {
            Bitmap Ret;
            if (((int)Red.Style & 0x40) != 0)
            {
                ColorMathStyle Save = Red.Style;
                //ColorMathStyle Style = (Save == ColorMathStyle.AdditiveHalf) ? ColorMathStyle.Addiditve : ColorMathStyle.Substracting;
                Red.Style = ColorMathStyle.Addiditve;
                Green.Style = ColorMathStyle.Addiditve;
                Blue.Style = ColorMathStyle.Addiditve;

                if (Save == ColorMathStyle.AdditiveHalf)
                {
                    Bitmap Ret2 = Red.Draw(Green.Draw(Blue.Draw(new Bitmap(256, Scanlines))));
                    Ret = HalfAddImages(Ret2, ImageToOverwrite);
                }
                else
                {
                    Bitmap Ret2 = Red.Draw(Green.Draw(Blue.Draw(new Bitmap(256, Scanlines))));
                    //Ret2.Save("white.png");
                    Ret = HalfSubImages(ImageToOverwrite, Ret2);
                }
                Red.Style = Save;
                Green.Style = Save;
                Blue.Style = Save;
            }
            else
                Ret = Red.Draw(Green.Draw(Blue.Draw(ImageToOverwrite)));

            return Ret;
        }
        
        /// <summary>
        /// Fügt den oberen Bereich des zu Zeichnenten Gradients ein. Der Effekt wird versucht über die angegben Reichweite zu zeichnen.
        /// Bedingt durch das Verhältniss von Reichweite zu Anfangsfabre, kann nicht immer die volle Reichweite ausgefüllt werden.
        /// Desweiteren erstellt die Methode ein Table-Element, dass zusammen mit dem DrawBottom den nötigen Table für den Code ergiebt.
        /// </summary>
        /// <param name="ImageToOverwrite">Das Bild, auf das der Effekt gezeichnet werden soll</param>
        /// <param name="TopValue8bit">Der Anfangswert der Fabre in 8bit (0-255)</param>
        /// <param name="ColorToUse">Die Farbe(n) die für den Effekt benutzt werden.</param>
        /// <returns>Die überarbeitete Bitmap mit dem gezeichneten Effekt</returns>
        private Bitmap DrawTop(Bitmap ImageToOverwrite, int TopValue8bit, Farbe ColorToUse)
        {
            int TopValue8BitStorage = TopValue8bit;
            Color TopColor = Color.FromArgb(                            // \
                ((ColorToUse & Farbe.Red) == 0) ? 0 : TopValue8bit,     //  | Erstellt eine Farbinstanz, die den übergabeparametern entspricht.
                ((ColorToUse & Farbe.Green) == 0) ? 0 : TopValue8bit,   //  | Die Farbtiefe ist immer gleich, und ColorToUse entscheidet ob R,G und B verwendet werden.
                ((ColorToUse & Farbe.Blue) == 0) ? 0 : TopValue8bit);   // /

            int R_5bit, G_5bit, B_5bit; // \
            R_5bit = TopColor.R / 8;    //  | Für manche berechnungen wird der 5Bit wert der Farben gebraucht.
            G_5bit = TopColor.G / 8;    //  | 
            B_5bit = TopColor.B / 8;    // /

            //Die Anzahl der Farbspalten.
            //Gibt an wie viele Spalten für den Effekt benötigt werden.
            //Wenn z.B. der Rote 5bit wert bei 31 liegt (maximum), dann werden auch 31 spalten benötigt.
            int Maximal = new int[3] { R_5bit, G_5bit, B_5bit }.Max();

            //Wenn die Pixel-Reichweite geringer ist als die anzahl der benötigten Spalten, soll kein Effekt gezeichnet werden. (Schließlich gehts sich das nicht aus)
            //Genauso braucht auch kein Effekt errechnet zu werden, wenn die Raichweite 0 oder der höchste Anfangswert 0 ist.
            if (TopReach < Maximal || TopReach == 0 || Maximal == 0)
            {
                TableTop = new TableCalculator(0, TopReach);
                return ImageToOverwrite;
            }

            //Angabe wie hoch eine Spalte des Effekts ist.
            int Height_Row = TopReach / Maximal;

            TableTop = new TableCalculator(Height_Row, TopReach);

            int Red_8bit = TopColor.R;      // \
            int Green_8bit = TopColor.G;    //  | Für wieder andere Berechnungen werden die 8bit Werte (0-255) benötigt.
            int Blue_8bit = TopColor.B;     // /

            Bitmap NewBitmap = new Bitmap(ImageToOverwrite); //Die Bitmap, in der das neue Bild + Gradiant gezeichnet wird.

            //Es werden 2 int Variablen iniziallisiert.
            //      y ist der Schleifenzähler und außerdem der Index für die y-Achse des Bildes.
            //      cntCol ist ein Zähler der bei jedem Durchlauf erhöht wird.
            //      Er wird benutzt um festzustellen, ob die Spaltenbreite erreicht wurde und es zeit ist, die Farbe einen Ton tiefer zu setzen.
            for (int y = 0, cntCol = 0; y < TopReach; y++, cntCol++)
            {
                for (int x = 0; x < ImageToOverwrite.Width; x++)//Die verschachtelte Schleife, die dafür sorgt, 
                                                                //dass alles für die ganze Zeile der momentanen Spalte des Bildes ausgeführt wird
                {
                    int NewR, NewG, NewB;
                    if (Style == ColorMathStyle.Addiditve)
                    {
                        NewR = ImageToOverwrite.GetPixel(x, y).R + Red_8bit;   // \  Die neuen Farbwert werden errechnet, aus dem 
                        NewG = ImageToOverwrite.GetPixel(x, y).G + Green_8bit; //  | Farbwerten des Bildes + der Angegebenen Farbe
                        NewB = ImageToOverwrite.GetPixel(x, y).B + Blue_8bit;  // /  Die Angegeben Farbe wird jede Spalte um einen Ton dunkler.

                        //Das entsprechende Pixel wird anschließend mit den neuen Farben eingefärbt.
                        //Eine abfrage stellt sicher, dass ungeachtet der Rechnung keine Werte größer 255 eingetragen werden.
                        NewBitmap.SetPixel(x, y, Color.FromArgb((NewR > 255) ? 255 : NewR, (NewG > 255) ? 255 : NewG, (NewB > 255) ? 255 : NewB));
                    }
                    else if (Style == ColorMathStyle.Substracting)
                    {
                        NewR = ImageToOverwrite.GetPixel(x, y).R - Red_8bit;    // \  Die neuen Farbwert werden errechnet, aus dem 
                        NewG = ImageToOverwrite.GetPixel(x, y).G - Green_8bit;  //  | Farbwerten des Bildes - der Angegebenen Farbe
                        NewB = ImageToOverwrite.GetPixel(x, y).B - Blue_8bit;   // /  Die Angegeben Farbe wird jede Spalte um einen Ton dunkler.

                        //Das entsprechende Pixel wird anschließend mit den neuen Farben eingefärbt.
                        //Eine abfrage stellt sicher, dass ungeachtet der Rechnung keine Werte kleiner als 0 eingetragen werden.
                        NewBitmap.SetPixel(x, y, Color.FromArgb((NewR < 0) ? 0 : NewR, (NewG < 0) ? 0 : NewG, (NewB < 0) ? 0 : NewB));
                    }
                    else if (Style == ColorMathStyle.AdditiveHalf)
                    {
                        NewR = (ImageToOverwrite.GetPixel(x, y).R + Red_8bit) / ((ColorToUse == Farbe.Red || !PartOfTotal) ? 2 : 1);     // \  Die neuen Farbwert werden errechnet, aus dem 
                        NewG = (ImageToOverwrite.GetPixel(x, y).G + Green_8bit) / ((ColorToUse == Farbe.Green || !PartOfTotal) ? 2 : 1); //  | Farbwerten des Bildes + der Angegebenen Farbe durch 2
                        NewB = (ImageToOverwrite.GetPixel(x, y).B + Blue_8bit) / ((ColorToUse == Farbe.Blue || !PartOfTotal) ? 2 : 1);   // /  Die Angegeben Farbe wird jede Spalte um einen Ton dunkler.

                        //Das entsprechende Pixel wird anschließend mit den neuen Farben eingefärbt.
                        //Eine abfrage stellt sicher, dass ungeachtet der Rechnung keine Werte größer 255 eingetragen werden.
                        NewBitmap.SetPixel(x, y, Color.FromArgb((NewR > 255) ? 255 : NewR, (NewG > 255) ? 255 : NewG, (NewB > 255) ? 255 : NewB));
                    }
                    else if (Style == ColorMathStyle.SubstractingHalf)
                    {
                        NewR = (ImageToOverwrite.GetPixel(x, y).R - Red_8bit) / 2;    // \  Die neuen Farbwert werden errechnet, aus dem 
                        NewG = (ImageToOverwrite.GetPixel(x, y).G - Green_8bit) / 2;  //  | Farbwerten des Bildes - der Angegebenen Farbe durch 2
                        NewB = (ImageToOverwrite.GetPixel(x, y).B - Blue_8bit) / 2;   // /  Die Angegeben Farbe wird jede Spalte um einen Ton dunkler.

                        //Das entsprechende Pixel wird anschließend mit den neuen Farben eingefärbt.
                        //Eine abfrage stellt sicher, dass ungeachtet der Rechnung keine Werte kleiner als 0 eingetragen werden.
                        NewBitmap.SetPixel(x, y, Color.FromArgb((NewR < 0) ? 0 : NewR, (NewG < 0) ? 0 : NewG, (NewB < 0) ? 0 : NewB));
                    }
                }

                //Die Abrage überprüft, ob der Zähler der entsprechenden Farbe bereits die entsprechende Spaltenhöhe erreicht hat.
                //Falls ja, wird die Farbe einen Ton tiefer gestellt, der Schelifenzähler rückgesetzt, und ein Eintrag im Table gemacht.

                if (cntCol == Height_Row && TopValue8BitStorage != 0)
                {
                    if (Red_8bit != 0) Red_8bit -= 8;
                    if (Green_8bit != 0) Green_8bit -= 8;
                    if (Blue_8bit != 0) Blue_8bit -= 8;

                    if (Red_8bit < 0) Red_8bit = 0;
                    if (Green_8bit < 0) Green_8bit = 0;
                    if (Blue_8bit < 0) Blue_8bit = 0;

                    cntCol = 0;
                    TableTop.Add(TopValue8BitStorage / 8);
                    TopValue8BitStorage -= 8;
                }
            }            
            return NewBitmap;
        }
        /// <summary>
        /// Fügt den unteren Bereich des zu Zeichnenten Gradients ein. Der Effekt wird versucht über die angegben Reichweite zu zeichnen.
        /// Bedingt durch das Verhältniss von Reichweite zu Anfangsfabre, kann nicht immer die volle Reichweite ausgefüllt werden.
        /// Desweiteren erstellt die Methode ein Table-Element, dass zusammen mit dem DrawTop den nötigen Table für den Code ergiebt.
        /// </summary>
        /// <param name="ImageToOverwrite">Das Bild, auf das der Effekt gezeichnet werden soll</param>
        /// <param name="BottomValue8bit">Der Anfangswert der Fabre in 8bit (0-255)</param>
        /// <param name="ColorToUse">Die Farbe(n) die für den Effekt benutzt werden.</param>
        /// <returns>Die überarbeitete Bitmap mit dem gezeichneten Effekt</returns>
        private Bitmap DrawBottom(Bitmap ImageToOverwrite, int BottomValue8bit, Farbe ColorToUse)
        {
            int BottomValue8BitStorage = BottomValue8bit;
            Color BottomColor = Color.FromArgb(                            // \
                ((ColorToUse & Farbe.Red) == 0) ? 0 : BottomValue8bit,     //  | Erstellt eine Farbinstanz, die den übergabeparametern entspricht.
                ((ColorToUse & Farbe.Green) == 0) ? 0 : BottomValue8bit,   //  | Die Farbtiefe ist immer gleich, und ColorToUse entscheidet ob R,G und B verwendet werden.
                ((ColorToUse & Farbe.Blue) == 0) ? 0 : BottomValue8bit);   // /

            int R_5bit, G_5bit, B_5bit;     // \
            R_5bit = BottomColor.R / 8;     //  | Manche Berechnungen brauchen die 5Bit Werte.
            G_5bit = BottomColor.G / 8;     //  | 
            B_5bit = BottomColor.B / 8;     // /

            //Die Anzahl der Farbspalten.
            //Gibt an wie viele Spalten für den Effekt benötigt werden.
            //Wenn z.B. der Rote 5bit wert bei 31 liegt (maximum), dann werden auch 31 spalten benötigt.
            int Maximal = new int[3] { R_5bit, G_5bit, B_5bit }.Max();

            //Wenn die Pixel-Reichweite geringer ist als die anzahl der benötigten Spalten, soll kein Effekt gezeichnet werden. (Schließlich gehts sich das nicht aus)
            //Genauso braucht auch kein Effekt errechnet zu werden, wenn die Raichweite 0 oder der höchste Anfangswert 0 ist.
            if (BottomReach < Maximal || BottomReach == 0 || Maximal == 0)
            {
                TableBottom = new TableCalculator(0, BottomReach);
                return ImageToOverwrite;
            }

            //Die Höhe einer einzelnen Spalte in Pixel
            int Height_Row = BottomReach / Maximal;

            int Red_8bit = BottomColor.R;   // \
            int Green_8bit = BottomColor.G; //  | Für manche Rechnungen und die Farbsetzung werden die 8bit werte benötigt.
            int Blue_8bit = BottomColor.B;  // /

            //Instanziert den Table und der neuen Bitmap.
            TableBottom = new TableCalculator(Height_Row, BottomReach);
            Bitmap NewBitmap = new Bitmap(ImageToOverwrite);
            
            //Es werden 2 int Variablen iniziallisiert.
            //      y ist der Schleifenzähler und außerdem der Index für die y-Achse des Bildes.
            //      cntCol ist ein Zähler der bei jedem Durchlauf erhöht wird.
            //      Er wird benutzt um festzustellen, ob die Spaltenbreite erreicht wurde und es zeit ist, die Farbe einen Ton tiefer zu setzen.
            for (int y = 0, cntCol = 0; y < BottomReach; y++, cntCol++)
            {
                //Die verschachtelte Schleife stellt sicher, dass jedes Pixel der Zeile (x-Achse) bearbeitet wird.
                for (int x = 0; x < ImageToOverwrite.Width; x++)
                {
                    int NewR, NewG, NewB;
                    if (Style == ColorMathStyle.Addiditve)
                    {
                        //Da der Schleifenzähler für die y-Achse positive zählt, muss der Wert von der Gesamtanzahl der Scanlinien abgezogen werden.
                        //Da der Zähler außerdem nur die Werte 0-223 durchläuft, muss noch 1 abgezogen werden, dadurch geht der Index von 223-0
                        NewR = ImageToOverwrite.GetPixel(x, Scanlines - y - 1).R + Red_8bit;    // \
                        NewG = ImageToOverwrite.GetPixel(x, Scanlines - y - 1).G + Green_8bit;  //  | Der neue Farbwert ist der alte + der der Aktuellen Farbe
                        NewB = ImageToOverwrite.GetPixel(x, Scanlines - y - 1).B + Blue_8bit;   // /

                        //Einfärben der ensprechenden Pixel auf der neuen Bitmap. Zusaätzlich wird sichergestellt, dass keine Werte über 255 eingetragen werden.
                        NewBitmap.SetPixel(x, Scanlines - y - 1, Color.FromArgb((NewR > 255) ? 255 : NewR, (NewG > 255) ? 255 : NewG, (NewB > 255) ? 255 : NewB));
                    }
                    else if (Style == ColorMathStyle.Substracting)
                    {
                        NewR = ImageToOverwrite.GetPixel(x, Scanlines - y - 1).R - Red_8bit;    // \
                        NewG = ImageToOverwrite.GetPixel(x, Scanlines - y - 1).G - Green_8bit;  //  | Der neue Farbwert ist der alte - der der Aktuellen Farbe
                        NewB = ImageToOverwrite.GetPixel(x, Scanlines - y - 1).B - Blue_8bit;   // /

                        //Einfärben der ensprechenden Pixel auf der neuen Bitmap. Zusaätzlich wird sichergestellt, dass keine Werte unter 0 eingetragen werden.
                        NewBitmap.SetPixel(x, Scanlines - y - 1, Color.FromArgb((NewR < 0) ? 0 : NewR, (NewG < 0) ? 0 : NewG, (NewB < 0) ? 0 : NewB));
                    }
                    else if (Style == ColorMathStyle.AdditiveHalf)
                    {
                        //Da der Schleifenzähler für die y-Achse positive zählt, muss der Wert von der Gesamtanzahl der Scanlinien abgezogen werden.
                        //Da der Zähler außerdem nur die Werte 0-223 durchläuft, muss noch 1 abgezogen werden, dadurch geht der Index von 223-0
                        NewR = (ImageToOverwrite.GetPixel(x, Scanlines - y - 1).R + Red_8bit) / (ColorToUse == Farbe.Red ? 2 : 1);      // \
                        NewG = (ImageToOverwrite.GetPixel(x, Scanlines - y - 1).G + Green_8bit) / (ColorToUse == Farbe.Green ? 2 : 1);  //  | Der neue Farbwert ist der alte + der der Aktuellen Farbe durch 2
                        NewB = (ImageToOverwrite.GetPixel(x, Scanlines - y - 1).B + Blue_8bit) / (ColorToUse == Farbe.Blue ? 2 : 1);    // /

                        //Einfärben der ensprechenden Pixel auf der neuen Bitmap. Zusaätzlich wird sichergestellt, dass keine Werte über 255 eingetragen werden.
                        NewBitmap.SetPixel(x, Scanlines - y - 1, Color.FromArgb((NewR > 255) ? 255 : NewR, (NewG > 255) ? 255 : NewG, (NewB > 255) ? 255 : NewB));
                    }
                    else if (Style == ColorMathStyle.SubstractingHalf)
                    {
                        NewR = (ImageToOverwrite.GetPixel(x, Scanlines - y - 1).R - Red_8bit) / 2;    // \
                        NewG = (ImageToOverwrite.GetPixel(x, Scanlines - y - 1).G - Green_8bit) / 2;  //  | Der neue Farbwert ist der alte - der der Aktuellen Farbe druch 2
                        NewB = (ImageToOverwrite.GetPixel(x, Scanlines - y - 1).B - Blue_8bit) / 2;   // /

                        //Einfärben der ensprechenden Pixel auf der neuen Bitmap. Zusaätzlich wird sichergestellt, dass keine Werte unter 0 eingetragen werden.
                        NewBitmap.SetPixel(x, Scanlines - y - 1, Color.FromArgb((NewR < 0) ? 0 : NewR, (NewG < 0) ? 0 : NewG, (NewB < 0) ? 0 : NewB));
                    }
                }

                if (cntCol == Height_Row && BottomValue8BitStorage != 0)
                {
                    if (Red_8bit != 0) Red_8bit -= 8;
                    if (Green_8bit != 0) Green_8bit -= 8;
                    if (Blue_8bit != 0) Blue_8bit -= 8;

                    if (Red_8bit < 0) Red_8bit = 0;
                    if (Green_8bit < 0) Green_8bit = 0;
                    if (Blue_8bit < 0) Blue_8bit = 0;

                    cntCol = 0;
                    TableBottom.Add(BottomValue8BitStorage / 8);
                    BottomValue8BitStorage -= 8;
                }
            }

            TableBottom.Reverse();  // Der Table muss zum Schluss noch ugedreht werden, 
                                    // da die Werte in der richtigen reihenfolge eingetragen wurden
                                    // aber in der verkehrten benötigt werden

            return NewBitmap;
        }

        public static Bitmap Merge(Bitmap Red, Bitmap Green, Bitmap Blue)
        {
            Bitmap Ret = new Bitmap(Red);

            for (int x = 0; x < Ret.Width; x++)
                for (int y = 0; y < Ret.Height; y++)
                    Ret.SetPixel(x, y, Color.FromArgb(
                        Red.GetPixel(x, y).R,
                        Green.GetPixel(x, y).G,
                        Blue.GetPixel(x, y).B));

            return Ret;
        }

        public static Bitmap Merge(Bitmap ImageToOverwrite, Bitmap ImageToAdd, ColorMathStyle Style)
        {
            Bitmap Ret = new Bitmap(ImageToOverwrite);
            if (Style == ColorMathStyle.Addiditve)
            {
                for (int x = 0; x < Ret.Width; x++)
                    for (int y = 0; y < Ret.Height; y++)
                    {
                        int Red = ImageToOverwrite.GetPixel(x, y).R + ImageToAdd.GetPixel(x, y).R;
                        int Green = ImageToOverwrite.GetPixel(x, y).G + ImageToAdd.GetPixel(x, y).G;
                        int Blue = ImageToOverwrite.GetPixel(x, y).B + ImageToAdd.GetPixel(x, y).B;
                        Ret.SetPixel(x, y, Color.FromArgb(Red.Max(255), Green.Max(255), Blue.Max(255)));
                    }
                return Ret;
            }
            if (Style == ColorMathStyle.Substracting)
            {
                for (int x = 0; x < Ret.Width; x++)
                    for (int y = 0; y < Ret.Height; y++)
                    {
                        int Red = ImageToOverwrite.GetPixel(x, y).R - ImageToAdd.GetPixel(x, y).R;
                        int Green = ImageToOverwrite.GetPixel(x, y).G - ImageToAdd.GetPixel(x, y).G;
                        int Blue = ImageToOverwrite.GetPixel(x, y).B - ImageToAdd.GetPixel(x, y).B;
                        Ret.SetPixel(x, y, Color.FromArgb(Red.Min(0), Green.Min(0), Blue.Min(0)));
                    }
                return Ret;
            }
            if (Style == ColorMathStyle.AdditiveHalf)
                return HalfAddImages(ImageToOverwrite, ImageToAdd);
            if (Style == ColorMathStyle.SubstractingHalf)
                return HalfSubImages(ImageToOverwrite, ImageToAdd);
            return Ret;
        }

        private static Bitmap HalfAddImages(Bitmap First, Bitmap Second)
        {
            Bitmap Ret = new Bitmap(Second.Width, Second.Height);
            for(int x = 0; x < Ret.Width; x++)
                for (int y = 0; y < Ret.Height; y++)
                {
                    int Red = (First.GetPixel(x, y).R + Second.GetPixel(x, y).R) / 2;
                    int Green = (First.GetPixel(x, y).G + Second.GetPixel(x, y).G) / 2;
                    int Blue = (First.GetPixel(x, y).B + Second.GetPixel(x, y).B) / 2;
                    Ret.SetPixel(x, y, Color.FromArgb(Red, Green, Blue));
                }
            return Ret;
        }
        private static Bitmap HalfSubImages(Bitmap First, Bitmap Second)
        {
            Bitmap Ret = new Bitmap(First.Width, First.Height);
            for (int x = 0; x < Ret.Width; x++)
                for (int y = 0; y < Ret.Height; y++)
                {
                    int Red = (First.GetPixel(x, y).R - Second.GetPixel(x, y).R) / 2;
                    int Green = (First.GetPixel(x, y).G - Second.GetPixel(x, y).G) / 2;
                    int Blue = (First.GetPixel(x, y).B - Second.GetPixel(x, y).B) / 2;
                    Ret.SetPixel(x, y, Color.FromArgb(Red.Min(0), Green.Min(0), Blue.Min(0)));
                }
            return Ret;
        }
    }

    class FG_FullColorGradient_HDMA : FG_ColorGradient_HDMA
    {
        public FG_ColorGradient_HDMA Red;
        public FG_ColorGradient_HDMA Green;
        public FG_ColorGradient_HDMA Blue;
        
        new public int TopReach
        {
            get { return Red.TopReach; }
            set
            {
                Red.TopReach = value;
                Green.TopReach = value;
                Blue.TopReach = value;
            }
        }
        new public int BottomReach
        {
            get { return Red.BottomReach; }
            set
            {
                Red.BottomReach = value;
                Green.BottomReach = value;
                Blue.BottomReach = value;
            }
        }
        new public ColorMathStyle Style
        {
            get { return Red.Style; }
            set
            {
                Red.Style = value;
                Green.Style = value;
                Blue.Style = value;
            }
        }
        new public Color TopValue
        {
            get 
            {
                return Color.FromArgb(
                    Red.TopValue,
                    Green.TopValue,
                    Blue.TopValue);
            }
            set
            {
                Red.TopValue = value.R;
                Green.TopValue = value.G;
                Blue.TopValue = value.B;
            }
        }
        new public Color BottomValue
        {
            get
            {
                return Color.FromArgb(
                    Red.BottomValue,
                    Green.BottomValue,
                    Blue.BottomValue);
            }
            set
            {
                Red.BottomValue = value.R;
                Green.BottomValue = value.G;
                Blue.BottomValue = value.B;
            }
        }
        new public Bitmap BaseImage
        {
            get { return Red.BaseImage; }
            set
            {
                Red.BaseImage = value;
                Green.BaseImage = value;
                Blue.BaseImage = value;
            }
        }

        public FG_FullColorGradient_HDMA()
        {
            Red = new FG_ColorGradient_HDMA(Farbe.Red);
            Green = new FG_ColorGradient_HDMA(Farbe.Green);
            Blue = new FG_ColorGradient_HDMA(Farbe.Blue);
        }

        public override Bitmap Draw(Bitmap ImageToOverwrite) { return Draw(ImageToOverwrite, Red, Green, Blue); }
        public override void SetStyle(RadioButton Add, RadioButton Sub, RadioButton HalfAdd, RadioButton HalfSub)
        {
            Red.SetStyle(Add, Sub, HalfAdd, HalfSub);
            Green.SetStyle(Add, Sub, HalfAdd, HalfSub);
            Blue.SetStyle(Add, Sub, HalfAdd, HalfSub);
        }
        public override String Code()
        {
            return FG_ColorGradient_HDMA.Code(Red, Green, Blue);
        }
    }
}
