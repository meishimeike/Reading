using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Text.RegularExpressions;

namespace Reading
{
    public partial class MainForm : Form
    {
        //string txts;
        int distance = 0;

        public MainForm()
        {
            InitializeComponent();
        }

        private void openOToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Multiselect = true;
            ofd.Filter = "TXT File(*.txt)|*.txt";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                foreach (string f in ofd.FileNames)
                {
                    Addfiction(f);
                }
            }
        }

        private void closeCToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.ExitThread();
        }

        private void backgroudColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ColorDialog Cd = new ColorDialog();
            Cd.Color = richTextBox1.BackColor;
            if (Cd.ShowDialog() == DialogResult.OK)
                richTextBox1.BackColor = Cd.Color;
        }

        private void fontColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ColorDialog Cd = new ColorDialog();
            Cd.Color = richTextBox1.ForeColor;
            if (Cd.ShowDialog() == DialogResult.OK)
                richTextBox1.ForeColor = Cd.Color;
        }

        private void fontToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FontDialog Fd = new FontDialog();
            Fd.Font = richTextBox1.Font;
            if (Fd.ShowDialog() == DialogResult.OK)
                richTextBox1.Font = Fd.Font;
        }

        /// <summary>
        /// 打开TXT文件j
        /// </summary>
        /// <param name="filepath">文件路径</param>
        private fictioninfo OpenFile(string filepath)
        {
            LogHelp.Log("Reading " + filepath);

            fictioninfo tit = new fictioninfo();
            tit.path = filepath;
            string Title = Path.GetFileNameWithoutExtension(tit.path);
            tit.Data = File.ReadAllText(filepath, EncodingType.GetEncoding(tit.path));
            string testtitle = Regex.Match(tit.Data.Substring(0,200), @"(《|【|〖|『)\S*(》|】|〗|』)").Value;
            if (testtitle!="" && testtitle.Length < 50 )
                tit.Title = testtitle;
            else
                tit.Title = Title;

            MatchCollection matchtxt = Regex.Matches(tit.Data, @"序言|内容简介|第(\d|零|一|二|两|三|四|五|六|七|八|九|十|百|千|万|亿)*(卷|章|回|节).*\s");
            for (int i=0; i < matchtxt.Count; i++)
            {
                subtitinfo sub = new subtitinfo();
                sub.tit = matchtxt[i].Value;
                sub.index = matchtxt[i].Index;
                if (sub.tit.Length < 50)
                {
                    if (tit.subtitle.Count == 0 || sub.index - tit.subtitle[tit.subtitle.Count - 1].index > 100)
                        tit.subtitle.Add(sub);
                }
            }

            return tit;
        }

        private void treeView1_DoubleClick(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode!=null&&treeView1.SelectedNode.Level == 1)
            {
                Getchapter();
             }
         }

        private void Getchapter()
        {
            if (treeView1.SelectedNode != null)
            {
                int startindex = int.Parse(treeView1.SelectedNode.Name);
                int endindex;
                if (treeView1.SelectedNode.Index != treeView1.SelectedNode.Parent.Nodes.Count - 1)
                {
                    endindex = int.Parse(treeView1.SelectedNode.NextNode.Name);
                }
                else
                {
                    endindex = ((string[])treeView1.SelectedNode.Parent.Tag)[1].Length;
                }

                string str = ((string[])treeView1.SelectedNode.Parent.Tag)[1].Substring(startindex, endindex - startindex);
                richTextBox1.Text = str;
                toolStripStatusLabel2.Text = treeView1.SelectedNode.Parent.Text.Trim() + "：" + treeView1.SelectedNode.Text.Trim();
                toolStripStatusLabel4.Text = Math.Round(endindex / (double)((string[])treeView1.SelectedNode.Parent.Tag)[1].ToString().Length * 100, 2).ToString() + "%";
            }
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (treeView1.SelectedNode!=null && treeView1.SelectedNode.Level == 1)
            {
                treeView1.SelectedNode = treeView1.SelectedNode.NextNode;
                Getchapter();
                richTextBox1.Select(0, 1);
                richTextBox1.ScrollToCaret();
            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (treeView1.SelectedNode != null && treeView1.SelectedNode.Level == 1)
            {
                treeView1.SelectedNode = treeView1.SelectedNode.PrevNode;
                Getchapter();
                richTextBox1.Select(0, 1);
                richTextBox1.ScrollToCaret();
            }
        }

        private void splitContainer1_SplitterMoved(object sender, SplitterEventArgs e)
        {
            pictureBox1.Left = splitContainer1.SplitterDistance - 10;
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            if (pictureBox1.Tag.ToString()=="close")
            {
                treeView1.Visible = false;
                distance = splitContainer1.SplitterDistance;
                splitContainer1.SplitterDistance = 0;
                pictureBox1.Image = Properties.Resources.op;
                //pictureBox1.Left = splitContainer1.Panel2.Left;
                pictureBox1.Tag = "open";
            }
            else if (pictureBox1.Tag.ToString() == "open")
            {
                treeView1.Visible = true;
                splitContainer1.SplitterDistance = distance;
                pictureBox1.Image = Properties.Resources.cl;
               //pictureBox1.Left = splitContainer1.Panel1.Left + splitContainer1.Panel1.Width - pictureBox1.Width;
                pictureBox1.Tag = "close";
            }
        }

        private void splitContainer1_SizeChanged(object sender, EventArgs e)
        {
            pictureBox1.Top = (splitContainer1.Height - pictureBox1.Height) / 2;
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            string paths=null;
            foreach(TreeNode btr in treeView1.Nodes)
            {
                paths += ((string[])btr.Tag)[0] + ",";
            }

            string sindex = null;
            if (treeView1.SelectedNode != null && treeView1.SelectedNode.Parent!=null)
            {
                sindex = treeView1.SelectedNode.Parent.Index.ToString() + "," + treeView1.SelectedNode.Index.ToString();
            }
            else
            {
                sindex = "0";
            }
            
            ReadWriteIni.IniWriteValue(Config.configpath, "Fiction", "Path", Base64.Base64Encode(paths));
            ReadWriteIni.IniWriteValue(Config.configpath, "Fiction", "Sindex", sindex);
            ReadWriteIni.IniWriteValue(Config.configpath, "Fiction", "Backcolor", (new ColorConverter()).ConvertToString(richTextBox1.BackColor));
            ReadWriteIni.IniWriteValue(Config.configpath, "Fiction", "Fontcolor", (new ColorConverter()).ConvertToString(richTextBox1.ForeColor));
            ReadWriteIni.IniWriteValue(Config.configpath, "Fiction", "Font", (new FontConverter()).ConvertToString(richTextBox1.Font));        
        }

        private void Addfiction(string fictionpath)
        {       
            if (!string.IsNullOrEmpty(fictionpath)) 
            {
                fictioninfo fiction = OpenFile(fictionpath);
                string[] tags = new string[] { fiction.path, fiction.Data };
                treeView1.Nodes.Add(fiction.Title,fiction.Title);
                treeView1.Nodes[fiction.Title].Tag = tags;
                foreach (subtitinfo sub in fiction.subtitle)
                {
                    treeView1.Nodes[fiction.Title].Nodes.Add(sub.index.ToString(), sub.tit);
                }
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            try
            {
                string[] fictions = Config.fiction.Split(',');
                foreach (string f in fictions)
                {
                    if (File.Exists(f))
                        Addfiction(f);
                }
                richTextBox1.BackColor = (Color)(new ColorConverter()).ConvertFromString(Config.Backcolor);
                richTextBox1.ForeColor = (Color)(new ColorConverter()).ConvertFromString(Config.Fontcolor);
                richTextBox1.Font = (Font)(new FontConverter()).ConvertFromString(Config.Font);
                if (Config.Sindex != "0")
                {
                    string[] s = Config.Sindex.Split(',');
                    treeView1.SelectedNode = treeView1.Nodes[int.Parse(s[0])].Nodes[int.Parse(s[1])];
                    Getchapter();
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);
                LogHelp.Log(ex.Message);
            }
           
        }

        private void addToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog OFile = new OpenFileDialog();
            OFile.Filter = "TXT File(*.txt)|*.txt";
            if (OFile.ShowDialog() == DialogResult.OK)
            {
                Addfiction(OFile.FileName);
            }
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode.Level == 0)
                treeView1.SelectedNode.Remove();
        }

        private void showToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Visible = true;
            WindowState = FormWindowState.Normal;
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void addFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog FBD = new FolderBrowserDialog();
            if (FBD.ShowDialog() == DialogResult.OK)
            {
                string[] files = Directory.GetFiles(FBD.SelectedPath);
                foreach (string f in files) 
                {
                    if (Path.GetExtension(f).ToLower() == ".txt") 
                    {
                        Addfiction(f);
                    }
                }
            }
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Read TXT(*.txt) file");
        }

        private void cleanToolStripMenuItem_Click(object sender, EventArgs e)
        {
            treeView1.Nodes.Clear();
            richTextBox1.Text = "";
        }
    }
}
