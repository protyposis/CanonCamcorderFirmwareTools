namespace HF10_Bitmap_Viewer {
    partial class Form1 {
        /// <summary>
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Windows Form-Designer generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent() {
            this.btnLoadFile = new System.Windows.Forms.Button();
            this.lblFilename = new System.Windows.Forms.Label();
            this.pbOriginal = new System.Windows.Forms.PictureBox();
            this.nudPos = new System.Windows.Forms.NumericUpDown();
            this.nudWidth = new System.Windows.Forms.NumericUpDown();
            this.nudHeight = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.lblDecimalValues = new System.Windows.Forms.Label();
            this.lblBitmapHeader = new System.Windows.Forms.Label();
            this.btnNext = new System.Windows.Forms.Button();
            this.btnPrev = new System.Windows.Forms.Button();
            this.pbMediumZoom = new HF10_Bitmap_Viewer.PictureBoxEx();
            this.pbZoomed = new HF10_Bitmap_Viewer.PictureBoxEx();
            this.btnClearHistory = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pbOriginal)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudPos)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudWidth)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudHeight)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbMediumZoom)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbZoomed)).BeginInit();
            this.SuspendLayout();
            // 
            // btnLoadFile
            // 
            this.btnLoadFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnLoadFile.Location = new System.Drawing.Point(376, 12);
            this.btnLoadFile.Name = "btnLoadFile";
            this.btnLoadFile.Size = new System.Drawing.Size(75, 23);
            this.btnLoadFile.TabIndex = 0;
            this.btnLoadFile.Text = "Load File";
            this.btnLoadFile.UseVisualStyleBackColor = true;
            this.btnLoadFile.Click += new System.EventHandler(this.btnLoadFile_Click);
            // 
            // lblFilename
            // 
            this.lblFilename.AutoSize = true;
            this.lblFilename.Location = new System.Drawing.Point(12, 17);
            this.lblFilename.Name = "lblFilename";
            this.lblFilename.Size = new System.Drawing.Size(49, 13);
            this.lblFilename.TabIndex = 1;
            this.lblFilename.Text = "Filename";
            // 
            // pbOriginal
            // 
            this.pbOriginal.BackColor = System.Drawing.Color.Magenta;
            this.pbOriginal.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pbOriginal.Location = new System.Drawing.Point(12, 68);
            this.pbOriginal.Name = "pbOriginal";
            this.pbOriginal.Size = new System.Drawing.Size(100, 96);
            this.pbOriginal.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.pbOriginal.TabIndex = 2;
            this.pbOriginal.TabStop = false;
            // 
            // nudPos
            // 
            this.nudPos.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.nudPos.Hexadecimal = true;
            this.nudPos.Location = new System.Drawing.Point(331, 68);
            this.nudPos.Maximum = new decimal(new int[] {
            100000000,
            0,
            0,
            0});
            this.nudPos.Name = "nudPos";
            this.nudPos.Size = new System.Drawing.Size(120, 20);
            this.nudPos.TabIndex = 4;
            this.nudPos.ValueChanged += new System.EventHandler(this.numericUpDown_ValueChanged);
            this.nudPos.Enter += new System.EventHandler(this.numericUpDown_ValueChanged);
            // 
            // nudWidth
            // 
            this.nudWidth.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.nudWidth.Enabled = false;
            this.nudWidth.Hexadecimal = true;
            this.nudWidth.Location = new System.Drawing.Point(331, 94);
            this.nudWidth.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.nudWidth.Name = "nudWidth";
            this.nudWidth.Size = new System.Drawing.Size(120, 20);
            this.nudWidth.TabIndex = 5;
            // 
            // nudHeight
            // 
            this.nudHeight.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.nudHeight.Hexadecimal = true;
            this.nudHeight.Location = new System.Drawing.Point(331, 120);
            this.nudHeight.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.nudHeight.Name = "nudHeight";
            this.nudHeight.Size = new System.Drawing.Size(120, 20);
            this.nudHeight.TabIndex = 6;
            this.nudHeight.Value = new decimal(new int[] {
            18,
            0,
            0,
            0});
            this.nudHeight.ValueChanged += new System.EventHandler(this.numericUpDown_ValueChanged);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(246, 70);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(70, 13);
            this.label1.TabIndex = 7;
            this.label1.Text = "Data Position";
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(246, 96);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(35, 13);
            this.label2.TabIndex = 8;
            this.label2.Text = "Width";
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(246, 122);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(38, 13);
            this.label3.TabIndex = 9;
            this.label3.Text = "Height";
            // 
            // lblDecimalValues
            // 
            this.lblDecimalValues.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblDecimalValues.Location = new System.Drawing.Point(249, 143);
            this.lblDecimalValues.Name = "lblDecimalValues";
            this.lblDecimalValues.Size = new System.Drawing.Size(202, 23);
            this.lblDecimalValues.TabIndex = 10;
            this.lblDecimalValues.Text = "decimal values";
            this.lblDecimalValues.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblBitmapHeader
            // 
            this.lblBitmapHeader.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblBitmapHeader.AutoSize = true;
            this.lblBitmapHeader.Location = new System.Drawing.Point(246, 187);
            this.lblBitmapHeader.Name = "lblBitmapHeader";
            this.lblBitmapHeader.Size = new System.Drawing.Size(63, 13);
            this.lblBitmapHeader.TabIndex = 12;
            this.lblBitmapHeader.Text = "bmp header";
            // 
            // btnNext
            // 
            this.btnNext.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnNext.Location = new System.Drawing.Point(342, 243);
            this.btnNext.Name = "btnNext";
            this.btnNext.Size = new System.Drawing.Size(109, 23);
            this.btnNext.TabIndex = 13;
            this.btnNext.Text = "find next ->";
            this.btnNext.UseVisualStyleBackColor = true;
            this.btnNext.Click += new System.EventHandler(this.btnNext_Click);
            // 
            // btnPrev
            // 
            this.btnPrev.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnPrev.Location = new System.Drawing.Point(249, 243);
            this.btnPrev.Name = "btnPrev";
            this.btnPrev.Size = new System.Drawing.Size(87, 23);
            this.btnPrev.TabIndex = 14;
            this.btnPrev.Text = "<- history back";
            this.btnPrev.UseVisualStyleBackColor = true;
            this.btnPrev.Click += new System.EventHandler(this.btnPrev_Click);
            // 
            // pbMediumZoom
            // 
            this.pbMediumZoom.BackColor = System.Drawing.Color.Magenta;
            this.pbMediumZoom.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pbMediumZoom.Location = new System.Drawing.Point(118, 68);
            this.pbMediumZoom.Name = "pbMediumZoom";
            this.pbMediumZoom.Size = new System.Drawing.Size(100, 96);
            this.pbMediumZoom.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pbMediumZoom.TabIndex = 11;
            this.pbMediumZoom.TabStop = false;
            // 
            // pbZoomed
            // 
            this.pbZoomed.BackColor = System.Drawing.Color.Magenta;
            this.pbZoomed.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.pbZoomed.Location = new System.Drawing.Point(12, 170);
            this.pbZoomed.Name = "pbZoomed";
            this.pbZoomed.Size = new System.Drawing.Size(206, 192);
            this.pbZoomed.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pbZoomed.TabIndex = 3;
            this.pbZoomed.TabStop = false;
            // 
            // btnClearHistory
            // 
            this.btnClearHistory.Location = new System.Drawing.Point(249, 272);
            this.btnClearHistory.Name = "btnClearHistory";
            this.btnClearHistory.Size = new System.Drawing.Size(87, 23);
            this.btnClearHistory.TabIndex = 15;
            this.btnClearHistory.Text = "clear history";
            this.btnClearHistory.UseVisualStyleBackColor = true;
            this.btnClearHistory.Click += new System.EventHandler(this.btnClearHistory_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(463, 376);
            this.Controls.Add(this.btnClearHistory);
            this.Controls.Add(this.btnPrev);
            this.Controls.Add(this.btnNext);
            this.Controls.Add(this.lblBitmapHeader);
            this.Controls.Add(this.pbMediumZoom);
            this.Controls.Add(this.lblDecimalValues);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.nudHeight);
            this.Controls.Add(this.nudWidth);
            this.Controls.Add(this.nudPos);
            this.Controls.Add(this.pbZoomed);
            this.Controls.Add(this.pbOriginal);
            this.Controls.Add(this.lblFilename);
            this.Controls.Add(this.btnLoadFile);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "Form1";
            this.Text = "HF10 Bitmap Viewer";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pbOriginal)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudPos)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudWidth)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudHeight)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbMediumZoom)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbZoomed)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnLoadFile;
        private System.Windows.Forms.Label lblFilename;
        private System.Windows.Forms.PictureBox pbOriginal;
        private System.Windows.Forms.NumericUpDown nudPos;
        private System.Windows.Forms.NumericUpDown nudWidth;
        private System.Windows.Forms.NumericUpDown nudHeight;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label lblDecimalValues;
        private System.Windows.Forms.Label lblBitmapHeader;
        private PictureBoxEx pbZoomed;
        private PictureBoxEx pbMediumZoom;
        private System.Windows.Forms.Button btnNext;
        private System.Windows.Forms.Button btnPrev;
        private System.Windows.Forms.Button btnClearHistory;
    }
}

