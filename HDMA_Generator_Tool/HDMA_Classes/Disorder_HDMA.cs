using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading;
using System.Diagnostics;

namespace HDMA_Generator_Tool
{
    enum Orientation { Left = 0x01, Right = 0x03, Up = 0x02, Down = 0x04 };
    enum LayerRegister { Layer1_X = 0x210D, Layer1_Y = 0x210E, Layer2_X = 0x210F, Layer2_Y = 0x2110 };

    /// <summary>
    /// Veröffentlicht die wesentlichen Methoden für HDMA Effekte die das Bild verschieben.
    /// </summary>
    abstract class Disorder_HDMA : HDMA
    {
        public const int Mode = 0x02;
        public const int Layer1_XPosition = 0x1462;
        public const int Layer1_YPosition = 0x1464;
        public const int Layer2_XPosition = 0x1466;
        public const int Layer2_YPosition = 0x1468;

        /// <summary>
        /// Die unbenütze RAM Adresse ab der der Table eingefügt wird.
        /// </summary>
        public int FreeRAM
        {
            get { return _FreeRAM; }
            set
            {
                if (value < 0x7E0000 || value > 0x7FFFFF)
                    throw new InvalidOperationException("The given address is not RAM");
                else
                    _FreeRAM = value;
            }
        }
        private int _FreeRAM = 0x7F9E00;

        protected static Dictionary<Orientation, Orientation> Invert = new Dictionary<Orientation, Orientation>()
        {
            { Orientation.Left, Orientation.Right },
            { Orientation.Right, Orientation.Left },
            { Orientation.Up, Orientation.Down },
            { Orientation.Down, Orientation.Up }
        };
        
