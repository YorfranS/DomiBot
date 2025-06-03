namespace TelegramFoodBot.Presentation.Forms
{
    partial class FormReservas
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormReservas));
            panel1 = new System.Windows.Forms.Panel();
            panelDetallesPedidos = new System.Windows.Forms.Panel();
            vScrollBar2 = new System.Windows.Forms.VScrollBar();
            panel3 = new System.Windows.Forms.Panel();
            label1 = new System.Windows.Forms.Label();
            panelPedidos = new System.Windows.Forms.Panel();
            vScrollBar1 = new System.Windows.Forms.VScrollBar();
            label2 = new System.Windows.Forms.Label();
            btnBuscar = new System.Windows.Forms.Button();
            txtBuscar = new System.Windows.Forms.TextBox();
            panelPEDIDOSACTIVOS = new System.Windows.Forms.Panel();
            titlePEDIDOSACTI = new System.Windows.Forms.Label();
            panelCHATROJO = new System.Windows.Forms.Panel();
            labelTITLECHAT = new System.Windows.Forms.Label();
            panel1.SuspendLayout();
            panelDetallesPedidos.SuspendLayout();
            panelPedidos.SuspendLayout();
            panelCHATROJO.SuspendLayout();
            SuspendLayout();
            // 
            // panel1
            // 
            panel1.Controls.Add(panelDetallesPedidos);
            panel1.Controls.Add(panelPedidos);
            panel1.Controls.Add(panelCHATROJO);
            panel1.Location = new System.Drawing.Point(0, 0);
            panel1.Margin = new System.Windows.Forms.Padding(4);
            panel1.Name = "panel1";
            panel1.Size = new System.Drawing.Size(1297, 711);
            panel1.TabIndex = 0;
            // 
            // panelDetallesPedidos
            // 
            panelDetallesPedidos.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            panelDetallesPedidos.BackColor = System.Drawing.Color.FromArgb(224, 224, 224);
            panelDetallesPedidos.Controls.Add(vScrollBar2);
            panelDetallesPedidos.Controls.Add(panel3);
            panelDetallesPedidos.Controls.Add(label1);
            panelDetallesPedidos.Location = new System.Drawing.Point(632, 88);
            panelDetallesPedidos.Margin = new System.Windows.Forms.Padding(4);
            panelDetallesPedidos.Name = "panelDetallesPedidos";
            panelDetallesPedidos.Size = new System.Drawing.Size(651, 607);
            panelDetallesPedidos.TabIndex = 9;
            // 
            // vScrollBar2
            // 
            vScrollBar2.Location = new System.Drawing.Point(711, 0);
            vScrollBar2.Name = "vScrollBar2";
            vScrollBar2.Size = new System.Drawing.Size(17, 716);
            vScrollBar2.TabIndex = 11;
            // 
            // panel3
            // 
            panel3.BackColor = System.Drawing.SystemColors.Control;
            panel3.Location = new System.Drawing.Point(10, 41);
            panel3.Margin = new System.Windows.Forms.Padding(4);
            panel3.Name = "panel3";
            panel3.Size = new System.Drawing.Size(624, 314);
            panel3.TabIndex = 1;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new System.Drawing.Font("Stencil", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
            label1.Location = new System.Drawing.Point(4, 4);
            label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(327, 29);
            label1.TabIndex = 0;
            label1.Text = "Detalles de la Reserva";
            // 
            // panelPedidos
            // 
            panelPedidos.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            panelPedidos.BackColor = System.Drawing.Color.FromArgb(224, 224, 224);
            panelPedidos.Controls.Add(vScrollBar1);
            panelPedidos.Controls.Add(label2);
            panelPedidos.Controls.Add(btnBuscar);
            panelPedidos.Controls.Add(txtBuscar);
            panelPedidos.Controls.Add(panelPEDIDOSACTIVOS);
            panelPedidos.Controls.Add(titlePEDIDOSACTI);
            panelPedidos.Location = new System.Drawing.Point(14, 88);
            panelPedidos.Margin = new System.Windows.Forms.Padding(4);
            panelPedidos.Name = "panelPedidos";
            panelPedidos.Size = new System.Drawing.Size(610, 607);
            panelPedidos.TabIndex = 8;
            // 
            // vScrollBar1
            // 
            vScrollBar1.Location = new System.Drawing.Point(711, 0);
            vScrollBar1.Name = "vScrollBar1";
            vScrollBar1.Size = new System.Drawing.Size(17, 716);
            vScrollBar1.TabIndex = 10;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            label2.ForeColor = System.Drawing.Color.Gray;
            label2.Location = new System.Drawing.Point(385, 45);
            label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(143, 16);
            label2.TabIndex = 9;
            label2.Text = "Total reservas activas:";
            label2.Click += label2_Click;
            // 
            // btnBuscar
            // 
            btnBuscar.BackColor = System.Drawing.Color.FromArgb(224, 224, 224);
            btnBuscar.Cursor = System.Windows.Forms.Cursors.Hand;
            btnBuscar.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(224, 224, 224);
            btnBuscar.FlatAppearance.BorderSize = 0;
            btnBuscar.Font = new System.Drawing.Font("Stencil", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
            btnBuscar.ForeColor = System.Drawing.Color.White;
            btnBuscar.Image = (System.Drawing.Image)resources.GetObject("btnBuscar.Image");
            btnBuscar.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            btnBuscar.Location = new System.Drawing.Point(316, 32);
            btnBuscar.Margin = new System.Windows.Forms.Padding(4);
            btnBuscar.Name = "btnBuscar";
            btnBuscar.Size = new System.Drawing.Size(42, 43);
            btnBuscar.TabIndex = 8;
            btnBuscar.TabStop = false;
            btnBuscar.UseVisualStyleBackColor = false;
            // 
            // txtBuscar
            // 
            txtBuscar.ForeColor = System.Drawing.Color.FromArgb(224, 224, 224);
            txtBuscar.Location = new System.Drawing.Point(10, 41);
            txtBuscar.Margin = new System.Windows.Forms.Padding(4);
            txtBuscar.Name = "txtBuscar";
            txtBuscar.Size = new System.Drawing.Size(298, 23);
            txtBuscar.TabIndex = 7;
            txtBuscar.Text = " Buscar pedido...";            // 
            // panelPEDIDOSACTIVOS
            // 
            panelPEDIDOSACTIVOS.AutoScroll = true;
            panelPEDIDOSACTIVOS.BackColor = System.Drawing.SystemColors.Control;
            panelPEDIDOSACTIVOS.Location = new System.Drawing.Point(10, 88);
            panelPEDIDOSACTIVOS.Margin = new System.Windows.Forms.Padding(4);
            panelPEDIDOSACTIVOS.Name = "panelPEDIDOSACTIVOS";
            panelPEDIDOSACTIVOS.Size = new System.Drawing.Size(583, 485);
            panelPEDIDOSACTIVOS.TabIndex = 1;
            // 
            // titlePEDIDOSACTI
            // 
            titlePEDIDOSACTI.AutoSize = true;
            titlePEDIDOSACTI.Font = new System.Drawing.Font("Stencil", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
            titlePEDIDOSACTI.Location = new System.Drawing.Point(4, 4);
            titlePEDIDOSACTI.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            titlePEDIDOSACTI.Name = "titlePEDIDOSACTI";
            titlePEDIDOSACTI.Size = new System.Drawing.Size(363, 29);
            titlePEDIDOSACTI.TabIndex = 0;
            titlePEDIDOSACTI.Text = "Lista de Reservas Activas";
            // 
            // panelCHATROJO
            // 
            panelCHATROJO.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            panelCHATROJO.BackColor = System.Drawing.Color.FromArgb(211, 47, 47);
            panelCHATROJO.Controls.Add(labelTITLECHAT);
            panelCHATROJO.Location = new System.Drawing.Point(14, 14);
            panelCHATROJO.Margin = new System.Windows.Forms.Padding(4);
            panelCHATROJO.Name = "panelCHATROJO";
            panelCHATROJO.Size = new System.Drawing.Size(1269, 58);
            panelCHATROJO.TabIndex = 5;
            // 
            // labelTITLECHAT
            // 
            labelTITLECHAT.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            labelTITLECHAT.AutoSize = true;
            labelTITLECHAT.Font = new System.Drawing.Font("Stencil", 21.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
            labelTITLECHAT.ForeColor = System.Drawing.Color.White;
            labelTITLECHAT.Location = new System.Drawing.Point(472, 10);
            labelTITLECHAT.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            labelTITLECHAT.Name = "labelTITLECHAT";
            labelTITLECHAT.Size = new System.Drawing.Size(491, 34);
            labelTITLECHAT.TabIndex = 4;
            labelTITLECHAT.Text = "GESTIÓN DE RESERVAS DE MESAS";
            labelTITLECHAT.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // FormReservas
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(1297, 711);
            Controls.Add(panel1);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            Margin = new System.Windows.Forms.Padding(4);
            Name = "FormReservas";
            Text = "FormReservas";
            Load += FormReservas_Load;
            panel1.ResumeLayout(false);
            panelDetallesPedidos.ResumeLayout(false);
            panelDetallesPedidos.PerformLayout();
            panelPedidos.ResumeLayout(false);
            panelPedidos.PerformLayout();
            panelCHATROJO.ResumeLayout(false);
            panelCHATROJO.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panelCHATROJO;
        private System.Windows.Forms.Label labelTITLECHAT;
        private System.Windows.Forms.Panel panelDetallesPedidos;
        private System.Windows.Forms.VScrollBar vScrollBar2;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panelPedidos;
        private System.Windows.Forms.VScrollBar vScrollBar1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnBuscar;
        private System.Windows.Forms.TextBox txtBuscar;
        private System.Windows.Forms.Panel panelPEDIDOSACTIVOS;
        private System.Windows.Forms.Label titlePEDIDOSACTI;
    }
}