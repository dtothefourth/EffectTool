using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Extansion.Control
{
    public static class Ext_Control
    {
        public static void MoveItemUp(this ListBox myListBox)
        {
            int selectedIndex = myListBox.SelectedIndex;
            if (selectedIndex > 0 & selectedIndex != -1)
            {
                myListBox.Items.Insert(selectedIndex - 1, myListBox.Items[selectedIndex]);
                myListBox.Items.RemoveAt(selectedIndex + 1);
                myListBox.SelectedIndex = selectedIndex - 1;
            }
        }

        public static void MoveItemDown(this ListBox myListBox)
        {
            int selectedIndex = myListBox.SelectedIndex;
            if (selectedIndex < myListBox.Items.Count - 1 & selectedIndex != -1)
            {
                myListBox.Items.Insert(selectedIndex + 2, myListBox.Items[selectedIndex]);
                myListBox.Items.RemoveAt(selectedIndex);
                myListBox.SelectedIndex = selectedIndex + 1;

            }
        }        
    }
}