        /// <summary>
        /// Rotiert einen bestimmten Bereich einer Bitmap nach links und 
        /// </summary>
        /// <param name="InitHeight">Die Y Höhe ab der die Zeilen verschoben werden</param>
        /// <param name="Bandwidth">Wie viele Zeilen verschoben werden sollen</param>
        /// <param name="Moving">Um wie viele Pixel die Bitmap nach links verschoben werden soll</param>
        /// <param name="ImageToOverwrite">Die Bitmap die verschoben werden soll</param>
        /// <returns>Die neue Bitmap mit der entsprechend verschobenen Linien</returns>
        public static Bitmap MoveLine(int InitHeight, int Bandwidth, int Moving, Bitmap ImageToOverwrite)
        { return MoveLine(InitHeight, Bandwidth, Moving, ImageToOverwrite, Orientation.Left); }
        /// <summary>
        /// Rotiert einen bestimmten Bereich einer Bitmap nach links und 
        /// </summary>
        /// <param name="InitHeight">Die Y Höhe ab der die Zeilen verschoben werden</param>
        /// <param name="Bandwidth">Wie viele Zeilen verschoben werden sollen</param>
        /// <param name="Moving">Um wie viele Pixel die Bitmap nach links verschoben werden soll</param>
        /// <param name="ImageToOverwrite">Die Bitmap die verschoben werden soll</param>
        /// <param name="Direction">Die Richtung in die sich der ausgewählte Teil bewegen soll</param>
        /// <returns>Die neue Bitmap mit der entsprechend verschobenen Linien</returns>
        public static Bitmap MoveLine(int InitHeight, int Bandwidth, int Moving, Bitmap ImageToOverwrite, Orientation Direction)
        {
            FastBitmap BM = new FastBitmap(ImageToOverwrite);
            if (Moving == 0 || Bandwidth == 0)  // \ Wenn der zu verschiebende Streifen keine Breite hat
                return BM;                      // / oder um keine Pixel verschoben werden soll, kann das Originalbild zurückgegeben werden.
            if (Moving < 0)                     // \
            {                                   //  | Wenn um eine negative Anzahl von Pixel verschoben werden soll,
                Moving = Math.Abs(Moving);      //  | dann wird die Richtung einfach umgekehrt und die Pixel positive.
                Direction = Invert[Direction];  //  |
            }                                   // /

            // Left & Right
            if ((int)Direction % 2 == 1)
            {
                if (Moving == BM.Width)
                    return BM;
                Moving = Moving % BM.Width;

                if (Direction == Orientation.Right)
                    Moving = BM.Width - Moving;

                //Erstellt ein 2D Farben Array, das Entsprechend der zu verschiebenden Breite hoch und ensprechend der Bitmap breit ist.
                Color[,] Arr = new Color[Bandwidth, BM.Width];

                //Die Schleife wird durchlaufen und schreibt die einzelnen Pixel in das Array.
                //Sie beginnt an dem angegeben Beginwert und endet nachdem der Beginwert + Breite überschritten sind, oder das Ende des Bildes erreicht wurde.
                for (int j = InitHeight; j < Bandwidth + InitHeight && j < BM.Height; j++)
                    //Verschachtelte Schleife, damit die ganze Zeile bearbeitet wird.
                    for (int i = 0; i < BM.Width; i++)
                        Arr[j - InitHeight, i] = BM.GetPixel(i, j); //Schreibt die Pixel farben an der Momentanen Stelle ins Array.
                //Für das Array muss noch der BeginWert vom Zähler abgezogen werden.

                FastBitmap N_BM = new FastBitmap(BM);   //Erstellt eine Copie der Bitmap

                //Die Schleife ist gleich aufgesetzt wie die Vorherige. Die verschachtelte Schleife auch
                for (int j = InitHeight; j < Bandwidth + InitHeight && j < BM.Height; j++)
                    for (int i = 0; i < N_BM.Width; i++)
                        //Färbt die Pixel der neuen Bitmap ein. Die erste Variable muss wieder der Beginwert subtrahiert werden.
                        //Für die zweite Variable, wird der Wert, um den der Bereich verschoben werden soll addiert, und mit der Breite "Modolosiert"
                        N_BM.SetPixel(i, j, Arr[j - InitHeight, (i + Moving) % N_BM.Width]);

                return N_BM;
            }
            //Default: Hoch
            else
            {
                if (Moving == BM.Height)
                    return BM;
                Moving = Moving % BM.Height;

                if (Direction == Orientation.Down)
                    Moving = BM.Height - Moving;

                //Erstellt ein 2D Farben Array, das Entsprechend der zu verschiebenden Breite hoch und ensprechend der Bitmap breit ist.
                Color[,] Arr = new Color[Bandwidth, BM.Width];

                //Die Schleife wird durchlaufen und schreibt die einzelnen Pixel in das Array.
                //Sie beginnt an dem angegeben Beginwert und endet nachdem der Beginwert + Breite überschritten sind, oder das Ende des Bildes erreicht wurde.
                for (int j = InitHeight + Moving, k = 0; k < Bandwidth; j++, k++)
                    //Verschachtelte Schleife, damit die ganze Zeile bearbeitet wird.
                    for (int i = 0; i < BM.Width; i++)
                        Arr[k, i] = BM.GetPixel(i, j % BM.Height); //Schreibt die Pixel farben an der Momentanen Stelle ins Array.
                //Für das Array muss noch der BeginWert vom Zähler abgezogen werden.

                FastBitmap N_BM = new FastBitmap(BM);   //Erstellt eine Copie der Bitmap

                //Die Schleife ist gleich aufgesetzt wie die Vorherige. Die verschachtelte Schleife auch
                for (int j = InitHeight; j < Bandwidth + InitHeight && j < BM.Height; j++)
                    for (int i = 0; i < N_BM.Width; i++)
                        //Färbt die Pixel der neuen Bitmap ein. Die erste Variable muss wieder der Beginwert subtrahiert werden.
                        //Für die zweite Variable, wird der Wert, um den der Bereich verschoben werden soll addiert, und mit der Breite "Modolosiert"
                        N_BM.SetPixel(i, j, Arr[j - InitHeight, i]);

                return N_BM;                    
            }
        }

        /// <summary>
        /// Rotiert das ganze bild um eine bestimmte Anzahl von Pixeln nach links
        /// </summary>
        /// <param name="Pixel">Die Anzahl der Pixel, um die das Bild nach links verschoben werden soll</param>
        /// <param name="Image">Das Bild, das verschoben werden soll.</param>
        /// <returns>Das entsprechend der Angaben verschobene Bild</returns>
        public static Bitmap MoveImage(int Pixel, Bitmap Image)
        {
            return MoveImage(Pixel, Image, Orientation.Left);
        }

