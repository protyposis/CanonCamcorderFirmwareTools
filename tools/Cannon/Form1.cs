using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Be.Windows.Forms;

namespace Cannon {
    public partial class Form1 : Form {

        private String filename;

        public Form1() {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e) {
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog(this) == DialogResult.OK) {
                filename = ofd.FileName.Substring(ofd.FileName.LastIndexOf('\\') + 1);
                panel1.ByteProvider = new DynamicFileByteProvider(ofd.FileName);
            }
        }

        private void button2_Click(object sender, EventArgs e) {
            long start = panel1.SelectionStart;
            long length = panel1.SelectionLength;

            byte[] bytes = panel1.CopySelectionToArray();

            System.Text.Encoding ascii = System.Text.Encoding.ASCII;
            List<DecryptionResult> rl = KeyTable.decryptTryAll(bytes, ascii.GetBytes(textBox1.Text), checkBox1.Checked);

            if (rl.Count > 10 && MessageBox.Show(this, "The resultset contains " + rl.Count + " elements. Do you want to see them all?", "Results", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning) == (DialogResult.No | DialogResult.Cancel))
                return;

            foreach (DecryptionResult res in rl) {
                Console.WriteLine(ascii.GetString(res.bytes));
                ByteSelectBox bsb = new ByteSelectBox(res);
                DialogResult result = bsb.ShowDialog(this);
                if (result == DialogResult.Abort)
                    break;
                if (result == DialogResult.OK)
                    newTableEntry(
                        start + bsb.SelectionStart,
                        (int)((res.i + bsb.SelectionStart) % (KeyTable.crypt1.Length - 1)),
                        (int)((res.j + bsb.SelectionStart) % (KeyTable.crypt2.Length - 1)),
                        bsb.SelectionLength,
                        bsb.SelectedBytes);
                //break;
            }
        }

        private byte[] getBytesFromClipboard() {

            byte[] buffer = null;
            IDataObject da = Clipboard.GetDataObject();
            if (da.GetDataPresent("BinaryData")) {
                System.IO.MemoryStream ms = (System.IO.MemoryStream)da.GetData("BinaryData");
                buffer = new byte[ms.Length];
                ms.Read(buffer, 0, buffer.Length);

            } else if (da.GetDataPresent(typeof(string))) {
                string sBuffer = (string)da.GetData(typeof(string));
                buffer = System.Text.Encoding.ASCII.GetBytes(sBuffer);
            }

            return buffer;
        }

        private void newTableEntry(long pos, int i, int j, long length, byte[] bytes) {
            System.Text.Encoding ascii = System.Text.Encoding.ASCII;
            dataGridView1.Rows.Add(pos, pos + length - 1, i, j, i + length - 1, j + length - 1, length, ascii.GetString(bytes));
            dataGridView1.Sort(dataGridView1.Columns[0], ListSortDirection.Ascending);
            dataGridView1.FirstDisplayedScrollingRowIndex = dataGridView1.Rows.Count - 1;
        }

        private void button3_Click(object sender, EventArgs e) {
            InputBox ib = new InputBox("Position...");
            if (ib.ShowDialog(this) == DialogResult.OK) {
                //try
                {
                    long p;
                    if (ib.Answer.StartsWith("0x"))
                        p = long.Parse(ib.Answer.Substring(2), System.Globalization.NumberStyles.HexNumber);
                    else
                        p = long.Parse(ib.Answer);
                    panel1.Select(p, 1);
                    panel1.ScrollByteIntoView();
                }
                //catch { }
            }
        }

        private void SaveLogToFile() {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.AddExtension = true;
            sfd.CreatePrompt = false;
            sfd.OverwritePrompt = true;
            sfd.FileName = "cannon_" + filename + "_" + DateTime.Now.ToShortDateString() + "_" + DateTime.Now.ToShortTimeString();
            sfd.FileName = System.Text.RegularExpressions.Regex.Replace(sfd.FileName, @"[^\w\.-]", "_");
            sfd.DefaultExt = "txt";
            sfd.Filter = "Cannon Logs (*.clo)|*.clo";

            if (sfd.ShowDialog(this) == DialogResult.OK) {

                System.IO.StreamWriter sw;
                sw = new System.IO.StreamWriter(sfd.OpenFile(), Encoding.ASCII); // sfd.OpenFile();
     

                foreach (DataGridViewRow r in dataGridView1.Rows) {
                    foreach (DataGridViewCell c in r.Cells) {
                        if(c.ColumnIndex == 7)
                            sw.Write("<binary>" + c.Value + "</binary>");
                        else
                            sw.Write(c.Value + "\t");
                    }
                    sw.Write(sw.NewLine);
                }

                sw.Close();
            }
        }

        private void LoadLogFromFile(bool clear) {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.DefaultExt = "txt";
            ofd.Filter = "Cannon Logs (*.clo)|*.clo";

            if (ofd.ShowDialog(this) == DialogResult.OK) {

                System.IO.StreamReader sr;
                sr = new System.IO.StreamReader(ofd.OpenFile(), Encoding.ASCII); // sfd.OpenFile();

                if (clear)
                    dataGridView1.Rows.Clear();

                string file = sr.ReadToEnd();
                string[] splitstrings = {"<binary>", "</binary>"};
                string[] binchunks = file.Split(splitstrings, StringSplitOptions.RemoveEmptyEntries);
                
                //fields
                long offset, offsetend, iend, jend, length;
                int i, j;
                string data;
                for (int x = 0; x < binchunks.Length - 1; x = x + 2) {
                    string[] numbers = binchunks[x].Split('\t');

                    if (numbers[0].StartsWith("\r\n"))
                        offset = Convert.ToInt64(numbers[0].Substring(2)); // remove newline
                    else
                        offset = Convert.ToInt64(numbers[0]); // only at file start

                    offsetend = Convert.ToInt64(numbers[1]);
                    i = Convert.ToInt32(numbers[2]);
                    j = Convert.ToInt32(numbers[3]);
                    iend = Convert.ToInt64(numbers[4]);
                    jend = Convert.ToInt64(numbers[5]);
                    length = Convert.ToInt64(numbers[6]);
                    data = binchunks[x + 1];

                    dataGridView1.Rows.Add(
                        offset,
                        offsetend,
                        i,
                        j,
                        iend,
                        jend,
                        length,
                        data);
                }

                sr.Close();
            }
        }

        private void button4_Click(object sender, EventArgs e) {
            SaveLogToFile();
        }

        private void button5_Click(object sender, EventArgs e) {
            LoadLogFromFile(true);
        }

        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e) {
            //toolStripStatusLabel1.Text = 
        }
    }
}
