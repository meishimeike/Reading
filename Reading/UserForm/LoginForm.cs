using System;
using System.Windows.Forms;
using System.IO;

namespace Reading
{
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();
        }

        private void LoginForm_Load(object sender, EventArgs e)
        {
            Config.datapath = Application.UserAppDataPath;
            Config.Logpath = Config.datapath + "\\log.txt";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Trim()=="" || textBox2.Text.Trim()=="") 
            {
                MessageBox.Show("Username or Password is null", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                LogHelp.Log("Username or Password is null" + " Test:" + textBox1.Text + "," + textBox2.Text);
                return;
            }
            Config.userpath = Config.datapath + "\\" + textBox1.Text;
            if (!Directory.Exists(Config.userpath)) 
            {
                MessageBox.Show("Username is not exist", "Warning",MessageBoxButtons.OK,MessageBoxIcon.Warning);
                LogHelp.Log("Username is not exist" + " Test:" + textBox1.Text + "," + textBox2.Text);
                return;
            }
            Config.configpath = Config.userpath + "\\conf.ini";
            string passmd5 = ReadWriteIni.IniReadValue(Config.configpath, "User", "Passwd");
            if(passmd5!=MD5Helper.EncryptString(textBox2.Text))
            {
                MessageBox.Show("Password is error", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                LogHelp.Log("Password is error" + " Test:" + textBox1.Text + "," + textBox2.Text);
                return;
            }
            string temppath = ReadWriteIni.IniReadValue(Config.configpath, "Fiction", "Path");
            Config.fiction = Base64.Base64Decode(temppath);
            Config.Sindex = ReadWriteIni.IniReadValue(Config.configpath, "Fiction", "Sindex");
            Config.Backcolor = ReadWriteIni.IniReadValue(Config.configpath, "Fiction", "Backcolor");
            Config.Fontcolor = ReadWriteIni.IniReadValue(Config.configpath, "Fiction", "Fontcolor");
            Config.Font = ReadWriteIni.IniReadValue(Config.configpath, "Fiction", "Font");
            LogHelp.Log("Login Succeed" + " Username:" + textBox1.Text);
            Hide();
            MainForm Nmf = new MainForm();
            Nmf.ShowDialog();
            Close();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            RegForm RF = new RegForm();
            RF.ShowDialog();
        }
    }
}