        /// <summary>
        /// Rotiert das ganze bild um eine bestimmte Anzahl von Pixeln in eine beliebige Richtung
        /// </summary>
        /// <param name="Pixel">Die Anzahl der Pixel, um die das Bild verschoben werden soll</param>
        /// <param name="Image">Das Bild, das verschoben werden soll</param>
        /// <param name="Direction">Enumerator: die Richtung in die das Bild verschoben werden soll.</param>
        /// <returns></returns>
        public static Bitmap MoveImage(int Pixel, Bitmap Image, Orientation Direction)
        {
            if (Pixel == 0)
                return Image;
            if (Pixel < 0)
            {
                Pixel = Math.Abs(Pixel);
                Direction = Invert[Direction];
            }

            if ((int)Direction % 2 == 1)
            {
                // Left & Right
                if (Pixel == Image.Width)
                    return Image;
                Pixel = Pixel % Image.Width;

                if (Direction == Orientation.Right)
                    Pixel = Image.Width - Pixel;

                Rectangle rec = new Rectangle(Pixel, 0, Image.Width - Pixel, Image.Height);
                Rectangle rec2 = new Rectangle(0, 0, Pixel, Image.Height);
                Bitmap Drawing = new Bitmap(Image.Width, Image.Height);
             
                using(Bitmap Section = Image.Clone(rec, PixelFormat.Format24bppRgb))
                using(Bitmap Rest = Image.Clone(rec2, PixelFormat.Format24bppRgb))
                using (Graphics g = Graphics.FromImage(Drawing))
                {
                    g.DrawImageUnscaled(Section, 0, 0);
                    g.DrawImageUnscaled(Rest, Image.Width - Pixel, 0);
                    
                    return Drawing;
                }
            }
            else
            {
                // Up & Down
                if (Pixel == Image.Height)
                    return Image;
                Pixel = Pixel % Image.Height;

                if (Direction == Orientation.Down)
                    Pixel = Image.Height - Pixel;

                Rectangle rec = new Rectangle(0, Pixel, Image.Width, Image.Height - Pixel);
                Rectangle rec2 = new Rectangle(0, 0, Image.Width, Pixel);
                Bitmap Drawing = new Bitmap(Image.Width, Image.Height);

                using(Bitmap Section = Image.Clone(rec, PixelFormat.Format24bppRgb))
                using(Bitmap Rest = Image.Clone(rec2, PixelFormat.Format24bppRgb))
                using (Graphics g = Graphics.FromImage(Drawing))
                {
                    g.DrawImageUnscaled(Section, 0, 0);
                    g.DrawImageUnscaled(Rest, 0, Image.Height - Pixel);
                    return Drawing;
                }
            }
        }
    }

    abstract class Wave_HDMA : Disorder_HDMA
    {
        /// <summary>
        /// Gibt an wie Stark die abweichung der Wellen ist.
        /// </summary>
        public int Amplitude
        {
            set
            {
                for (int i = 0; i < _IArr.Length; i++)
                    _IArr[i] = value * _baseArray[i];
                _Amplitude = value;
            }
            get { return _Amplitude;}
        }
        /// <summary>
        /// Gibt an wie Breit ein einzelnes zu verschiebendes Segment ist (in einem Sinus entsräche es Omega)
        /// </summary>
        public int Breite
        {
            get { return _Breite; }
            set 
            {
                if (value < 1 || value > 112)
                    throw new ArgumentException("Value cannot be smaller than 1 or bigger than 112");
                _Breite = value; 
            }
        }
        /// <summary>
        /// Gibt an mit welcher Geschwindigeit die Animation ausgeführt wird.
        /// Also, alle wie vielen durchläufe das Bild sich verändert und für den Code wie viele Frames gewartet wird.
        /// </summary>
        public double Speed
        {
            get { return _speed; }
            set 
            { 
                _dRounds = 0;
                if (value > 1)
                    throw new ArgumentOutOfRangeException("Speed cannot be set to a value larger than 1");
                int reversed = (int)(1.0 / value);

                if ((reversed & (reversed - 1)) != 0)
                    throw new ArgumentException("Speed can only be a value equal to 1/2^n. For example: 1, 0.5, 0.25...");

                _speed = value; 
            }
        }

        protected int[] _baseArray = new int[16] { 0, 1, 2, 3, 4, 5, 6, 7, 7, 6, 5, 4, 3, 2, 1, 0 };
        protected int[] _IArr = new int[16] { 0, 1, 2, 3, 4, 5, 6, 7, 7, 6, 5, 4, 3, 2, 1, 0 };
        protected int _Amplitude = 1;
        protected int _Breite = 6;
        protected int _Rounds = 0;
        protected double _dRounds = 0.0;
        protected double _speed = 0.25;

        public abstract Bitmap StaticPic(Bitmap Base);
        public abstract Bitmap Animate(Bitmap Base);
        public abstract String Code(String Tablename);
    }

    class Wave_X_HDMA : Wave_HDMA
    {
        public LayerRegister Layers
        {
            set
            {
                if (value == LayerRegister.Layer1_Y || value == LayerRegister.Layer2_Y)
                    throw new ArgumentException("This wave effect cannot take a LayerRegister for the Y axis");
                _Layers = value;
            }
            get { return _Layers; }
        }
        private LayerRegister _Layers = LayerRegister.Layer2_X;

