using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace HDMA_Generator_Tool
{

    abstract class HDMA
    {
        //###################################################################
        //------------------- Konstanten ------------------------------------
        //###################################################################

        public const int Scanlines = 224;
        public const String INIT_Label =
            ";;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;\n" +
            "; Level INIT Code\n" +
            ";;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;\n\n";
        public const String MAIN_Label =
            ";;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;\n" +
            "; Level MAIN Code\n" +
            ";;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;;\n\n";

        //###################################################################
        //------------------- Private Felder --------------------------------
        //###################################################################

        protected int _channel;
        protected List<String[]> _tables;

        //###################################################################
        //------------------- Member ----------------------------------------
        //###################################################################


        public List<String[]> Tables { set { _tables = value; } get { return _tables; } }
        public int Channel
        {
            get { return _channel; }
            set { _channel = (value % 3) + 3; }
        }


        //###################################################################
        //------------------- Konstruktoren ---------------------------------
        //###################################################################

        public HDMA() { this.Channel = 3; }
        public HDMA(int Channel) { this._channel = Channel; }

        //###################################################################
        //------------------- Methoden --------------------------------------
        //###################################################################

        public virtual void SetChannel(RadioButton CH3Button, RadioButton CH4Button, RadioButton CH5Button)
        {
            if (CH3Button.Checked)
                this.Channel = 3;
            else if (CH4Button.Checked)
                this.Channel = 4;
            else if (CH5Button.Checked)
                this.Channel = 5;
            else
                throw new NotImplementedException("At least one RadioButton has to be checked");
        }
                        
        public static bool Join(HDMA hdma1, HDMA hdma2, out List<String[]> NewTable)
        {
            try
            {
                if (hdma1._tables.Count == hdma2._tables.Count)
                {
                    for(int j = 0; j < hdma1._tables.Count; j++)
                        if (hdma1._tables[j][0] != hdma2._tables[j][0])
                        {
                            NewTable = null;
                            return false;
                        }

                    NewTable = new List<String[]>();
                    int newSize = (hdma1._tables[0].Length + hdma2._tables[0].Length) - 1;
                    for (int i = 0; i < hdma1._tables.Count; i++)
                    {
                        String[] SA = new String[newSize];
                        for (int k = 0; k < newSize; k++)
                            if (k < hdma1._tables[i].Length)
                                SA[k] = hdma1._tables[i][k];
                            else
                                SA[k] = hdma2._tables[i][k - hdma1._tables[i].Length + 1];
                        NewTable.Add(SA);
                    }
                    return true;
                }
                else
                    NewTable = null;
            }
            catch
            {
                NewTable = null;
            }
            return false;
        }
    }
}
