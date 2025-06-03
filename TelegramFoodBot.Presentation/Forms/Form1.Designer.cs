using System.Windows.Forms;

namespace TelegramFoodBot.Presentation.Forms
{
    partial class FormPrincipal
    {
        /// <summary>
        /// Variable del diseñador necesaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpiar los recursos que se estén usando.
        /// </summary>
        /// <param name="disposing">true si los recursos administrados se deben desechar; false en caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código generado por el Diseñador de Windows Forms

        /// <summary>
        /// Método necesario para admitir el Diseñador. No se puede modificar
        /// el contenido de este método con el editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormPrincipal));
            panelBarraTitulo = new Panel();
            labelDOMIBOT = new Label();
            btnMENUSIDEBAR = new PictureBox();
            SiderBar = new FlowLayoutPanel();
            panelINICIO = new Panel();
            btnINICIO = new Button();
            panelPEDIDO = new Panel();
            btnPEDIDO = new Button();
            panelRESERVA = new Panel();
            btnRESERVA = new Button();
            panelMENSAJES = new Panel();
            btnMENSAJES = new Button();
            panelPEDIDOS = new Panel();
            btnPEDIDOS = new Button();
            menuContainer = new FlowLayoutPanel();
            panelCONFIGURACION = new Panel();
            btnCONFIG = new Button();
            panelCONFIGMENU = new Panel();
            btnCONFIGMENU = new Button();
            panelCONFIGCUENTAS = new Panel();
            btnCONFIGCUENTAS = new Button();
            panelCONFIGMENSAJES = new Panel();
            btnCONFIGMENSAJE = new Button();
            panel2 = new Panel();
            labelVERSION = new Label();
            pictureBox1 = new PictureBox();
            menuTransicion = new Timer(components);
            sidebarTransicion = new Timer(components);
            panelContenedorForm = new Panel();
            panelBarraTitulo.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)btnMENUSIDEBAR).BeginInit();
            SiderBar.SuspendLayout();
            panelINICIO.SuspendLayout();
            panelPEDIDO.SuspendLayout();
            panelRESERVA.SuspendLayout();
            panelMENSAJES.SuspendLayout();
            panelPEDIDOS.SuspendLayout();
            menuContainer.SuspendLayout();
            panelCONFIGURACION.SuspendLayout();
            panelCONFIGMENU.SuspendLayout();
            panelCONFIGCUENTAS.SuspendLayout();
            panelCONFIGMENSAJES.SuspendLayout();
            panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            SuspendLayout();
            // 
            // panelBarraTitulo
            // 
            panelBarraTitulo.BackColor = System.Drawing.Color.FromArgb(211, 47, 47);
            panelBarraTitulo.Controls.Add(labelDOMIBOT);
            panelBarraTitulo.Controls.Add(btnMENUSIDEBAR);
            panelBarraTitulo.Dock = DockStyle.Top;
            panelBarraTitulo.Location = new System.Drawing.Point(0, 0);
            panelBarraTitulo.Margin = new Padding(4, 3, 4, 3);
            panelBarraTitulo.Name = "panelBarraTitulo";
            panelBarraTitulo.Size = new System.Drawing.Size(1366, 65);
            panelBarraTitulo.TabIndex = 0;
            panelBarraTitulo.MouseDown += panelBarraTitulo_MouseDown;
            // 
            // labelDOMIBOT
            // 
            labelDOMIBOT.AutoSize = true;
            labelDOMIBOT.Font = new System.Drawing.Font("Stencil", 26.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
            labelDOMIBOT.ForeColor = System.Drawing.Color.White;
            labelDOMIBOT.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            labelDOMIBOT.Location = new System.Drawing.Point(85, 8);
            labelDOMIBOT.Margin = new Padding(4, 0, 4, 0);
            labelDOMIBOT.Name = "labelDOMIBOT";
            labelDOMIBOT.Size = new System.Drawing.Size(177, 42);
            labelDOMIBOT.TabIndex = 1;
            labelDOMIBOT.Text = "DomiBot";
            // 
            // btnMENUSIDEBAR
            // 
            btnMENUSIDEBAR.Cursor = Cursors.Hand;
            btnMENUSIDEBAR.Image = (System.Drawing.Image)resources.GetObject("btnMENUSIDEBAR.Image");
            btnMENUSIDEBAR.Location = new System.Drawing.Point(4, 3);
            btnMENUSIDEBAR.Margin = new Padding(4, 3, 4, 3);
            btnMENUSIDEBAR.Name = "btnMENUSIDEBAR";
            btnMENUSIDEBAR.Size = new System.Drawing.Size(75, 58);
            btnMENUSIDEBAR.SizeMode = PictureBoxSizeMode.Zoom;
            btnMENUSIDEBAR.TabIndex = 1;
            btnMENUSIDEBAR.TabStop = false;
            btnMENUSIDEBAR.Click += pictureBox1_Click;
            // 
            // SiderBar
            // 
            SiderBar.BackColor = System.Drawing.Color.WhiteSmoke;
            SiderBar.Controls.Add(panelINICIO);
            SiderBar.Controls.Add(panelPEDIDO);
            SiderBar.Controls.Add(panelRESERVA);
            SiderBar.Controls.Add(panelMENSAJES);
            SiderBar.Controls.Add(panelPEDIDOS);
            SiderBar.Controls.Add(menuContainer);
            SiderBar.Controls.Add(panel2);
            SiderBar.Dock = DockStyle.Left;
            SiderBar.Location = new System.Drawing.Point(0, 65);
            SiderBar.Margin = new Padding(4, 3, 4, 3);
            SiderBar.Name = "SiderBar";
            SiderBar.Size = new System.Drawing.Size(72, 715);
            SiderBar.TabIndex = 1;
            // 
            // panelINICIO
            // 
            panelINICIO.Controls.Add(btnINICIO);
            panelINICIO.Location = new System.Drawing.Point(4, 3);
            panelINICIO.Margin = new Padding(4, 3, 4, 3);
            panelINICIO.Name = "panelINICIO";
            panelINICIO.Size = new System.Drawing.Size(343, 59);
            panelINICIO.TabIndex = 3;
            // 
            // btnINICIO
            // 
            btnINICIO.Cursor = Cursors.Hand;
            btnINICIO.Font = new System.Drawing.Font("Stencil", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
            btnINICIO.Image = (System.Drawing.Image)resources.GetObject("btnINICIO.Image");
            btnINICIO.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            btnINICIO.Location = new System.Drawing.Point(0, 0);
            btnINICIO.Margin = new Padding(4, 3, 4, 3);
            btnINICIO.Name = "btnINICIO";
            btnINICIO.Padding = new Padding(12, 0, 0, 0);
            btnINICIO.Size = new System.Drawing.Size(343, 59);
            btnINICIO.TabIndex = 2;
            btnINICIO.Text = "          INICIO";
            btnINICIO.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            btnINICIO.UseVisualStyleBackColor = true;
            btnINICIO.Click += btnINICIO_Click;
            // 
            // panelPEDIDO
            // 
            panelPEDIDO.Controls.Add(btnPEDIDO);
            panelPEDIDO.Location = new System.Drawing.Point(4, 68);
            panelPEDIDO.Margin = new Padding(4, 3, 4, 3);
            panelPEDIDO.Name = "panelPEDIDO";
            panelPEDIDO.Size = new System.Drawing.Size(343, 59);
            panelPEDIDO.TabIndex = 4;
            // 
            // btnPEDIDO
            // 
            btnPEDIDO.Cursor = Cursors.Hand;
            btnPEDIDO.Font = new System.Drawing.Font("Stencil", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
            btnPEDIDO.Image = (System.Drawing.Image)resources.GetObject("btnPEDIDO.Image");
            btnPEDIDO.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            btnPEDIDO.Location = new System.Drawing.Point(0, 0);
            btnPEDIDO.Margin = new Padding(4, 3, 4, 3);
            btnPEDIDO.Name = "btnPEDIDO";
            btnPEDIDO.Padding = new Padding(12, 0, 0, 0);
            btnPEDIDO.Size = new System.Drawing.Size(343, 59);
            btnPEDIDO.TabIndex = 2;
            btnPEDIDO.Text = "          PEDIDOS ACTIVOS";
            btnPEDIDO.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            btnPEDIDO.UseVisualStyleBackColor = true;
            btnPEDIDO.Click += btnPEDIDO_Click;
            // 
            // panelRESERVA
            // 
            panelRESERVA.Controls.Add(btnRESERVA);
            panelRESERVA.Location = new System.Drawing.Point(4, 133);
            panelRESERVA.Margin = new Padding(4, 3, 4, 3);
            panelRESERVA.Name = "panelRESERVA";
            panelRESERVA.Size = new System.Drawing.Size(343, 59);
            panelRESERVA.TabIndex = 4;
            // 
            // btnRESERVA
            // 
            btnRESERVA.Cursor = Cursors.Hand;
            btnRESERVA.Font = new System.Drawing.Font("Stencil", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
            btnRESERVA.Image = (System.Drawing.Image)resources.GetObject("btnRESERVA.Image");
            btnRESERVA.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            btnRESERVA.Location = new System.Drawing.Point(0, 0);
            btnRESERVA.Margin = new Padding(4, 3, 4, 3);
            btnRESERVA.Name = "btnRESERVA";
            btnRESERVA.Padding = new Padding(12, 0, 0, 0);
            btnRESERVA.Size = new System.Drawing.Size(343, 59);
            btnRESERVA.TabIndex = 2;
            btnRESERVA.Text = "          RESERVA DE MESAS";
            btnRESERVA.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            btnRESERVA.UseVisualStyleBackColor = true;
            btnRESERVA.Click += btnRESERVA_Click;
            // 
            // panelMENSAJES
            // 
            panelMENSAJES.Controls.Add(btnMENSAJES);
            panelMENSAJES.Location = new System.Drawing.Point(4, 198);
            panelMENSAJES.Margin = new Padding(4, 3, 4, 3);
            panelMENSAJES.Name = "panelMENSAJES";
            panelMENSAJES.Size = new System.Drawing.Size(343, 59);
            panelMENSAJES.TabIndex = 4;
            // 
            // btnMENSAJES
            // 
            btnMENSAJES.Cursor = Cursors.Hand;
            btnMENSAJES.Font = new System.Drawing.Font("Stencil", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
            btnMENSAJES.Image = (System.Drawing.Image)resources.GetObject("btnMENSAJES.Image");
            btnMENSAJES.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            btnMENSAJES.Location = new System.Drawing.Point(0, 0);
            btnMENSAJES.Margin = new Padding(4, 3, 4, 3);
            btnMENSAJES.Name = "btnMENSAJES";
            btnMENSAJES.Padding = new Padding(12, 0, 0, 0);
            btnMENSAJES.Size = new System.Drawing.Size(343, 59);
            btnMENSAJES.TabIndex = 2;
            btnMENSAJES.Text = "          MENSAJES CLIENTES";
            btnMENSAJES.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            btnMENSAJES.UseVisualStyleBackColor = true;
            btnMENSAJES.Click += btnMENSAJES_Click;
            // 
            // panelPEDIDOS
            // 
            panelPEDIDOS.Controls.Add(btnPEDIDOS);
            panelPEDIDOS.Location = new System.Drawing.Point(4, 263);
            panelPEDIDOS.Margin = new Padding(4, 3, 4, 3);
            panelPEDIDOS.Name = "panelPEDIDOS";
            panelPEDIDOS.Size = new System.Drawing.Size(343, 59);
            panelPEDIDOS.TabIndex = 4;
            // 
            // btnPEDIDOS
            // 
            btnPEDIDOS.Cursor = Cursors.Hand;
            btnPEDIDOS.Font = new System.Drawing.Font("Stencil", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
            btnPEDIDOS.Image = (System.Drawing.Image)resources.GetObject("btnPEDIDOS.Image");
            btnPEDIDOS.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            btnPEDIDOS.Location = new System.Drawing.Point(0, 0);
            btnPEDIDOS.Margin = new Padding(4, 3, 4, 3);
            btnPEDIDOS.Name = "btnPEDIDOS";
            btnPEDIDOS.Padding = new Padding(12, 0, 0, 0);
            btnPEDIDOS.Size = new System.Drawing.Size(343, 59);
            btnPEDIDOS.TabIndex = 2;
            btnPEDIDOS.Text = "          HISTORIAL PEDIDOS";
            btnPEDIDOS.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            btnPEDIDOS.UseVisualStyleBackColor = true;
            btnPEDIDOS.Click += btnPEDIDOS_Click;
            // 
            // menuContainer
            // 
            menuContainer.BackColor = System.Drawing.Color.WhiteSmoke;
            menuContainer.Controls.Add(panelCONFIGURACION);
            menuContainer.Controls.Add(panelCONFIGMENU);
            menuContainer.Controls.Add(panelCONFIGCUENTAS);
            menuContainer.Controls.Add(panelCONFIGMENSAJES);
            menuContainer.Location = new System.Drawing.Point(4, 328);
            menuContainer.Margin = new Padding(4, 3, 4, 3);
            menuContainer.Name = "menuContainer";
            menuContainer.Size = new System.Drawing.Size(349, 66);
            menuContainer.TabIndex = 6;
            // 
            // panelCONFIGURACION
            // 
            panelCONFIGURACION.Controls.Add(btnCONFIG);
            panelCONFIGURACION.Location = new System.Drawing.Point(4, 3);
            panelCONFIGURACION.Margin = new Padding(4, 3, 4, 3);
            panelCONFIGURACION.Name = "panelCONFIGURACION";
            panelCONFIGURACION.Size = new System.Drawing.Size(343, 59);
            panelCONFIGURACION.TabIndex = 4;
            // 
            // btnCONFIG
            // 
            btnCONFIG.Cursor = Cursors.Hand;
            btnCONFIG.Font = new System.Drawing.Font("Stencil", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
            btnCONFIG.Image = (System.Drawing.Image)resources.GetObject("btnCONFIG.Image");
            btnCONFIG.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            btnCONFIG.Location = new System.Drawing.Point(0, 0);
            btnCONFIG.Margin = new Padding(4, 3, 4, 3);
            btnCONFIG.Name = "btnCONFIG";
            btnCONFIG.Padding = new Padding(12, 0, 0, 0);
            btnCONFIG.Size = new System.Drawing.Size(343, 59);
            btnCONFIG.TabIndex = 2;
            btnCONFIG.Text = "          CONFIGURACION";
            btnCONFIG.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            btnCONFIG.UseVisualStyleBackColor = true;
            btnCONFIG.Click += btnCONFIG_Click;
            // 
            // panelCONFIGMENU
            // 
            panelCONFIGMENU.Controls.Add(btnCONFIGMENU);
            panelCONFIGMENU.Location = new System.Drawing.Point(4, 68);
            panelCONFIGMENU.Margin = new Padding(4, 3, 4, 3);
            panelCONFIGMENU.Name = "panelCONFIGMENU";
            panelCONFIGMENU.Size = new System.Drawing.Size(343, 59);
            panelCONFIGMENU.TabIndex = 5;
            // 
            // btnCONFIGMENU
            // 
            btnCONFIGMENU.BackColor = System.Drawing.Color.DarkGray;
            btnCONFIGMENU.Font = new System.Drawing.Font("Stencil", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
            btnCONFIGMENU.Image = (System.Drawing.Image)resources.GetObject("btnCONFIGMENU.Image");
            btnCONFIGMENU.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            btnCONFIGMENU.Location = new System.Drawing.Point(0, 0);
            btnCONFIGMENU.Margin = new Padding(4, 3, 4, 3);
            btnCONFIGMENU.Name = "btnCONFIGMENU";
            btnCONFIGMENU.Padding = new Padding(12, 0, 0, 0);
            btnCONFIGMENU.Size = new System.Drawing.Size(343, 59);
            btnCONFIGMENU.TabIndex = 2;
            btnCONFIGMENU.Text = "          MENU";
            btnCONFIGMENU.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            btnCONFIGMENU.UseVisualStyleBackColor = false;
            btnCONFIGMENU.Click += btnCONFIGMENU_Click;
            // 
            // panelCONFIGCUENTAS
            // 
            panelCONFIGCUENTAS.Controls.Add(btnCONFIGCUENTAS);
            panelCONFIGCUENTAS.Location = new System.Drawing.Point(4, 133);
            panelCONFIGCUENTAS.Margin = new Padding(4, 3, 4, 3);
            panelCONFIGCUENTAS.Name = "panelCONFIGCUENTAS";
            panelCONFIGCUENTAS.Size = new System.Drawing.Size(343, 59);
            panelCONFIGCUENTAS.TabIndex = 4;
            // 
            // btnCONFIGCUENTAS
            // 
            btnCONFIGCUENTAS.BackColor = System.Drawing.Color.DarkGray;
            btnCONFIGCUENTAS.Font = new System.Drawing.Font("Stencil", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
            btnCONFIGCUENTAS.Image = (System.Drawing.Image)resources.GetObject("btnCONFIGCUENTAS.Image");
            btnCONFIGCUENTAS.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            btnCONFIGCUENTAS.Location = new System.Drawing.Point(0, 0);
            btnCONFIGCUENTAS.Margin = new Padding(4, 3, 4, 3);
            btnCONFIGCUENTAS.Name = "btnCONFIGCUENTAS";
            btnCONFIGCUENTAS.Padding = new Padding(12, 0, 0, 0);
            btnCONFIGCUENTAS.Size = new System.Drawing.Size(343, 59);
            btnCONFIGCUENTAS.TabIndex = 2;
            btnCONFIGCUENTAS.Text = "          CUENTAS DE PAGO";
            btnCONFIGCUENTAS.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            btnCONFIGCUENTAS.UseVisualStyleBackColor = false;
            btnCONFIGCUENTAS.Click += btnCONFIGCUENTAS_Click;
            // 
            // panelCONFIGMENSAJES
            // 
            panelCONFIGMENSAJES.Controls.Add(btnCONFIGMENSAJE);
            panelCONFIGMENSAJES.Location = new System.Drawing.Point(4, 198);
            panelCONFIGMENSAJES.Margin = new Padding(4, 3, 4, 3);
            panelCONFIGMENSAJES.Name = "panelCONFIGMENSAJES";
            panelCONFIGMENSAJES.Size = new System.Drawing.Size(343, 59);
            panelCONFIGMENSAJES.TabIndex = 4;
            // 
            // btnCONFIGMENSAJE
            // 
            btnCONFIGMENSAJE.BackColor = System.Drawing.Color.DarkGray;
            btnCONFIGMENSAJE.Font = new System.Drawing.Font("Stencil", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
            btnCONFIGMENSAJE.Image = (System.Drawing.Image)resources.GetObject("btnCONFIGMENSAJE.Image");
            btnCONFIGMENSAJE.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            btnCONFIGMENSAJE.Location = new System.Drawing.Point(0, 0);
            btnCONFIGMENSAJE.Margin = new Padding(4, 3, 4, 3);
            btnCONFIGMENSAJE.Name = "btnCONFIGMENSAJE";
            btnCONFIGMENSAJE.Padding = new Padding(12, 0, 0, 0);
            btnCONFIGMENSAJE.Size = new System.Drawing.Size(343, 59);
            btnCONFIGMENSAJE.TabIndex = 2;
            btnCONFIGMENSAJE.Text = "          MENSAJES";
            btnCONFIGMENSAJE.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            btnCONFIGMENSAJE.UseVisualStyleBackColor = false;
            btnCONFIGMENSAJE.Click += btnCONFIGMENSAJE_Click;
            // 
            // panel2
            // 
            panel2.Controls.Add(labelVERSION);
            panel2.Controls.Add(pictureBox1);
            panel2.Location = new System.Drawing.Point(4, 400);
            panel2.Margin = new Padding(4, 3, 4, 3);
            panel2.Name = "panel2";
            panel2.Size = new System.Drawing.Size(348, 192);
            panel2.TabIndex = 3;
            // 
            // labelVERSION
            // 
            labelVERSION.AutoSize = true;
            labelVERSION.Font = new System.Drawing.Font("Stencil", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
            labelVERSION.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            labelVERSION.Location = new System.Drawing.Point(104, 168);
            labelVERSION.Margin = new Padding(4, 0, 4, 0);
            labelVERSION.Name = "labelVERSION";
            labelVERSION.Size = new System.Drawing.Size(128, 19);
            labelVERSION.TabIndex = 0;
            labelVERSION.Text = "DomiBot V1.0";
            // 
            // pictureBox1
            // 
            pictureBox1.Image = (System.Drawing.Image)resources.GetObject("pictureBox1.Image");
            pictureBox1.Location = new System.Drawing.Point(0, 3);
            pictureBox1.Margin = new Padding(4, 3, 4, 3);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new System.Drawing.Size(344, 162);
            pictureBox1.SizeMode = PictureBoxSizeMode.Zoom;
            pictureBox1.TabIndex = 2;
            pictureBox1.TabStop = false;
            pictureBox1.Click += pictureBox1_Click_1;
            // 
            // menuTransicion
            // 
            menuTransicion.Interval = 10;
            menuTransicion.Tick += menuTransicion_Tick;
            // 
            // sidebarTransicion
            // 
            sidebarTransicion.Interval = 10;
            sidebarTransicion.Tick += sidebarTransicion_Tick;
            // 
            // panelContenedorForm
            // 
            panelContenedorForm.Location = new System.Drawing.Point(80, 66);
            panelContenedorForm.Margin = new Padding(4, 3, 4, 3);
            panelContenedorForm.Name = "panelContenedorForm";
            panelContenedorForm.Size = new System.Drawing.Size(1366, 780);
            panelContenedorForm.TabIndex = 2;
            // 
            // FormPrincipal
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(1366, 780);
            Controls.Add(SiderBar);
            Controls.Add(panelBarraTitulo);
            Controls.Add(panelContenedorForm);
            FormBorderStyle = FormBorderStyle.None;
            Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
            Margin = new Padding(4, 3, 4, 3);
            Name = "FormPrincipal";
            Text = "DomiBot";
            panelBarraTitulo.ResumeLayout(false);
            panelBarraTitulo.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)btnMENUSIDEBAR).EndInit();
            SiderBar.ResumeLayout(false);
            panelINICIO.ResumeLayout(false);
            panelPEDIDO.ResumeLayout(false);
            panelRESERVA.ResumeLayout(false);
            panelMENSAJES.ResumeLayout(false);
            panelPEDIDOS.ResumeLayout(false);
            menuContainer.ResumeLayout(false);
            panelCONFIGURACION.ResumeLayout(false);
            panelCONFIGMENU.ResumeLayout(false);
            panelCONFIGCUENTAS.ResumeLayout(false);
            panelCONFIGMENSAJES.ResumeLayout(false);
            panel2.ResumeLayout(false);
            panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelBarraTitulo;
        private System.Windows.Forms.PictureBox btnMENUSIDEBAR;
        private System.Windows.Forms.Label labelDOMIBOT;
        private System.Windows.Forms.FlowLayoutPanel SiderBar;
        private System.Windows.Forms.Button btnINICIO;
        private System.Windows.Forms.Panel panelINICIO;
        private System.Windows.Forms.Panel panelPEDIDO;
        private System.Windows.Forms.Button btnPEDIDO;
        private System.Windows.Forms.Panel panelRESERVA;
        private System.Windows.Forms.Button btnRESERVA;
        private System.Windows.Forms.Panel panelMENSAJES;
        private System.Windows.Forms.Button btnMENSAJES;
        private System.Windows.Forms.Panel panelPEDIDOS;
        private System.Windows.Forms.Panel panelCONFIGURACION;
        private System.Windows.Forms.Button btnCONFIG;
        private System.Windows.Forms.Button btnPEDIDOS;
        private System.Windows.Forms.Panel panelCONFIGMENU;
        private System.Windows.Forms.Button btnCONFIGMENU;
        private System.Windows.Forms.Panel panelCONFIGCUENTAS;
        private System.Windows.Forms.Button btnCONFIGCUENTAS;
        private System.Windows.Forms.Panel panelCONFIGMENSAJES;
        private System.Windows.Forms.Button btnCONFIGMENSAJE;
        private System.Windows.Forms.FlowLayoutPanel menuContainer;
        private System.Windows.Forms.Timer menuTransicion;
        private System.Windows.Forms.Timer sidebarTransicion;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label labelVERSION;
        private System.Windows.Forms.Button btnCerrar;
        private System.Windows.Forms.Button btnMinimizar;
        private System.Windows.Forms.Panel panelContenedorForm;
    }
}