        /// <summary>
        /// Erstellt eine Bitmap entsprechend den Angaben.
        /// </summary>
        /// <param name="Base">Das Stillstehende (unveränderte) Bild</param>
        /// <returns>Entsprechen den Angebane verschobenes Bild</returns>
        public override Bitmap StaticPic(Bitmap Base)
        {
            //Erstellt erst mal eine Kopie des übergeben Bildes.
            Bitmap NewOne = new Bitmap(Base);

            //NewOne wird überschrieben mit dem verzärten Bild. die 
            //_Breite * i gibt an, wo die verschiebung beginnt.
            //_Breite gibt an wie breit der zu verschiebende Bereich ist
            //_IArr[i % 16] gitb an um wie viele Pixel das Bild im aktuellen durchlauf verschoben werden soll
            for (int i = 0; i * _Breite < NewOne.Height; i++)
                NewOne = MoveLine((_Breite * i), _Breite, _IArr[i % 16] / 2, NewOne);

            return NewOne; //Gibt die veränderte Bild zurück
        }
        /// <summary>
        /// Erstellt eine Bitmap entsprechend den Angaben und dem internen Zähler und erhöht diesen im nachhinein
        /// </summary>
        /// <param name="Base">Das Stillstehende (unveränderte) Bild</param>
        /// <returns>Entsprechen den Angebane verschobenes Bild</returns>
        public override Bitmap Animate(Bitmap Base)
        {
            //Erstellt erst mal eine Kopie des übergeben Bildes.
            Bitmap NewOne = new Bitmap(Base);

            //Schleife die abhängig von der Breite teile des Bildes entsprechend verschiebt.
            //Die Schleife wird durchlaufen bis die Anzahl der Schleifendurchgänge mal der Breite die Höhe des Bildes überschreitet
            for (int i = 0; i * _Breite < NewOne.Height; i++)
                //NewOne wird überschrieben mit dem verzärten Bild. die 
                //_Breite * i gibt an, wo die verschiebung beginnt.
                //_Breite gibt an wie breit der zu verschiebende Bereich ist
                //_IArr[(i + _Rounds) % 16] gitb an um wie viele Pixel das Bild im aktuellen durchlauf verschoben werden soll
                NewOne = Disorder_HDMA.MoveLine((_Breite * i), _Breite, _IArr[(i + _Rounds) % 16] / 2, NewOne);

            //interner Zähler für den Offset des Arrays in der oberen Funktion.
            //der double Zähler wird pro durchlauf um das 4fache der _speed Variable erhöht.
            //dadurch können auch kleiner Angaben für die Geschwindigkeit (Speed) gemacht werden.
            _dRounds = (_dRounds + (_speed * 4.0)) % 16;
            _Rounds = (int)_dRounds;    //der tatsächliche offset entspricht dem double Zähler ohne Kommastellen.
            return NewOne;              //wenn für speed angaben kleiner 0.25 verwendet werden, wird der Zähler nur jeden xten durchlauf erhöht.
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Tablename"></param>
        /// <returns></returns>
        public override String Code(String Tablename)
        {
            int Base = 0x4330 + ((Channel - 3) * 0x10);
            int Register = (int)_Layers;
            int BaseAddress = (_Layers == LayerRegister.Layer1_X) ? Layer1_XPosition : Layer2_XPosition;
            int RegMode = ((Register & 0xFF) << 8) | Mode;
            int LSRs = (int)((1.0 / _speed) / 2.0);

            int Tablesize = (Scanlines / _Breite) >= 15 ? 15 : (Scanlines / _Breite);
            int TableInRAM = (((Scanlines / _Breite) + 1) * 3) + 1;
            String Table = ";The Table takes up " + TableInRAM + " bytes of the free RAM\n" +
                ";It ranges from $" + FreeRAM.ToString("X6") + " - $" + (FreeRAM + TableInRAM - 1).ToString("X6") + " (both addresses included)\n" +
                "." + Tablename.TrimStart('.') + "\n";
            for (int i = 0; i <= Tablesize; i++)
                Table += "\tdb $" + _IArr[i].ToString("X2") + "\n";

            String Code =
                INIT_Label +

                "REP #$20\t\t;\\\n" +
                "LDA #$" + RegMode.ToString("X4") + "\t\t; | Use Mode " + Mode.ToString("X") + " on register " + Register.ToString("X") + "\n" +
                "STA $" + Base.ToString("X4") + "\t\t; | 43" + Channel + "0 = Mode, 43" + Channel + "1 = Register\n" +
                "LDA #$" + (FreeRAM & 0xFFFF).ToString("X4") + "\t\t; | Address of HDMA table\n" +
                "STA $" + (Base + 2).ToString("X") + "\t\t; | 43" + Channel + "2 = Low-Byte of table, 43" + Channel + "3 = High-Byte of table\n" +
                "LDY.b #$" + (FreeRAM >> 16).ToString("X2") + "\t\t; | Address of HDMA table, get bank byte\n" +
                "STY $" + (Base + 4).ToString("X") + "\t\t; | 43" + Channel + "4 = Bank-Byte of table\n" +
                "SEP #$20\t\t;/\n" +
                "LDA #$" + (0x08 << (Channel - 3)).ToString("X2") + "\t\t;\\\n" +
                "TSB $0D9F\t\t;/ Enable HDMA channel " + Channel + "\n" +
                "RTS\n\n" +

                MAIN_Label +

                "LDX #$00\t\t;\\ Init of\n" +
                "LDY #$00\t\t; | X and Y. Y will be the loop counter, X the index for writing the table to the RAM\n" +
                "LDA $13\t\t\t; | Speed of Waves\n" +
                "LSR #" + LSRs.ToString() + "\t\t\t; | Slowing down A\n" +
                "STA $00\t\t\t;/ Save for later use\n\n" +

                "PHB\t\t\t;\\ Push data bank\n" +
                "PHK\t\t\t; | Push program bank\n" +
                "PLB\t\t\t;/ Pull data bank\n\n" +

                ".Loop\n" +
                "LDA #$" + _Breite.ToString("X2") + "\t\t;\\ Set scanline height\n" +
                "STA $" + FreeRAM.ToString("X6") + ",x\t\t; | for each wave\n" +
                "TYA\t\t\t; | Transfer Y to A\n" +
                "ADC $00\t\t\t; | Add in frame counter\n" +
                "AND #$" + Tablesize.ToString("X2") + "\t\t; | only the lower half of the byte\n" +
                "PHY\t\t\t; | Push Y, so that the loop counter isn't lost.\n" +
                "TAY\t\t\t;/ Transfer A to Y\n\n" +

                "LDA.w ." + Tablename.TrimStart('.') + ",y\t;\\ Load in wave values\n" +
                "LSR A\t\t\t; | half of waves only\n" +
                "CLC\t\t\t; | Clear carry flag for proper addition\n" +
                "ADC $" + BaseAddress.ToString("X4") + "\t\t; | Add value from the wave table to layer x position (low byte).\n" +
                "STA $" + (FreeRAM + 1).ToString("X6") + ",x\t\t; | X position low byte\n" +
                "LDA $" + (BaseAddress + 1).ToString("X4") + "\t\t; | Load layer x position (high byte).\n" +
                "ADC #$00\t\t; | Add #$00 without clearing the carry for pseude 16-bit addition\n" +
                "STA $" + (FreeRAM + 2).ToString("X6") + ",x\t\t;/ X position high byte\n\n" +

                "PLY\t\t\t;\\ Pull Y (original loop counter)\n" +
                "CPY #$" + (Scanlines / _Breite).ToString("X2") + "\t\t; | Compare with #$" + (Scanlines / _Breite).ToString("X2") + " scanlines\n" +
                "BPL .End\t\t; | If bigger, end HDMA\n" +
                "INX\t\t\t; | Increase X, so that in the next loop, it writes the new table data at the end of the old one...\n" +
                "INX\t\t\t; | Increase X, ...instead of overwriting it.\n" +
                "INX\t\t\t; | Increase X\n" +
                "INY\t\t\t; | Increase Y\n" +
                "BRA .Loop\t\t;/ Do the loop\n\n" +

                ".End\n" +
                "PLB\t\t\t;\\ Pull data bank. Not doing this would be ugly...\n" +
                "LDA #$00\t\t; | End HMDA by writing\n" +
                "STA $" + (FreeRAM + 3).ToString("X6") + ",x\t\t; | #$00 here\n" +
                "RTS\t\t\t;/ Return";

            Code += "\n\n\n" + Table;

            return Code;
        }
    }

