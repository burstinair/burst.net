using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using EnvDTE;

using Burst.Data.Entity.CodeGenerate;
using BurstStudio.Burst_Intergration.DataEntity.AdapterProperties;

namespace BurstStudio.Burst_Intergration.DataEntity
{
    public partial class Confirm : StepBase
    {
        public Confirm()
        {
            InitializeComponent();
        }

        public override void Initialize()
        {
            tbNamespace.Text = Utils.GetProject(DTE).Properties.Item("RootNamespace").Value.ToString();
        }

        public override void Enter()
        {
            cbTable.Items.Clear();
            cbTable.Items.AddRange(Wizard.Tables);
            if (Wizard.Tables.Length > 0)
                cbTable.SelectedIndex = 0;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            GeneratorManager.Generate(
                new CodeDomGenerator(),
                DTE,
                Wizard.Tables,
                cbOverwrite.Checked,
                cbToPascalName.Checked,
                cbGenerateInitializer.Checked,
                Wizard.InitializeParameters,
                tbNamespace.Text
            );
            Wizard.Close();
        }

        private void InitializeFields()
        {
            dataGridView1.Rows.Clear();
            foreach (GFieldInfo gfi in Wizard.Tables[cbTable.SelectedIndex].Fields)
                dataGridView1.Rows.Add(gfi.Name, string.Format("{0}({1})", gfi.DbType, gfi.Type), gfi.FieldAttributes);
        }

        private void cbTable_SelectedIndexChanged(object sender, EventArgs e)
        {
            InitializeFields();
        }

        //Edit
        private void button4_Click(object sender, EventArgs e)
        {
            new frmFields(Wizard.Tables[cbTable.SelectedIndex]).ShowDialog();
            InitializeFields();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Wizard.PrevStep();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Wizard.Cancel();
        }
    }
}
