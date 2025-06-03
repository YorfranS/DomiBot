using System;
using System.Windows.Forms;
using TelegramFoodBot.Data;

namespace TelegramFoodBot.Presentation.Forms
{
    public partial class Login : Form
    {
        private readonly AdminRepository _adminRepo = new AdminRepository();

        public Login()
        {
            InitializeComponent();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            string usuario = txtUsuario.Text.Trim();
            string contrasena = txtContrasena.Text;

            if (_adminRepo.ValidarCredenciales(usuario, contrasena))
            {
                this.DialogResult = DialogResult.OK; // Esto cierra el login y permite abrir FormPrincipal
            }
            else
            {
                MessageBox.Show("❌ Usuario o contraseña incorrectos", "Acceso denegado", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}