    class Wave_Y_HDMA : Wave_HDMA
    {
        public LayerRegister Layers
        {
            set
            {
                if(value == LayerRegister.Layer1_X || value == LayerRegister.Layer2_X)
                    throw new ArgumentException("This wave effect cannot take a LayerRegister for the X axis");
                _Layers = value;
            }
            get { return _Layers; }
        }

        private LayerRegister _Layers = LayerRegister.Layer2_Y;

        public override Bitmap StaticPic(Bitmap Base)
        {
            //Erstellt erst mal eine Kopie des übergeben Bildes.
            Bitmap NewOne = new Bitmap(Base);

            //NewOne wird überschrieben mit dem verzärten Bild. die 
            //_Breite * i gibt an, wo die verschiebung beginnt.
            //_Breite gibt an wie breit der zu verschiebende Bereich ist
            //_IArr[i % 16] gitb an um wie viele Pixel das Bild im aktuellen durchlauf verschoben werden soll
            for (int i = 0; i * _Breite < NewOne.Height; i++)
                NewOne = MoveLine((_Breite * i), _Breite, _IArr[i % 16] / 2, NewOne, Orientation.Up);

            return NewOne; //Gibt die veränderte Bild zurück
        }
        public override Bitmap Animate(Bitmap Base)
        {
            //Erstellt erst mal eine Kopie des übergeben Bildes.
            Bitmap NewOne = new Bitmap(Base);

            //Schleife die abhängig von der Breite teile des Bildes entsprechend verschiebt.
            //Die Schleife wird durchlaufen bis die Anzahl der Schleifendurchgänge mal der Breite die Höhe des Bildes überschreitet
            for (int i = 0; i * _Breite < NewOne.Height; i++)
                //NewOne wird überschrieben mit dem verzärten Bild. die 
                //_Breite * i gibt an, wo die verschiebung beginnt.
                //_Breite gibt an wie breit der zu verschiebende Bereich ist
                //_IArr[(i + _Rounds) % 16] gitb an um wie viele Pixel das Bild im aktuellen durchlauf verschoben werden soll
                NewOne = Disorder_HDMA.MoveLine((_Breite * i), _Breite, _IArr[(i + _Rounds) % 16] / 2, NewOne, Orientation.Up);

            //interner Zähler für den Offset des Arrays in der oberen Funktion.
            //der double Zähler wird pro durchlauf um das 4fache der _speed Variable erhöht.
            //dadurch können auch kleiner Angaben für die Geschwindigkeit (Speed) gemacht werden.
            _dRounds = (_dRounds + (_speed * 4.0)) % 16;
            _Rounds = (int)_dRounds;    //der tatsächliche offset entspricht dem double Zähler ohne Kommastellen.
            return NewOne;              //wenn für speed angaben kleiner 0.25 verwendet werden, wird der Zähler nur jeden xten durchlauf erhöht.
        }
        public override string Code(string Tablename)
        {
            int Base = 0x4330 + ((Channel - 3) * 0x10);
            int Register = (int)_Layers;
            int BaseAddress = (_Layers == LayerRegister.Layer1_Y) ? Layer1_YPosition : Layer2_YPosition;
            int RegMode = ((Register & 0xFF) << 8) | Mode;
            int LSRs = (int)((1.0 / _speed) / 2.0);

            int Tablesize = (Scanlines / _Breite) >= 15 ? 15 : (Scanlines / _Breite);
            int TableInRAM = (((Scanlines / _Breite) + 1) * 3) + 1;
            String Table = ";The Table takes up " + TableInRAM + " bytes of the free RAM\n" +
                ";It ranges from $" + FreeRAM.ToString("X6") + " - $" + (FreeRAM + TableInRAM - 1).ToString("X6") + " (both addresses included)\n" +
                "." + Tablename.TrimStart('.') + "\n";
            for (int i = 0; i <= Tablesize; i++)
                Table += "\tdb $" + _IArr[i].ToString("X2") + "\n";

            String Code =
                INIT_Label +

                "REP #$20\t\t;\\\n" +
                "LDA #$" + RegMode.ToString("X4") + "\t\t; | Use Mode " + Mode.ToString("X") + " on register " + Register.ToString("X") + "\n" +
                "STA $" + Base.ToString("X4") + "\t\t; | 43" + Channel + "0 = Mode, 43" + Channel + "1 = Register\n" +
                "LDA #$" + (FreeRAM & 0xFFFF).ToString("X4") + "\t\t; | Address of HDMA table\n" +
                "STA $" + (Base + 2).ToString("X") + "\t\t; | 43" + Channel + "2 = Low-Byte of table, 43" + Channel + "3 = High-Byte of table\n" +
                "LDY.b #$" + (FreeRAM >> 16).ToString("X2") + "\t\t; | Address of HDMA table, get bank byte\n" +
                "STY $" + (Base + 4).ToString("X") + "\t\t; | 43" + Channel + "4 = Bank-Byte of table\n" +
                "SEP #$20\t\t;/\n" +
                "LDA #$" + (0x08 << (Channel - 3)).ToString("X2") + "\t\t;\\\n" +
                "TSB $0D9F\t\t;/ Enable HDMA channel " + Channel + "\n" +
                "RTS\n\n" +

                MAIN_Label +

                "LDX #$00\t\t;\\ Init of\n" +
                "LDY #$00\t\t; | X and Y. Y will be the loop counter, X the index for writing the table to the RAM\n" +
                "LDA $13\t\t\t; | Speed of Waves\n" +
                "LSR #" + LSRs.ToString() + "\t\t\t; | Slowing down A\n" +
                "STA $00\t\t\t;/ Save for later use\n\n" +

                "PHB\t\t\t;\\ Push data bank\n" +
                "PHK\t\t\t; | Push program bank\n" +
                "PLB\t\t\t;/ Pull data bank\n\n" +

                ".Loop\n" +
                "LDA #$" + _Breite.ToString("X2") + "\t\t;\\ Set scanline height\n" +
                "STA $" + FreeRAM.ToString("X6") + ",x\t\t; | for each wave\n" +
                "TYA\t\t\t; | Transfer Y to A\n" +
                "ADC $00\t\t\t; | Add in frame counter\n" +
                "AND #$" + Tablesize.ToString("X2") + "\t\t; | only the lower half of the byte\n" +
                "PHY\t\t\t; | Push Y, so that the loop counter isn't lost.\n" +
                "TAY\t\t\t;/ Transfer A to Y\n\n" +

                "LDA.w ." + Tablename.TrimStart('.') + ",y\t;\\ Load in wave values\n" +
                "LSR A\t\t\t; | half of waves only\n" +
                "CLC\t\t\t; | Clear carry flag for proper addition\n" +
                "ADC $" + BaseAddress.ToString("X4") + "\t\t; | Add value from the wave table to layer x position (low byte).\n" +
                "STA $" + (FreeRAM + 1).ToString("X6") + ",x\t\t; | X position low byte\n" +
                "LDA $" + (BaseAddress + 1).ToString("X4") + "\t\t; | Load layer x position (high byte).\n" +
                "ADC #$00\t\t; | Add #$00 without clearing the carry for pseude 16-bit addition\n" +
                "STA $" + (FreeRAM + 2).ToString("X6") + ",x\t\t;/ X position high byte\n\n" +

                "PLY\t\t\t;\\ Pull Y (original loop counter)\n" +
                "CPY #$" + (Scanlines / _Breite).ToString("X2") + "\t\t; | Compare with #$" + (Scanlines / _Breite).ToString("X2") + " scanlines\n" +
                "BPL .End\t\t; | If bigger, end HDMA\n" +
                "INX\t\t\t; | Increase X, so that in the next loop, it writes the new table data at the end of the old one...\n" +
                "INX\t\t\t; | Increase X, ...instead of overwriting it.\n" +
                "INX\t\t\t; | Increase X\n" +
                "INY\t\t\t; | Increase Y\n" +
                "BRA .Loop\t\t;/ Do the loop\n\n" +

                ".End\n" +
                "PLB\t\t\t;\\ Pull data bank. Not doing this would be ugly...\n" +
                "LDA #$00\t\t; | End HMDA by writing\n" +
                "STA $" + (FreeRAM + 3).ToString("X6") + ",x\t\t; | #$00 here\n" +
                "RTS\t\t\t;/ Return";

            Code += "\n\n\n" + Table;

            return Code;
        }            
    }

