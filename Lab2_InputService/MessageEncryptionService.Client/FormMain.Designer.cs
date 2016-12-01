namespace MessageEncryptionService.Client
{
    partial class FormMain
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
            this.btnSend = new System.Windows.Forms.Button();
            this.radioButtonSockets = new System.Windows.Forms.RadioButton();
            this.radioButtonRabbitMQ = new System.Windows.Forms.RadioButton();
            this.groupBoxType = new System.Windows.Forms.GroupBox();
            this.groupBoxType.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnSend
            // 
            this.btnSend.Location = new System.Drawing.Point(12, 135);
            this.btnSend.Name = "btnSend";
            this.btnSend.Size = new System.Drawing.Size(110, 23);
            this.btnSend.TabIndex = 0;
            this.btnSend.Text = "Send";
            this.btnSend.UseVisualStyleBackColor = true;
            this.btnSend.Click += new System.EventHandler(this.btnSend_Click);
            // 
            // radioButtonSockets
            // 
            this.radioButtonSockets.AutoSize = true;
            this.radioButtonSockets.Checked = true;
            this.radioButtonSockets.Location = new System.Drawing.Point(6, 19);
            this.radioButtonSockets.Name = "radioButtonSockets";
            this.radioButtonSockets.Size = new System.Drawing.Size(64, 17);
            this.radioButtonSockets.TabIndex = 1;
            this.radioButtonSockets.TabStop = true;
            this.radioButtonSockets.Text = "Sockets";
            this.radioButtonSockets.UseVisualStyleBackColor = true;
            // 
            // radioButtonRabbitMQ
            // 
            this.radioButtonRabbitMQ.AutoSize = true;
            this.radioButtonRabbitMQ.Location = new System.Drawing.Point(6, 42);
            this.radioButtonRabbitMQ.Name = "radioButtonRabbitMQ";
            this.radioButtonRabbitMQ.Size = new System.Drawing.Size(73, 17);
            this.radioButtonRabbitMQ.TabIndex = 2;
            this.radioButtonRabbitMQ.Text = "RabbitMQ";
            this.radioButtonRabbitMQ.UseVisualStyleBackColor = true;
            // 
            // groupBoxType
            // 
            this.groupBoxType.Controls.Add(this.radioButtonRabbitMQ);
            this.groupBoxType.Controls.Add(this.radioButtonSockets);
            this.groupBoxType.Location = new System.Drawing.Point(12, 12);
            this.groupBoxType.Name = "groupBoxType";
            this.groupBoxType.Size = new System.Drawing.Size(260, 68);
            this.groupBoxType.TabIndex = 3;
            this.groupBoxType.TabStop = false;
            this.groupBoxType.Text = "Send by";
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 170);
            this.Controls.Add(this.groupBoxType);
            this.Controls.Add(this.btnSend);
            this.Name = "FormMain";
            this.Text = "Service Client";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormMain_FormClosed);
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.groupBoxType.ResumeLayout(false);
            this.groupBoxType.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnSend;
        private System.Windows.Forms.RadioButton radioButtonSockets;
        private System.Windows.Forms.RadioButton radioButtonRabbitMQ;
        private System.Windows.Forms.GroupBox groupBoxType;
    }
}

