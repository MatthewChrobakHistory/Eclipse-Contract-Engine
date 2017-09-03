using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AdminClient.Editors
{
    public partial class AdminWindow : Form
    {
        public AdminWindow() {
            InitializeComponent();

            this.cmdLogin.MouseClick += CmdLogin_MouseClick;
        }

        private void CmdLogin_MouseClick(object sender, MouseEventArgs e) {
            Networking.NetworkManager.PacketManager.SendLoginUser(txtUsername.Text, txtPassword.Text);
        }
    }
}
