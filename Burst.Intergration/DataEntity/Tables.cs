using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Burst.Data.Entity.CodeGenerate;
using BurstStudio.Burst_Intergration.DataEntity.AdapterProperties;

namespace BurstStudio.Burst_Intergration.DataEntity
{
    public partial class Tables : StepBase
    {
        public Tables()
        {
            InitializeComponent();
        }

        public override void Enter()
        {
            try
            {
                dataGridView1.Rows.Clear();
                foreach (var table in Wizard.Schema.AllTables)
                    dataGridView1.Rows.Add(false, table.Name);
            }
            catch
            {
                MessageBox.Show("无法连接到数据库。");
                Wizard.Cancel();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Wizard.PrevStep();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            List<string> res = new List<string>();

            foreach (DataGridViewRow row in dataGridView1.Rows)
                if (bool.Parse(row.Cells[0].Value.ToString()))
                    res.Add(row.Cells[1].Value as string);

            Dictionary<string, GFieldInfo[]> gfiss = new Dictionary<string, GFieldInfo[]>();
            foreach(var t in Wizard.Schema.AllTables)
                if (res.Contains(t.Name))
                {
                    var Fields = new List<GFieldInfo>();
                    foreach (var c in t.Columns.Values)
                    {
                        var c_i = new GFieldInfo();
                        c_i.Name = c.Name;
                        c_i.Type = c.Type.ToString();
                        c_i._db_type = c.DbType;
                        Fields.Add(c_i);
                    }
                    gfiss.Add(t.Name, Fields.ToArray());
                }
            List<GeneralProperty> ths = new List<GeneralProperty>();
            foreach (KeyValuePair<string, GFieldInfo[]> gfis in gfiss)
            {
                GeneralProperty gp = new GeneralProperty();
                gp.Fields = new List<GFieldInfo>(gfis.Value);
                gp.OwnerFields = new List<OwnerFieldInfo>();
                gp.TableName = gfis.Key;
                ths.Add(gp);
            }
            Wizard.Tables = ths.ToArray();
            Wizard.NextStep();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Wizard.Cancel();
        }
    }
}
