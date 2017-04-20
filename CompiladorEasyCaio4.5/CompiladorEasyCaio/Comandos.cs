using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace CompiladorEasyCaio
{
    public partial class Comandos : Form
    {
        public Comandos()
        {
            InitializeComponent();
        }

        public void Comandos_Load(object sender, EventArgs e)
        {
            cl_comandos cmd = new cl_comandos();
            cmd.CriarPastaRaiz();

            DirectoryInfo directoryInfo = new DirectoryInfo(cl_comandos.localPastaRaiz);

            if (directoryInfo.Exists)            
                BuildTree(directoryInfo, treeView1.Nodes);
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {            
            if (e.Node.Name.EndsWith("txt"))
            {
                this.textBox1.Clear();
                StreamReader reader = new StreamReader(e.Node.Name);
                this.textBox1.Text = reader.ReadToEnd();
                reader.Close();
            }
        }

        private void BuildTree(DirectoryInfo directoryInfo, TreeNodeCollection addInMe)
        {
            TreeNode curNode = addInMe.Add(directoryInfo.Name);

            foreach (FileInfo file in directoryInfo.GetFiles())
            {
                curNode.Nodes.Add(file.FullName, file.Name);
            }
            foreach (DirectoryInfo subdir in directoryInfo.GetDirectories())
            {
                BuildTree(subdir, curNode.Nodes);
            }
        }
    }
}
