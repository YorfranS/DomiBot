namespace TelegramFoodBot.Presentation.Forms
{
    partial class FormChats
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormChats));
            panel1 = new System.Windows.Forms.Panel();
            panel2 = new System.Windows.Forms.Panel();
            panelMensajes = new System.Windows.Forms.FlowLayoutPanel();
            paneLENVIARMSJ = new System.Windows.Forms.Panel();
            btnEnviar = new System.Windows.Forms.Button();
            txtMensaje = new System.Windows.Forms.TextBox();
            panelCHATROJO = new System.Windows.Forms.Panel();
            labelTITLECHAT = new System.Windows.Forms.Label();
            panelleftchat = new System.Windows.Forms.Panel();
            lstClientes = new System.Windows.Forms.ListBox();
            btnBuscar = new System.Windows.Forms.Button();
            txtBuscar = new System.Windows.Forms.TextBox();
            vScrollBar1 = new System.Windows.Forms.VScrollBar();
            labelCLIENTES = new System.Windows.Forms.Label();
            panel1.SuspendLayout();
            panel2.SuspendLayout();
            paneLENVIARMSJ.SuspendLayout();
            panelCHATROJO.SuspendLayout();
            panelleftchat.SuspendLayout();
            SuspendLayout();
            // 
            // panel1
            // 
            panel1.BackColor = System.Drawing.Color.White;
            panel1.Controls.Add(panel2);
            panel1.Controls.Add(paneLENVIARMSJ);
            panel1.Controls.Add(panelCHATROJO);
            panel1.Controls.Add(panelleftchat);
            panel1.Location = new System.Drawing.Point(0, 0);
            panel1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            panel1.Name = "panel1";
            panel1.Size = new System.Drawing.Size(1297, 711);
            panel1.TabIndex = 0;
            // 
            // panel2
            // 
            panel2.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            panel2.BackColor = System.Drawing.Color.WhiteSmoke;
            panel2.Controls.Add(panelMensajes);
            panel2.Location = new System.Drawing.Point(419, 75);
            panel2.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            panel2.Name = "panel2";
            panel2.Size = new System.Drawing.Size(863, 545);
            panel2.TabIndex = 0;
            // 
            // panelMensajes
            // 
            panelMensajes.AutoScroll = true;
            panelMensajes.Dock = System.Windows.Forms.DockStyle.Fill;
            panelMensajes.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            panelMensajes.Location = new System.Drawing.Point(0, 0);
            panelMensajes.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            panelMensajes.Name = "panelMensajes";
            panelMensajes.Size = new System.Drawing.Size(863, 545);
            panelMensajes.TabIndex = 0;
            panelMensajes.WrapContents = false;
            panelMensajes.Paint += panelMensajes_Paint;
            // 
            // paneLENVIARMSJ
            // 
            paneLENVIARMSJ.BackColor = System.Drawing.Color.FromArgb(224, 224, 224);
            paneLENVIARMSJ.Controls.Add(btnEnviar);
            paneLENVIARMSJ.Controls.Add(txtMensaje);
            paneLENVIARMSJ.Dock = System.Windows.Forms.DockStyle.Bottom;
            paneLENVIARMSJ.Location = new System.Drawing.Point(0, 634);
            paneLENVIARMSJ.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            paneLENVIARMSJ.Name = "paneLENVIARMSJ";
            paneLENVIARMSJ.Size = new System.Drawing.Size(1297, 77);
            paneLENVIARMSJ.TabIndex = 1;
            // 
            // btnEnviar
            // 
            btnEnviar.BackColor = System.Drawing.Color.FromArgb(211, 47, 47);
            btnEnviar.Cursor = System.Windows.Forms.Cursors.Hand;
            btnEnviar.Font = new System.Drawing.Font("Stencil", 11.25F, System.Drawing.FontStyle.Bold);
            btnEnviar.ForeColor = System.Drawing.Color.White;
            btnEnviar.Image = (System.Drawing.Image)resources.GetObject("btnEnviar.Image");
            btnEnviar.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            btnEnviar.Location = new System.Drawing.Point(1130, 8);
            btnEnviar.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btnEnviar.Name = "btnEnviar";
            btnEnviar.Size = new System.Drawing.Size(152, 57);
            btnEnviar.TabIndex = 0;
            btnEnviar.Text = "     ENVIAR";
            btnEnviar.UseVisualStyleBackColor = false;
            btnEnviar.Click += btnEnviar_Click;
            // 
            // txtMensaje
            // 
            txtMensaje.Location = new System.Drawing.Point(419, 25);
            txtMensaje.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            txtMensaje.Name = "txtMensaje";
            txtMensaje.Size = new System.Drawing.Size(703, 23);
            txtMensaje.TabIndex = 1;
            // 
            // panelCHATROJO
            // 
            panelCHATROJO.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            panelCHATROJO.BackColor = System.Drawing.Color.FromArgb(211, 47, 47);
            panelCHATROJO.Controls.Add(labelTITLECHAT);
            panelCHATROJO.Location = new System.Drawing.Point(14, 16);
            panelCHATROJO.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            panelCHATROJO.Name = "panelCHATROJO";
            panelCHATROJO.Size = new System.Drawing.Size(1269, 59);
            panelCHATROJO.TabIndex = 2;
            // 
            // labelTITLECHAT
            // 
            labelTITLECHAT.Anchor = System.Windows.Forms.AnchorStyles.Top;
            labelTITLECHAT.AutoSize = true;
            labelTITLECHAT.Font = new System.Drawing.Font("Stencil", 21.75F, System.Drawing.FontStyle.Bold);
            labelTITLECHAT.ForeColor = System.Drawing.Color.White;
            labelTITLECHAT.Location = new System.Drawing.Point(426, 10);
            labelTITLECHAT.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            labelTITLECHAT.Name = "labelTITLECHAT";
            labelTITLECHAT.Size = new System.Drawing.Size(356, 34);
            labelTITLECHAT.TabIndex = 0;
            labelTITLECHAT.Text = "MENSAJES DE CLIENTES";
            // 
            // panelleftchat
            // 
            panelleftchat.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
            panelleftchat.BackColor = System.Drawing.Color.FromArgb(224, 224, 224);
            panelleftchat.Controls.Add(lstClientes);
            panelleftchat.Controls.Add(btnBuscar);
            panelleftchat.Controls.Add(txtBuscar);
            panelleftchat.Controls.Add(vScrollBar1);
            panelleftchat.Controls.Add(labelCLIENTES);
            panelleftchat.Location = new System.Drawing.Point(14, 74);
            panelleftchat.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            panelleftchat.Name = "panelleftchat";
            panelleftchat.Size = new System.Drawing.Size(405, 624);
            panelleftchat.TabIndex = 3;
            // 
            // lstClientes
            // 
            lstClientes.ItemHeight = 15;
            lstClientes.Location = new System.Drawing.Point(18, 104);
            lstClientes.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            lstClientes.Name = "lstClientes";
            lstClientes.Size = new System.Drawing.Size(349, 454);
            lstClientes.TabIndex = 0;
            lstClientes.SelectedIndexChanged += lstClientes_SelectedIndexChanged;
            // 
            // btnBuscar
            // 
            btnBuscar.BackColor = System.Drawing.Color.Silver;
            btnBuscar.Cursor = System.Windows.Forms.Cursors.Hand;
            btnBuscar.FlatAppearance.BorderSize = 0;
            btnBuscar.Font = new System.Drawing.Font("Stencil", 11.25F, System.Drawing.FontStyle.Bold);
            btnBuscar.ForeColor = System.Drawing.Color.White;
            btnBuscar.Image = (System.Drawing.Image)resources.GetObject("btnBuscar.Image");
            btnBuscar.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            btnBuscar.Location = new System.Drawing.Point(324, 52);
            btnBuscar.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            btnBuscar.Name = "btnBuscar";
            btnBuscar.Size = new System.Drawing.Size(42, 43);
            btnBuscar.TabIndex = 1;
            btnBuscar.UseVisualStyleBackColor = false;
            btnBuscar.Click += btnBuscar_Click;
            // 
            // txtBuscar
            // 
            txtBuscar.ForeColor = System.Drawing.Color.Gray;
            txtBuscar.Location = new System.Drawing.Point(18, 61);
            txtBuscar.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            txtBuscar.Name = "txtBuscar";
            txtBuscar.Size = new System.Drawing.Size(298, 23);
            txtBuscar.TabIndex = 2;
            txtBuscar.Text = " Buscar cliente...";
            // 
            // vScrollBar1
            // 
            vScrollBar1.Location = new System.Drawing.Point(384, 3);
            vScrollBar1.Name = "vScrollBar1";
            vScrollBar1.Size = new System.Drawing.Size(18, 729);
            vScrollBar1.TabIndex = 3;
            // 
            // labelCLIENTES
            // 
            labelCLIENTES.AutoSize = true;
            labelCLIENTES.BackColor = System.Drawing.Color.Silver;
            labelCLIENTES.Font = new System.Drawing.Font("Stencil", 18F, System.Drawing.FontStyle.Bold);
            labelCLIENTES.ForeColor = System.Drawing.Color.Black;
            labelCLIENTES.Image = (System.Drawing.Image)resources.GetObject("labelCLIENTES.Image");
            labelCLIENTES.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            labelCLIENTES.Location = new System.Drawing.Point(99, 20);
            labelCLIENTES.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            labelCLIENTES.Name = "labelCLIENTES";
            labelCLIENTES.Size = new System.Drawing.Size(171, 29);
            labelCLIENTES.TabIndex = 4;
            labelCLIENTES.Text = "      CLIENTES";
            // 
            // FormChats
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(1297, 711);
            Controls.Add(panel1);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            Name = "FormChats";
            Text = "FormChats";
            panel1.ResumeLayout(false);
            panel2.ResumeLayout(false);
            paneLENVIARMSJ.ResumeLayout(false);
            paneLENVIARMSJ.PerformLayout();
            panelCHATROJO.ResumeLayout(false);
            panelCHATROJO.PerformLayout();
            panelleftchat.ResumeLayout(false);
            panelleftchat.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panelleftchat;
        private System.Windows.Forms.Panel panelCHATROJO;
        private System.Windows.Forms.Label labelTITLECHAT;
        private System.Windows.Forms.Panel paneLENVIARMSJ;
        private System.Windows.Forms.VScrollBar vScrollBar1;
        private System.Windows.Forms.Label labelCLIENTES;
        private System.Windows.Forms.Button btnEnviar;
        private System.Windows.Forms.TextBox txtMensaje;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.TextBox txtBuscar;
        private System.Windows.Forms.Button btnBuscar;
        private System.Windows.Forms.ListBox lstClientes;
        private System.Windows.Forms.FlowLayoutPanel panelMensajes;
    }
}
