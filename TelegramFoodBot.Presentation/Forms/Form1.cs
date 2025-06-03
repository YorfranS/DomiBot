using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TelegramFoodBot.Presentation.Forms
{
    public partial class FormPrincipal : Form
    {
        public FormPrincipal()
        {
            InitializeComponent();
            AbrirFormEnPanel(new FormInicio());
            SeleccionarBotonMenu(btnINICIO);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.MinimumSize = new Size(1024, 600); // previene que se encoja demasiado
            this.AutoScaleMode = AutoScaleMode.Dpi;
            this.FormBorderStyle = FormBorderStyle.Sizable;
        }

        bool menuExpand = false;

        //Método para Abrir formularios en el panel Contenedor
        private void AbrirFormEnPanel(object formHijo)
        {
            if (this.panelContenedorForm.Controls.Count > 0)
                this.panelContenedorForm.Controls.RemoveAt(0);
            Form fh = formHijo as Form;
            fh.TopLevel = false;
            fh.FormBorderStyle = FormBorderStyle.None;
            fh.Dock = DockStyle.Fill;
            this.panelContenedorForm.Controls.Add(fh);
            this.panelContenedorForm.Tag = fh;
            fh.Show();
        }

        // Colores para el botón seleccionado y por defecto
        private Color colorSeleccionado = Color.FromArgb(52, 152, 219); // Azul ejemplo
        private Color colorPorDefecto = SystemColors.Control; // O el color que uses normalmente

        private void SeleccionarBotonMenu(Button boton)
        {
            // Restaurar el color del botón previamente seleccionado
            if (botonSeleccionado != null)
                botonSeleccionado.BackColor = colorPorDefecto;

            // Cambiar el color del botón actual
            boton.BackColor = colorSeleccionado;
            botonSeleccionado = boton;
        }

        private void menuTransicion_Tick(object sender, EventArgs e)
        {
            if (menuExpand == false)
            {
                menuContainer.Height += 10;
                if(menuContainer.Height >= 188)
                {
                    menuTransicion.Stop();
                    menuExpand = true;
                    
                }
            }
            else
            {
                menuContainer.Height -= 10;
                if(menuContainer.Height <= 57)
                {
                    menuTransicion.Stop();
                    menuExpand = false;
                }
            }
        }

        private void btnCONFIG_Click(object sender, EventArgs e)
        {
            menuTransicion.Start();
        }

        bool sidebarExpand = false;
        private void pictureBox1_Click(object sender, EventArgs e)
        {
            sidebarTransicion.Start();  
        }

        private void sidebarTransicion_Tick(object sender, EventArgs e)
        {
            if(sidebarExpand)
            {
                SiderBar.Width -= 5;
                if (SiderBar.Width <= 72)
                {
                    
                    sidebarExpand = false;
                    sidebarTransicion.Stop();
                }
            }
            else
            {
                SiderBar.Width += 5;
                if (SiderBar.Width >= 310)
                {
                    
                    sidebarExpand = true;
                    sidebarTransicion.Stop();
                }
            }
        }

        private void pictureBox1_Click_1(object sender, EventArgs e)
        {

        }

        //  Función de Cerrar y Minimizar el formulario
        private void btnMinimizar_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void btnCerrar_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("¿Está seguro de cerrar?", "Alerta¡¡", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                Application.Exit();
            }
        }

        //  Función de arrastrar el formulario
        [DllImport("user32.DLL", EntryPoint = "ReleaseCapture")]
        private extern static void ReleaseCapture();
        [DllImport("user32.DLL", EntryPoint = "SendMessage")]
        private extern static void SendMessage(System.IntPtr hWnd, int wMsg, int wParam, int lParam);

        private void panelBarraTitulo_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(this.Handle, 0x112, 0xf012, 0);
        }

        // Referencia al botón actualmente seleccionado
        private Button botonSeleccionado = null;

        private void btnINICIO_Click(object sender, EventArgs e)
        {
            AbrirFormEnPanel(new FormInicio());
            SeleccionarBotonMenu((Button)sender);
        }

        private void btnPEDIDO_Click(object sender, EventArgs e)
        {
            AbrirFormEnPanel(new FormPedidos());
            SeleccionarBotonMenu((Button)sender);
        }

        private void btnRESERVA_Click(object sender, EventArgs e)
        {
            AbrirFormEnPanel(new FormReservas());
            SeleccionarBotonMenu((Button)sender);
        }

        private void btnMENSAJES_Click(object sender, EventArgs e)
        {
            AbrirFormEnPanel(new FormChats());
            SeleccionarBotonMenu((Button)sender);
        }

        private void btnPEDIDOS_Click(object sender, EventArgs e)
        {
            AbrirFormEnPanel(new FormHistorial());
            SeleccionarBotonMenu((Button)sender);
        }

        private void btnCONFIGMENU_Click(object sender, EventArgs e)
        {
            AbrirFormEnPanel(new FormConfiMenu());
            SeleccionarBotonMenu((Button)sender);
        }

        private void btnCONFIGCUENTAS_Click(object sender, EventArgs e)
        {
            AbrirFormEnPanel(new FormConfiPagos());
            SeleccionarBotonMenu((Button)sender);
        }

        private void btnCONFIGMENSAJE_Click(object sender, EventArgs e)
        {
            AbrirFormEnPanel(new FormConfiMensajes());
            SeleccionarBotonMenu((Button)sender);
        }

        
    }
}
