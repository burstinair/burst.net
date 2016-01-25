using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using BurstStudio.Burst_Intergration.DataEntity.AdapterProperties;

namespace BurstStudio.Burst_Intergration.DataEntity
{
    public partial class frmFields : Form
    {
        public frmFields(GeneralProperty GeneralProperty)
        {
            InitializeComponent();
            pgMain.SelectedObject = GeneralProperty;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void frmFields_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                Close();
        }
    }
}
