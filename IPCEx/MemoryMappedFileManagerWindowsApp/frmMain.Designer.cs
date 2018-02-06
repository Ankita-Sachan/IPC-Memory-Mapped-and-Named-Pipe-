using System;

namespace MemoryMappedFileManagerWindowsApp
{
    partial class frmMain
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.txtData = new System.Windows.Forms.TextBox();
            this.btnSend = new System.Windows.Forms.Button();
            this.lblEnterText = new System.Windows.Forms.Label();
            this.rdbMemoryMap = new System.Windows.Forms.RadioButton();
            this.rdbNamedPipe = new System.Windows.Forms.RadioButton();
            this.grp1 = new System.Windows.Forms.GroupBox();
            this.lblStatus = new System.Windows.Forms.Label();
            this.grp1.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtData
            // 
            this.txtData.Location = new System.Drawing.Point(24, 71);
            this.txtData.Name = "txtData";
            this.txtData.Size = new System.Drawing.Size(350, 20);
            this.txtData.TabIndex = 6;
            // 
            // btnSend
            // 
            this.btnSend.Location = new System.Drawing.Point(24, 107);
            this.btnSend.Name = "btnSend";
            this.btnSend.Size = new System.Drawing.Size(75, 23);
            this.btnSend.TabIndex = 7;
            this.btnSend.Text = "Enter";
            this.btnSend.UseVisualStyleBackColor = true;
            this.btnSend.Click += new System.EventHandler(this.btnSend_Click);
            // 
            // lblEnterText
            // 
            this.lblEnterText.AutoSize = true;
            this.lblEnterText.Location = new System.Drawing.Point(21, 42);
            this.lblEnterText.Name = "lblEnterText";
            this.lblEnterText.Size = new System.Drawing.Size(148, 13);
            this.lblEnterText.TabIndex = 8;
            this.lblEnterText.Text = "Please enter text here to send";
            // 
            // rdbMemoryMap
            // 
            this.rdbMemoryMap.AutoSize = true;
            this.rdbMemoryMap.Checked = true;
            this.rdbMemoryMap.Location = new System.Drawing.Point(12, 11);
            this.rdbMemoryMap.Name = "rdbMemoryMap";
            this.rdbMemoryMap.Size = new System.Drawing.Size(85, 17);
            this.rdbMemoryMap.TabIndex = 9;
            this.rdbMemoryMap.TabStop = true;
            this.rdbMemoryMap.Text = "Memory map";
            this.rdbMemoryMap.UseVisualStyleBackColor = true;
            // 
            // rdbNamedPipe
            // 
            this.rdbNamedPipe.AutoSize = true;
            this.rdbNamedPipe.Location = new System.Drawing.Point(113, 12);
            this.rdbNamedPipe.Name = "rdbNamedPipe";
            this.rdbNamedPipe.Size = new System.Drawing.Size(82, 17);
            this.rdbNamedPipe.TabIndex = 10;
            this.rdbNamedPipe.TabStop = true;
            this.rdbNamedPipe.Text = "Named pipe";
            this.rdbNamedPipe.UseVisualStyleBackColor = true;
            // 
            // grp1
            // 
            this.grp1.Controls.Add(this.lblStatus);
            this.grp1.Controls.Add(this.rdbMemoryMap);
            this.grp1.Controls.Add(this.rdbNamedPipe);
            this.grp1.Dock = System.Windows.Forms.DockStyle.Top;
            this.grp1.Location = new System.Drawing.Point(0, 0);
            this.grp1.Name = "grp1";
            this.grp1.Size = new System.Drawing.Size(419, 39);
            this.grp1.TabIndex = 11;
            this.grp1.TabStop = false;
            // 
            // lblStatus
            // 
            this.lblStatus.Location = new System.Drawing.Point(274, 9);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(100, 23);
            this.lblStatus.TabIndex = 11;
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(419, 143);
            this.Controls.Add(this.grp1);
            this.Controls.Add(this.lblEnterText);
            this.Controls.Add(this.btnSend);
            this.Controls.Add(this.txtData);
            this.MaximizeBox = false;
            this.Name = "frmMain";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Memory Mapped File Manager";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmMain_FormClosing);
            this.Load += new System.EventHandler(this.frmMain_Load);
            this.grp1.ResumeLayout(false);
            this.grp1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        #endregion
        private System.Windows.Forms.TextBox txtData;
        private System.Windows.Forms.Button btnSend;
        private System.Windows.Forms.Label lblEnterText;
        private System.Windows.Forms.RadioButton rdbMemoryMap;
        private System.Windows.Forms.RadioButton rdbNamedPipe;
        private System.Windows.Forms.GroupBox grp1;
        private System.Windows.Forms.Label lblStatus;
    }
}

