using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using System.Xml;
using System.IO;
using FBConversation;
using MyFileParser;

namespace ConvertMessages
{
    public partial class Form1 : Form
    {
        IList<string> FileNames;
        IList<string> FilePaths;
        IDictionary<string, string> Translations = new Dictionary<string, string>();
        string sourcePath;       
        string destinationPath;
        bool XML = true;

        public Form1()
        {
            InitializeComponent();

            toolStripComboBox1.ComboBox.DataSource = new string[]{ "Save as .XML", "Save as .CSV" };
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            pLToolStripMenuItem.Enabled = false;
            SetControlsTexts("PL");
            label1.Text = "";
            label2.Text = "";
            label3.Text = "";
        }

        private void SetControlsTexts(string Language)
        {
            string path = "";
            if (Directory.GetParent(Directory.GetCurrentDirectory()).GetFiles("*.xml").Length == 0)
            {
                FileInfo[] files = Directory.GetParent(Directory.GetParent(Directory.GetCurrentDirectory()).ToString()).GetFiles("*.xml");
                if (files.Length == 0) MessageBox.Show("Could not load languages.");
                else
                {
                    for (int i = 0; i < files.Length; ++i)
                    {
                        if (files[i].Name == Language + ".xml")
                        {
                            path = files[i].FullName;
                            break;
                        }
                    }
                }
            }

            if (!string.IsNullOrEmpty(path))
            {
                try
                {
                    XmlDocument xml = new XmlDocument();

                    xml.Load(path);
                    XmlNodeList nodeList = xml.SelectNodes("/controls/control");

                    Translations.Clear();

                    for (int i = 0; i < nodeList.Count; ++i)
                    {
                        Translations.Add(nodeList[i].FirstChild.Name, nodeList[i].InnerText);
                        Control control = Controls[nodeList[i].FirstChild.Name];

                        if (control != null) control.Text = nodeList[i].InnerText;
                        else
                        {
                            control = tableLayoutPanel1.Controls[nodeList[i].FirstChild.Name];
                            if (control != null) control.Text = nodeList[i].InnerText;
                        }
                    }
                }
                catch (Exception e)
                {
                    MessageBox.Show("Error loading control texts.\n" + e.Message);
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog browserDialog = new FolderBrowserDialog();
            if (browserDialog.ShowDialog() == DialogResult.OK)
            {
                sourcePath = browserDialog.SelectedPath;

                label1.Text = sourcePath;

                FileNames = new List<string>();

                try
                {
                    FilePaths = Directory.GetFiles(sourcePath).ToList();
                    for (int i = 0; i < FilePaths.Count; ++i)
                    {
                        FileNames.Add(Path.GetFileName(FilePaths[i]));
                    }
                }
                catch (Exception A)
                {
                    MessageBox.Show(A.Message);
                }
            }
        }

        private void pLToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetControlsTexts("PL");
            pLToolStripMenuItem.Enabled = false;
            eNToolStripMenuItem.Enabled = true;
        }

        private void eNToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetControlsTexts("EN");
            pLToolStripMenuItem.Enabled = true;
            eNToolStripMenuItem.Enabled = false;
        }

        private void AddItemToLV(string FileName)
        {
            if (listView1.Columns.Count == 0)
            {
                listView1.Columns.Add(Translations["column1"]);
                listView1.Columns.Add(Translations["column2"]);
                listView1.Columns.Add(Translations["column3"]);
            }

            string[] values = { FileName, "", "" };

            ListViewItem item = new ListViewItem(values);
            item.Name = FileName;
            listView1.Items.Add(item);

            listView1.View = View.Details;
        }

        private void ModListViewItem(string ItemName, int SubitemIndex, string Text)
        {
            if (listView1.Columns.Count > SubitemIndex)
            {
                try
                {
                    ListViewItem item = listView1.Items[ItemName];

                    item.SubItems[SubitemIndex].Text = Text;
                    listView1.Update();
                }
                catch (Exception a)
                {
                    MessageBox.Show(a.Message);
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (FileNames != null)
            {
                if (FileNames.Count > 0)
                {
                    for (int i = 0; i < FileNames.Count; ++i)
                    {
                        AddItemToLV(FileNames[i]);
                    }

                    FolderBrowserDialog browserDialog = new FolderBrowserDialog();
                    if (browserDialog.ShowDialog() == DialogResult.OK)
                    {
                        destinationPath = browserDialog.SelectedPath;

                        groupBox1.Enabled = false;

                        for (int i = 0; i < FileNames.Count; ++i)
                        {
                            try
                            {
                                Conversation conversation = new Conversation(FilePaths[i]);

                                label3.Text = FileNames[i];
                                label3.Update();

                                BackgroundWorker background = new BackgroundWorker();
                                background.DoWork += Background_DoWork;
                                background.ProgressChanged += Background_ProgressChanged;
                                background.RunWorkerCompleted += Background_RunWorkerCompleted;
                                background.WorkerReportsProgress = true;

                                progressBar1.Value = 0;

                                background.RunWorkerAsync(conversation);

                                while (background.IsBusy)
                                {
                                    Application.DoEvents();
                                }

                                ModListViewItem(FileNames[i], 2, conversation.messages.Count.ToString());
                                ModListViewItem(FileNames[i], 1, conversation.name);
                                listView1.Update();
                            }
                            catch (Exception a)
                            {
                                MessageBox.Show(a.Message);
                            }

                        }

                        label3.Text = Translations["label3"];
                        groupBox2.Enabled = true;
                    }
                }
            }
        }

        private void Background_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {

        }

        private void Background_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
           
            progressBar1.Value = e.ProgressPercentage;
        }

        private void Background_DoWork(object sender, DoWorkEventArgs e)
        {
            Conversation conversation = (Conversation)e.Argument;
            BackgroundWorker worker = (BackgroundWorker)sender;
            double k = 0, pk = k;
            while ((k = conversation.Read(100)) < 1.0)
            {
                if (k == pk) worker.ReportProgress(100);
                else worker.ReportProgress((int)(k * 100));

                pk = k;
            }

            CSVFile f = new CSVFile(conversation.GetTable());
            f.Save(destinationPath + "\\" + conversation.filename, ";", true);
            conversation.SaveXml(destinationPath + "\\" + conversation.filename.Replace(".html", "") + ".xml");
            string[] toindex = new string[] { conversation.filename , conversation.messages.Count.ToString() };
            f.Save(destinationPath + "\\Index.csv", toindex, ";");
        }

        private void toolStripComboBox1_Click(object sender, EventArgs e)
        {
            if (toolStripComboBox1.ComboBox.SelectedItem.ToString() == "Save as .XML") XML = true;
            else XML = false;
        }
    }
}
