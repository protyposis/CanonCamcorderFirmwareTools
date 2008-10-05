using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Cannon
{
    public partial class InputBox : Form
    {
        private string answer;

        public InputBox(String question)
        {
            InitializeComponent();
            label1.Text = question;
        }

        public InputBox(String question, String defaultanswer):this(question)
        {
            textBox1.Text = defaultanswer;
        }

        public string Answer
        {
            get { return answer; }
        }

        private void InputBox_FormClosing(object sender, FormClosingEventArgs e)
        {
            answer = textBox1.Text;
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                DialogResult = DialogResult.OK;
                this.Close();
            }
        }
    }
}
