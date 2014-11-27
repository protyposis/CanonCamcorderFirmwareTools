using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Be.Windows.Forms;

namespace Cannon {
    public partial class ByteSelectBox : Form {

        private byte[] bytes_selection;
        private long position, length;
        DecryptionResult res;

        public ByteSelectBox(DecryptionResult result) {
            InitializeComponent();
            panel1.ByteProvider = new DynamicByteProvider(result.bytes);
            panel1.SelectionStart = result.result_position;
            panel1.SelectionLength = result.result_length;
            res = result;
            refreshPositionLabel();

            String hex = "";
            for (int x = result.result_position; x < result.result_position + result.result_length; x++) {
                hex += String.Format("{0:X2}", result.bytes[x]) + " ";
            }
            label2.Text = hex;

            String str = "";
            for (int x = result.result_position; x < result.result_position + result.result_length; x++) {
                str += (char)result.bytes[x] + "  ";
            }
            label3.Text = str;
        }

        private void panel1_KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Enter) {
                DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        private void ByteSelectBox_FormClosing(object sender, FormClosingEventArgs e) {
            bytes_selection = panel1.CopySelectionToArray();
            position = panel1.SelectionStart;
            length = panel1.SelectionLength;
        }

        public byte[] SelectedBytes { get { return bytes_selection; } }
        public long SelectionStart { get { return position; } }
        public long SelectionLength { get { return length; } }

        private void panel1_SelectionStartChanged(object sender, EventArgs e) {
            refreshPositionLabel();
        }

        private void panel1_SelectionLengthChanged(object sender, EventArgs e) {
            refreshPositionLabel();
        }

        private void refreshPositionLabel() {
            label1.Text = String.Format("Offset: {0} KI1: {1} KI2: {2} length: {3}",
                "n/a", (res.i + panel1.SelectionStart) % (KeyTable.crypt1.Length - 1), (res.j + panel1.SelectionStart) % (KeyTable.crypt2.Length - 1),
                panel1.SelectionLength);
        }
    }
}