    class Parallax_HDMA : Disorder_HDMA
    {
        public class ScrollData : IComparable
        {
            public int Index;
            public int LineCount;
            public double ScrollRate;

            public ScrollData() { }
            public ScrollData(int Index, int LineCount, double ScrollRate)
            {
                this.Index = Index;
                this.LineCount = LineCount;
                this.ScrollRate = ScrollRate;
            }

            public int CompareTo(object obj)
            {
                if (this.ScrollRate > ((ScrollData)obj).ScrollRate)
                    return 1;
                else if (this.ScrollRate == ((ScrollData)obj).ScrollRate)
                    return 0;
                else
                    return -1;
            }
        }

        public List<ScrollData> TableData = new List<ScrollData>();
        public String Code() { return Code(this.TableData); }
        public String Code(List<ScrollData> Table)
        {
            if(Table == null || Table.Count == 0)
                return "";
            String BaseTable = ".ScanlineTable\n";
            int CountLines = 0;
            foreach (ScrollData SD in Table)
            {
                if (SD.LineCount <= 0x80)
                    BaseTable += "\tdb $" + SD.LineCount.ToString("X2") + ",$00,$00\n";
                else
                {
                    BaseTable += "\tdb $80,$00,$00\n";
                    BaseTable += "\tdb $" + (SD.LineCount - 0x80).ToString("X2") + ",$00,$00\n";
                }
                CountLines += SD.LineCount;
                if (CountLines >= Scanlines)
                    break;
            }
            BaseTable += "\tdb $00\n";

            Table.Sort();
            var GroupedTable = Table.GroupBy(Tab => Tab.ScrollRate); 
            
            int Base = 0x4330 + ((Channel - 3) * 0x10);
            int RegMode = (((int)LayerRegister.Layer2_X << 8) & 0xFF00) + Mode;

            String Code = INIT_Label +

                "REP #$20\t\t;\\\n" +
                "LDA #$" + RegMode.ToString("X4") + "\t\t; | Use Mode " + Mode.ToString("X2") + " on register " + ((int)LayerRegister.Layer2_X).ToString("X4") + "\n" +
                "STA $" + Base.ToString("X4") + "\t\t; | 43" + Channel + "0 = Mode, 43" + Channel + "1 = Register\n" +
                "LDA #$" + (FreeRAM & 0xFFFF).ToString("X4") + "\t\t; | Address of HDMA table\n" +
                "STA $" + (Base + 2).ToString("X") + "\t\t; | 43" + Channel + "2 = Low-Byte of table, 43" + Channel + "3 = High-Byte of table\n" +
                "LDY.b #$" + (FreeRAM >> 16).ToString("X2") + "\t\t; | Address of HDMA table, get bank byte\n" +
                "STY $" + (Base + 4).ToString("X") + "\t\t; | 43" + Channel + "4 = Bank-Byte of table\n" +
                "SEP #$20\t\t;/\n" +
                "LDA #$" + (0x08 << (Channel - 3)).ToString("X2") + "\t\t;\\\n" +
                "TSB $0D9F\t\t;/ Enable HDMA channel " + Channel + "\n" +
                "RTS\n\n" +

                MAIN_Label +

                "LDX #$" + (Table.Count * 3).ToString("X2") + "\t\t;\\  Index for the table\n" +
                ".Loop\t\t\t; | Loop label...\n" +
                "LDA .ScanlineTable,x\t; | Load the value from the table\n" +
                "STA $" + FreeRAM.ToString("X6") + ",x\t\t; | And store it to the FreeRAM\n" +
                "DEX\t\t\t; | Decrease x, so next time we load the next lower value from the table.\n" +
                "BPL .Loop\t\t ;/  If x is still inbetween #$00 and #$7F we loopback.\n\n" +

                "REP #$20\t\t;\\  It's 16bit-mode-time kids.\n";

            foreach (IGrouping<double, ScrollData> temp in GroupedTable)
            {
                if (temp.Key == 0)
                    continue;

                double Val = temp.Key;
                int LSRASL = (int)(Math.Log((Val < 1) ? (1.0 / Val) : Val) / Math.Log(2));

                Code += "LDA $" + Layer2_XPosition.ToString("X4") + "\t\t; | Load the Layer 2 x-position\n" +
                    ((Val < 1) ? "LSR " : "ASL ") + "#" + LSRASL + "\t\t\t; | Apmlive scrolling\n";

                foreach (var item in temp)
                    Code += "STA $" + (FreeRAM + (1 + (3 * item.Index))).ToString("X6") + "\t\t; | Store scrollrate to table in RAM\n";
            }
                Code += "SEP #$20\t\t;/  Back to 8bit\n" +
                    "RTS\n\n" +
                    ";The Table takes up " + (Table.Count * 3 + 1) + " bytes of the free RAM\n" +
                    ";It ranges from $" + FreeRAM.ToString("X6") + " - $" + (FreeRAM + (Table.Count * 3)).ToString("X6") + " (both addresses included)\n" +
                    BaseTable;

            return Code;
        }

        public static Bitmap Merge(Bitmap B1, Bitmap B2, int Lines)
        {
            if (Lines == B2.Height)
                return B1;
            if (Lines == 0)
                return B2;

            Bitmap newBitmap = new Bitmap(B1.Width, B1.Height);
            Graphics newBitmapGraphics = Graphics.FromImage(newBitmap);

            Rectangle rec1 = new Rectangle(0, 0, B1.Width, Lines);
            Bitmap Cut1 = B1.Clone(rec1, System.Drawing.Imaging.PixelFormat.Undefined);
            newBitmapGraphics.DrawImageUnscaled(Cut1, 0, 0, B1.Width, Lines);

            Rectangle rec2 = new Rectangle(0, Lines, B2.Width, B2.Height - Lines);
            Bitmap Cut2 = B2.Clone(rec2, System.Drawing.Imaging.PixelFormat.Undefined);
            newBitmapGraphics.DrawImageUnscaled(Cut2, 0, Lines, B1.Width, Lines);
            return newBitmap;
        }
    }
}
