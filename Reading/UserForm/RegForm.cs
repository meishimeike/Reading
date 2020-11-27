using System;
using System.Windows.Forms;
using System.IO;

namespace Reading
{
    public partial class RegForm : Form
    {
        //Resources.language.En Lang = new Resources.language.En();
        
        public RegForm()
        {
            InitializeComponent();

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Trim()=="") 
            {
                MessageBox.Show("Username is not allow null", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if(Directory.Exists(Config.datapath+"\\"+textBox1.Text))
            {
                MessageBox.Show("Username is exist", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (textBox2.Text.Trim()=="")
            {
                MessageBox.Show("Password is not allow null", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (textBox2.Text != textBox3.Text) 
            {
                MessageBox.Show("Password is discordance", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try {
                Config.userpath = Config.datapath + "\\" + textBox1.Text;
                Config.configpath = Config.userpath + "\\conf.ini";
                Directory.CreateDirectory(Config.userpath);
                ReadWriteIni.IniWriteValue(Config.configpath, "User", "Passwd", MD5Helper.EncryptString(textBox2.Text));
                MessageBox.Show("Register is Success");
                Close();
            }
            catch (Exception ex){
                MessageBox.Show(ex.Message);
            }
                
           
        }
    }
}
