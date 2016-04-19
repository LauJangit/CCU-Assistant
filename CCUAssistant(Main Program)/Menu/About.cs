using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApplication1
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            if (Var.Authstatus != 2)
            {
                System.Environment.Exit(0);
            }
            InitializeComponent();
            label6.Text = Var.Name+"账户的拥有者";
            if (Var.Studentname != "")
            { label6.Text = Var.Studentname + "(" + Var.Name + ")"; }
        }

        private void Form2_Load(object sender, EventArgs e)
        {

        }
    }
}
