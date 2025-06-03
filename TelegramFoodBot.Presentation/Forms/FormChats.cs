using TelegramFoodBot.Entities.Models;
using System;
using System.IO;
using System.Windows.Forms;
using System.Drawing;
using TelegramFoodBot.Business.Services;
using Entidades = TelegramFoodBot.Entities.Models;

namespace TelegramFoodBot.Presentation.Forms
{
    public partial class FormChats : Form
    {
        private readonly EnhancedTelegramService _telegramService;
        private readonly ChatService _chatService = new ChatService();
        private long _selectedClientId;        public FormChats()
        {
            InitializeComponent();

            _telegramService = EnhancedTelegramService.Instance;
            _telegramService.MessageReceived += OnTelegramMessageReceived;
            

            ActualizarListaClientes();
        }

        private void OnTelegramMessageReceived(object sender, Entidades.MessageEventArgs e)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(() => MostrarMensaje(e.Message)));
            }
            else
            {
                MostrarMensaje(e.Message);
            }
        }

        private void MostrarMensaje(Entidades.Message mensaje)
        {
            // Guardar cliente y mensaje
            if (mensaje.From != null)
            {
                _chatService.AddClient(new Entidades.Client
                {
                    Id = mensaje.ClientId,
                    Name = mensaje.From.FirstName
                });
            }

            _chatService.SaveMessage(mensaje);
            ActualizarListaClientes();

            // Si es el cliente actualmente seleccionado, mostrar sus mensajes
            if (mensaje.ClientId == _selectedClientId)
                MostrarMensajesCliente(_selectedClientId);
        }

        private void ActualizarListaClientes()
        {
            var clientes = _chatService.GetClientsOrderedByLastMessage();
            lstClientes.Items.Clear();
            foreach (var cliente in clientes)
            {
                lstClientes.Items.Add($"{cliente.Id} - {cliente.Name}");
            }
        }

        private void MostrarMensajesCliente(long clientId)
        {
            panelMensajes.Controls.Clear();
            var mensajes = _chatService.GetClientMessages(clientId);

            foreach (var msg in mensajes)
            {
                var contenedor = new Panel
                {
                    AutoSize = true,
                    BackColor = Color.Transparent,
                    Padding = new Padding(10),
                    Margin = new Padding(10),
                    MaximumSize = new Size(1000, 0), // Limita ancho del mensaje
                };

                if (!string.IsNullOrEmpty(msg.PhotoBase64))
                {
                    Image imagen = Base64ToImage(msg.PhotoBase64);

                    int maxAncho = 100;
                    int maxAlto = 100;

                    int anchoFinal = imagen.Width > maxAncho ? maxAncho : imagen.Width;
                    int altoFinal = imagen.Height > maxAlto ? maxAlto : imagen.Height;

                    var picture = new PictureBox
                    {
                        Width = anchoFinal,
                        Height = altoFinal,
                        SizeMode = PictureBoxSizeMode.Zoom,
                        Image = imagen,
                        Cursor = Cursors.Hand,
                        Anchor = AnchorStyles.Left
                    };

                    // Abrir imagen completa al hacer clic
                    picture.Click += (s, e) =>
                    {
                        var formImagen = new Form
                        {
                            Text = "Imagen completa",
                            Size = new Size(imagen.Width + 40, imagen.Height + 60),
                            StartPosition = FormStartPosition.CenterParent
                        };

                        var panel = new Panel
                        {
                            Dock = DockStyle.Fill,
                            AutoScroll = true
                        };

                        var fullPicture = new PictureBox
                        {
                            Image = imagen,
                            SizeMode = PictureBoxSizeMode.AutoSize
                        };

                        panel.Controls.Add(fullPicture);
                        formImagen.Controls.Add(panel);
                        formImagen.ShowDialog();
                    };

                    contenedor.Controls.Add(picture);
                }

                if (!string.IsNullOrWhiteSpace(msg.Text))
                {
                    var label = new Label
                    {
                        AutoSize = true,
                        Font = new Font("Segoe UI", 10),
                        Margin = new Padding(5),
                        MaximumSize = new Size(600, 0),
                        BackColor = msg.IsFromAdmin ? Color.LightGreen : Color.LightGray,
                        ForeColor = Color.Black,
                        Padding = new Padding(10),
                        Text = $"{msg.Text}"
                    };

                    contenedor.Controls.Add(label);
                }

                // Alinear contenedor a la izquierda o derecha
                contenedor.Dock = DockStyle.Top;
                contenedor.Padding = new Padding(msg.IsFromAdmin ? 300 : 5, 5, 5, 5);

                panelMensajes.Controls.Add(contenedor);
            }

            // Scroll automático
            if (panelMensajes.Controls.Count > 0)
            {
                var ultimoControl = panelMensajes.Controls[panelMensajes.Controls.Count - 1];
                panelMensajes.ScrollControlIntoView(ultimoControl);
            }
        }



        private Image Base64ToImage(string base64)
        {
            byte[] bytes = Convert.FromBase64String(base64);
            using var ms = new MemoryStream(bytes);
            return Image.FromStream(ms);
        }


        private void lstClientes_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstClientes.SelectedItem == null) return;

            string selected = lstClientes.SelectedItem.ToString();
            if (long.TryParse(selected.Split('-')[0].Trim(), out long clientId))
            {
                _selectedClientId = clientId;
                MostrarMensajesCliente(clientId);
            }
        }

        private void btnEnviar_Click(object sender, EventArgs e)
        {
            string texto = txtMensaje.Text.Trim();
            if (!string.IsNullOrEmpty(texto) && _selectedClientId != 0)
            {
                _telegramService.SendMessage(_selectedClientId, texto);

                var mensaje = new Entidades.Message
                {
                    Text = texto,
                    Timestamp = DateTime.Now,
                    IsFromAdmin = true,
                    ClientId = _selectedClientId,
                    From = null
                };

                MostrarMensaje(mensaje);
            }

            txtMensaje.Clear();
        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            string textoBuscar = txtBuscar.Text.Trim().ToLower();

            if (string.IsNullOrEmpty(textoBuscar) || textoBuscar == "buscar cliente...")
            {
                MessageBox.Show("Por favor ingrese un nombre para buscar.", "Búsqueda", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            bool encontrado = false;

            for (int i = 0; i < lstClientes.Items.Count; i++)
            {
                string cliente = lstClientes.Items[i].ToString().ToLower();
                if (cliente.Contains(textoBuscar))
                {
                    lstClientes.SelectedIndex = i;
                    encontrado = true;
                    break;
                }
            }

            if (!encontrado)
            {
                MessageBox.Show("Cliente no encontrado.", "Búsqueda", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }


        }

        private void panelMensajes_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
