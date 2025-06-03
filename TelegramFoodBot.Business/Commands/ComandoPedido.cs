using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramFoodBot.Data;
using TelegramFoodBot.Entities.Models;
using TelegramFoodBot.Business.Services;
using AppMessage = TelegramFoodBot.Entities.Models.Message;
using TelegramMessage = Telegram.Bot.Types.Message;

namespace TelegramFoodBot.Business.Commands
{
    public class ComandoPedido
    {
        private readonly TelegramBotClient _bot;
        private readonly Action<AppMessage> _onMessage;
        private readonly Dictionary<long, PedidoEnProceso> _pedidosEnProceso = new();
        private readonly ProductoService _productoService = new ProductoService();
        private readonly CategoriaService _categoriaService = new CategoriaService();
        private static int _contadorPedidosDelDia = 0;
        private static DateTime _fechaUltimoPedido = DateTime.MinValue;

        public ComandoPedido(TelegramBotClient bot, Action<AppMessage> onMessage)
        {
            _bot = bot;
            _onMessage = onMessage;
        }

        public bool EnPedido(long clientId) => _pedidosEnProceso.ContainsKey(clientId);        public async Task Ejecutar(TelegramMessage message)
        {
            long clientId = message.From.Id;
            string texto = message.Text?.Trim() ?? "";

            // Verificar y crear cliente automáticamente si no existe
            var clienteRepo = new ClienteRepository();
            if (!clienteRepo.ExisteCliente(clientId))
            {
                var nuevoCliente = new Client
                {
                    Id = clientId,
                    Name = message.From.FirstName ?? "Usuario",
                    Username = message.From.Username,
                    Phone = null // Se puede agregar más tarde
                };
                clienteRepo.AgregarCliente(nuevoCliente);
            }

            if (!_pedidosEnProceso.ContainsKey(clientId))
            {
                _pedidosEnProceso[clientId] = new PedidoEnProceso
                {
                    ClienteId = clientId,
                    PasoActual = PasoPedido.Direccion
                };                // Obtener productos en promoción desde la base de datos
                var productosEnPromocion = _productoService.ObtenerProductosEnPromocion();
                
                // Construir mensaje de bienvenida personalizado con promociones actuales
                var mensajeBienvenida = "🌟 *¡Hola! Bienvenido a nuestro servicio de delivery* 🌟\n\n" +
                                        "😋 *¿Qué se te antoja hoy?*\n\n";

                // Agregar promociones si existen
                if (productosEnPromocion.Count > 0)
                {
                    mensajeBienvenida += "🔥 *OFERTAS ESPECIALES:*\n";
                    
                    // Mostrar hasta 5 productos en promoción para no sobrecargar el mensaje
                    var promocionesToShow = productosEnPromocion.Take(5);
                    foreach (var producto in promocionesToShow)
                    {
                        // Mostrar nombre, precio y categoría si está disponible
                        var categoriaInfo = string.IsNullOrEmpty(producto.CategoriaNombre) ? "" : $" ({producto.CategoriaNombre})";
                        mensajeBienvenida += $"• {ObtenerEmojiCategoria(producto.CategoriaId)} {producto.Nombre}{categoriaInfo} = ${producto.Precio:N0}\n";
                    }
                    
                    // Si hay más productos en promoción, indicarlo
                    if (productosEnPromocion.Count > 5)
                    {
                        mensajeBienvenida += "... y más promociones en nuestro menú!\n";
                    }
                    
                    mensajeBienvenida += "\n";
                }

                mensajeBienvenida += "📍 Para comenzar tu pedido, por favor compártenos tu *dirección de entrega*:";

                await Responder(mensajeBienvenida, message);
                return;
            }

            var pedidoProceso = _pedidosEnProceso[clientId];

            switch (pedidoProceso.PasoActual)
            {                case PasoPedido.Direccion:
                    pedidoProceso.Direccion = texto;
                    pedidoProceso.PasoActual = PasoPedido.SeleccionandoProductos;
                    
                    // Mensaje de transición más amigable
                    var mensajeTransicion = $"✅ *¡Perfecto!* Tu dirección de entrega es: {texto}\n\n" +
                                          "😋 *Ahora viene lo divertido...*\n" +
                                          "¿Qué se te antoja? Tenemos deliciosas opciones esperándote 🍕🍔🥤\n\n" +
                                          "👇 *Explora nuestro menú por categorías:*";
                    
                    await Responder(mensajeTransicion, message);
                    await MostrarCategoriasProductos(message);
                    break;

                case PasoPedido.Productos:
                    // Cambiar al nuevo estado de selección de productos del menú
                    pedidoProceso.PasoActual = PasoPedido.SeleccionandoProductos;
                    await MostrarCategoriasProductos(message);
                    break;

                case PasoPedido.Observaciones:
                    pedidoProceso.Observaciones = texto.Equals("no", StringComparison.OrdinalIgnoreCase) ? "" : texto;
                    pedidoProceso.PasoActual = PasoPedido.MetodoPago;
                    
                    // Crear botones inline para métodos de pago
                    var tecladoPago = new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup(new[]
                    {
                        new[] 
                        { 
                            Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("💵 Efectivo", "pago_efectivo"),
                            Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("💳 Transferencia", "pago_transferencia") 
                        }
                    });
                    
                    await Responder("💳 ¿Cómo deseas pagar?", message, tecladoPago);
                    break;

                case PasoPedido.MetodoPago:
                    // Si llega aquí desde un callback, el texto ya contiene el método seleccionado
                    // Si es texto directo del usuario, también lo procesamos
                    pedidoProceso.MetodoPago = texto;
                      // Si seleccionó transferencia, mostrar botones de cuentas de pago activas
                    if (texto.Equals("Transferencia", StringComparison.OrdinalIgnoreCase))
                    {
                        pedidoProceso.PasoActual = PasoPedido.SeleccionCuentaPago;
                        
                        var cuentasPago = new CuentaPagoRepository().ObtenerActivas();
                        if (cuentasPago.Count == 0)
                        {
                            await Responder("❌ Lo sentimos, no hay cuentas de pago disponibles en este momento. Por favor selecciona efectivo como método de pago.", message);
                            pedidoProceso.PasoActual = PasoPedido.MetodoPago;
                            return;
                        }
                        
                        var mensajeCuentas = "💳 *Selecciona tu método de transferencia preferido:*\n\n" +
                                           "👇 *Elige una de las siguientes opciones:*";
                        
                        // Crear botones dinámicamente para cada cuenta activa
                        var botonesCuentas = new List<Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton[]>();
                        
                        foreach (var cuenta in cuentasPago)
                        {
                            // Emoji específico por tipo de cuenta
                            string emoji = cuenta.Banco.ToLower().Contains("nequi") ? "💜" : 
                                          cuenta.Banco.ToLower().Contains("daviplata") ? "🔴" :
                                          cuenta.Banco.ToLower().Contains("bancolombia") ? "🟡" : "🏦";
                            
                            var boton = new[] 
                            { 
                                Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData(
                                    $"{emoji} {cuenta.Banco} - {cuenta.TipoCuenta}", 
                                    $"cuenta_pago_{cuenta.Id}")
                            };
                            botonesCuentas.Add(boton);
                        }
                        
                        // Agregar botón para volver atrás
                        botonesCuentas.Add(new[] 
                        { 
                            Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("⬅️ Cambiar método de pago", "volver_metodo_pago")
                        });
                        
                        var tecladoCuentas = new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup(botonesCuentas);
                        
                        await Responder(mensajeCuentas, message, tecladoCuentas);
                    }
                    else
                    {                        // Si seleccionó efectivo, ir directamente a confirmación
                        pedidoProceso.PasoActual = PasoPedido.Confirmado;
                        
                        // Generar resumen detallado con productos y total
                        string productosDetalle = GenerarResumenProductos(pedidoProceso.ProductosSeleccionados);
                        
                        string resumen = $"🧾 *Resumen completo de tu pedido:*\n\n" +
                                         $"📍 *Dirección de entrega:*\n{pedidoProceso.Direccion}\n\n" +
                                         $"{productosDetalle}\n\n" +
                                         $"📝 *Observaciones:* {(string.IsNullOrWhiteSpace(pedidoProceso.Observaciones) ? "Ninguna" : pedidoProceso.Observaciones)}\n" +
                                         $"💳 *Método de pago:* {pedidoProceso.MetodoPago}\n\n" +
                                         $"✅ *¿Confirmas el pedido?*";

                        // Botones de confirmación
                        var teclado = new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup(new[]
                        {
                            new[] 
                            { 
                                Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("✅ Sí, confirmar", "pedido_confirmar"),
                                Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("❌ No, cancelar", "pedido_cancelar") 
                            }
                        });

                        await Responder(resumen, message, teclado);
                    }                    break;

                case PasoPedido.SeleccionCuentaPago:
                    // Este caso se maneja en EjecutarCallbackProducto cuando se selecciona una cuenta específica
                    await Responder("💳 Por favor selecciona una cuenta de pago usando los botones de arriba.", message);
                    break;                case PasoPedido.CuentasPago:
                    // Verificar si el usuario escribió "continuar" o hizo clic en el botón
                    string textoNormalizado = (texto ?? "")
                        .Trim()
                        .ToLowerInvariant()
                        .Normalize(NormalizationForm.FormD);

                    textoNormalizado = new string(textoNormalizado
                        .Where(c => System.Globalization.CharUnicodeInfo.GetUnicodeCategory(c) != System.Globalization.UnicodeCategory.NonSpacingMark)
                        .ToArray());

                    if (textoNormalizado == "continuar" || string.IsNullOrEmpty(texto))
                    {                        // El usuario escribió "continuar" o hizo clic en el botón "Continuar"
                        pedidoProceso.PasoActual = PasoPedido.Confirmado;
                        
                        // Generar resumen detallado con productos y total
                        string productosDetalle = GenerarResumenProductos(pedidoProceso.ProductosSeleccionados);
                        
                        // Generar información de pago basada en si hay cuenta específica seleccionada
                        string infoPago = "💳 *Método de pago:* " + pedidoProceso.MetodoPago;
                        
                        if (pedidoProceso.CuentaPagoSeleccionadaId.HasValue)
                        {
                            try
                            {
                                var cuentaPagoRepo = new CuentaPagoRepository();
                                var cuentaSeleccionada = cuentaPagoRepo.ObtenerPorId(pedidoProceso.CuentaPagoSeleccionadaId.Value);
                                
                                if (cuentaSeleccionada != null)
                                {
                                    var emoji = cuentaSeleccionada.Banco.ToLower().Contains("nequi") ? "💜" : 
                                               cuentaSeleccionada.Banco.ToLower().Contains("daviplata") ? "🔴" :
                                               cuentaSeleccionada.Banco.ToLower().Contains("bancolombia") ? "🟡" : "🏦";
                                    
                                    infoPago = $"💳 *Método de pago:* {pedidoProceso.MetodoPago}\n" +
                                              $"{emoji} *Cuenta:* {cuentaSeleccionada.Banco} - {cuentaSeleccionada.TipoCuenta}\n" +
                                              $"🔢 *Número:* `{cuentaSeleccionada.Numero}`";
                                }
                            }
                            catch (Exception)
                            {
                                // En caso de error, usar la información básica
                                infoPago = "💳 *Método de pago:* " + pedidoProceso.MetodoPago;
                            }
                        }
                        
                        string resumenTransferencia = $"🧾 *Resumen completo de tu pedido:*\n\n" +
                                         $"📍 *Dirección de entrega:*\n{pedidoProceso.Direccion}\n\n" +
                                         $"{productosDetalle}\n\n" +
                                         $"📝 *Observaciones:* {(string.IsNullOrWhiteSpace(pedidoProceso.Observaciones) ? "Ninguna" : pedidoProceso.Observaciones)}\n" +
                                         $"{infoPago}\n\n" +
                                         $"✅ *¿Confirmas el pedido?*";

                        // Botones de confirmación
                        var tecladoTransferencia = new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup(new[]
                        {
                            new[] 
                            { 
                                Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("✅ Sí, confirmar", "pedido_confirmar"),
                                Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("❌ No, cancelar", "pedido_cancelar") 
                            }
                        });

                        await Responder(resumenTransferencia, message, tecladoTransferencia);
                    }
                    else
                    {
                        // El usuario escribió algo diferente, recordarle que debe escribir "continuar" o usar el botón
                        await Responder("💬 Para continuar con tu pedido, puedes escribir *'continuar'* o usar el botón 'Continuar' que aparece arriba.", message);
                    }
                    break;

                case PasoPedido.Confirmado:
                    try
                    {
                        string confirmado = (texto ?? "")
                            .Trim()
                            .ToLowerInvariant()
                            .Normalize(NormalizationForm.FormD);                        confirmado = new string(confirmado
                            .Where(c => System.Globalization.CharUnicodeInfo.GetUnicodeCategory(c) != System.Globalization.UnicodeCategory.NonSpacingMark)
                            .ToArray());
                        
                        // También acepta la confirmación desde botones
                        if (confirmado.StartsWith("si") || confirmado == "pedido_confirmar")
                        {
                            // Verificar y crear cliente automáticamente si no existe
                            if (!clienteRepo.ExisteCliente(pedidoProceso.ClienteId))
                            {
                                var nuevoCliente = new Client
                                {
                                    Id = pedidoProceso.ClienteId,
                                    Name = message.From.FirstName ?? "Usuario",
                                    Username = message.From.Username,
                                    Phone = null // Se puede agregar más tarde
                                };
                                clienteRepo.AgregarCliente(nuevoCliente);
                            }

                            var pedido = new Pedido
                            {
                                Id = GenerarPedidoId(),
                                ClienteId = pedidoProceso.ClienteId,
                                TelefonoEntrega = "",
                                DireccionEntrega = pedidoProceso.Direccion,
                                Productos = pedidoProceso.Productos,
                                Observaciones = pedidoProceso.Observaciones,
                                MetodoPago = pedidoProceso.MetodoPago,
                                Estado = pedidoProceso.MetodoPago.Equals("Transferencia", StringComparison.OrdinalIgnoreCase) ? "PendientePago" : "Pendiente",
                                FechaHora = DateTime.Now,
                                PagoPendiente = pedidoProceso.MetodoPago.Equals("Transferencia", StringComparison.OrdinalIgnoreCase),
                                PagoConfirmado = false,
                                FechaConfirmacionPago = null,
                                MetodoConfirmacionPago = null
                            };

                            var detalles = new List<DetallePedido>();
                            
                            // Usar ProductosSeleccionados si están disponibles, si no, usar el método anterior
                            if (pedidoProceso.ProductosSeleccionados.Count > 0)
                            {
                                foreach (var productoSeleccionado in pedidoProceso.ProductosSeleccionados)
                                {
                                    detalles.Add(new DetallePedido
                                    {
                                        PedidoId = pedido.Id,
                                        Producto = productoSeleccionado.Nombre,
                                        Cantidad = productoSeleccionado.Cantidad
                                    });
                                }
                            }
                            else
                            {
                                // Mantener compatibilidad con el método anterior (texto libre)
                                foreach (var item in pedido.Productos.Split(','))
                                {
                                    var partes = item.Trim().Split('x');
                                    if (partes.Length == 2 && int.TryParse(partes[0], out int cantidad))
                                    {
                                        detalles.Add(new DetallePedido
                                        {
                                            PedidoId = pedido.Id,
                                            Producto = partes[1].Trim(),
                                            Cantidad = cantidad
                                        });
                                    }
                                }
                            }

                            new PedidoRepository().AgregarPedido(pedido, detalles);
                            // Disparar evento de nuevo pedido
                            try
                            {
                                var telegramService = TelegramFoodBot.Business.Services.EnhancedTelegramService.Instance;
                                telegramService.OnPedidoCreado(pedido);
                            }
                            catch { /* Si no hay subscriptores, ignorar */ }
                              // Si es pago por transferencia, notificar a los administradores
                            if (pedido.MetodoPago.Equals("Transferencia", StringComparison.OrdinalIgnoreCase))
                            {
                                try
                                {
                                    var telegramService = TelegramFoodBot.Business.Services.EnhancedTelegramService.Instance;
                                    await telegramService.NotificarPagoPendiente(
                                        pedido.Id, 
                                        pedido.ClienteId, 
                                        pedido.Productos, 
                                        pedido.DireccionEntrega, 
                                        0m // Por ahora sin total, se puede calcular más adelante
                                    );
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine($"Error al notificar pago pendiente: {ex.Message}");
                                    // No interrumpimos el flujo del pedido aunque falle la notificación
                                }
                            }
                            
                            // Crear teclado con opciones tras completar el pedido
                            var tecladoFinal = new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup(new[]
                            {
                                new[] 
                                { 
                                    Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("📅 Reservar Mesa", "reservar"),
                                    Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("🏠 Volver al inicio", "start") 
                                }
                            });
                            
                            string mensajeExito;
                            if (pedido.MetodoPago.Equals("Transferencia", StringComparison.OrdinalIgnoreCase))
                            {
                                mensajeExito = $"🎉 ¡Tu pedido se ha registrado correctamente!\n" +
                                             $"📋 *Número de pedido:* {pedido.Id}\n\n" +
                                             $"💳 *Pago por transferencia:*\n" +
                                             $"⏳ Tu pedido está pendiente de confirmación de pago.\n" +
                                             $"📱 Te notificaremos tan pronto como confirmemos tu transferencia.\n" +
                                             $"🕒 El proceso de confirmación puede tomar unos minutos.\n\n" +
                                             $"¡Gracias por confiar en nosotros!";
                            }
                            else
                            {
                                mensajeExito = "🎉 ¡Tu pedido se ha registrado correctamente! Gracias por confiar en nosotros.\n\n📌 Te mantendremos informado(a) aquí mismo sobre cada etapa de tu pedido: desde la preparación hasta que esté en camino. ¡Estamos atentos para ofrecerte el mejor servicio!";
                            }
                            
                            await Responder(mensajeExito, message, tecladoFinal);
                        }
                        else
                        {
                            // Crear teclado para pedido cancelado
                            var tecladoCancelado = new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup(new[]
                            {
                                new[] { Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("🔄 Hacer nuevo pedido", "pedir") },
                                new[] { Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("🏠 Volver al inicio", "start") }
                            });
                            
                            await Responder("❌ Pedido cancelado. ¿Deseas hacer algo más?", message, tecladoCancelado);
                        }

                        _pedidosEnProceso.Remove(clientId);
                    }
                    catch (Exception ex)
                    {
                        await Responder($"⚠️ Error interno: {ex.Message}", message);
                    }
                    break;
            }
        }

        public async Task EjecutarCallbackProducto(TelegramMessage message)
        {
            long clientId = message.From.Id;
            string callbackData = message.Text?.Trim() ?? "";

            if (!_pedidosEnProceso.ContainsKey(clientId))
            {
                await Responder("❌ No tienes un pedido en proceso. Usa /pedir para comenzar uno nuevo.", message);
                return;
            }

            var pedidoProceso = _pedidosEnProceso[clientId];            if (callbackData.StartsWith("categoria_"))
            {
                int categoriaId = int.Parse(callbackData.Replace("categoria_", ""));
                await MostrarProductosCategoria(message, categoriaId);
            }
            else if (callbackData.StartsWith("producto_"))
            {
                int productoId = int.Parse(callbackData.Replace("producto_", ""));
                await MostrarOpcionesProducto(message, productoId);
            }
            else if (callbackData.StartsWith("promocion_producto_"))
            {
                // Si el usuario selecciona un producto en promoción, iniciar pedido automáticamente
                int productoId = int.Parse(callbackData.Replace("promocion_producto_", ""));
                
                // Inicializar pedido si no existe
                if (!_pedidosEnProceso.ContainsKey(clientId))
                {
                    _pedidosEnProceso[clientId] = new PedidoEnProceso
                    {
                        ClienteId = clientId,
                        PasoActual = PasoPedido.SeleccionandoProductos,
                        ProductosSeleccionados = new List<ProductoSeleccionado>()
                    };
                }
                
                await MostrarOpcionesProducto(message, productoId);
            }
            else if (callbackData.StartsWith("cantidad_"))
            {
                // formato: cantidad_{productoId}_{cantidad}
                var partes = callbackData.Split('_');
                if (partes.Length == 3)
                {
                    int productoId = int.Parse(partes[1]);
                    int cantidad = int.Parse(partes[2]);
                    await AgregarProductoAlPedido(message, productoId, cantidad);
                }
            }
            else if (callbackData == "volver_menu")
            {
                await MostrarCategoriasProductos(message);
            }            else if (callbackData == "finalizar_productos")
            {
                await FinalizarSeleccionProductos(message);
            }
            else if (callbackData == "observaciones_si")
            {
                // El usuario quiere agregar observaciones, cambiar al paso de observaciones para entrada de texto
                pedidoProceso.PasoActual = PasoPedido.Observaciones;
                await Responder("📝 *Perfecto!* Escribe tus observaciones especiales.\n\nPor ejemplo: 'sin cebolla', 'extra queso', 'bien cocido', etc.", message);
            }            else if (callbackData == "observaciones_no")
            {
                // El usuario no quiere observaciones, continuar directamente al método de pago
                pedidoProceso.Observaciones = "";
                pedidoProceso.PasoActual = PasoPedido.MetodoPago;
                
                // Crear botones inline para métodos de pago
                var tecladoPago = new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup(new[]
                {
                    new[] 
                    { 
                        Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("💵 Efectivo", "pago_efectivo"),
                        Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("💳 Transferencia", "pago_transferencia")
                    }
                });
                
                await Responder("💳 *¿Cómo deseas pagar?*\n\nSelecciona tu método de pago preferido:", message, tecladoPago);
            }
            else if (callbackData.StartsWith("cuenta_pago_"))
            {
                // El usuario seleccionó una cuenta de pago específica
                var cuentaIdStr = callbackData.Replace("cuenta_pago_", "");
                if (int.TryParse(cuentaIdStr, out int cuentaId))
                {
                    await ProcesarSeleccionCuentaPago(message, cuentaId);
                }
                else
                {
                    await Responder("❌ Error en la selección de cuenta. Por favor intenta de nuevo.", message);
                }
            }
            else if (callbackData == "volver_metodo_pago")
            {
                // El usuario quiere cambiar el método de pago
                pedidoProceso.PasoActual = PasoPedido.MetodoPago;
                pedidoProceso.CuentaPagoSeleccionadaId = null; // Limpiar selección anterior
                
                // Mostrar de nuevo las opciones de método de pago
                var tecladoPago = new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup(new[]
                {
                    new[] 
                    { 
                        Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("💵 Efectivo", "pago_efectivo"),
                        Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("💳 Transferencia", "pago_transferencia")
                    }
                });
                  await Responder("💳 *¿Cómo deseas pagar?*\n\nSelecciona tu método de pago preferido:", message, tecladoPago);
            }
            else if (callbackData == "volver_cuentas_pago")
            {
                // El usuario quiere volver a seleccionar cuenta de pago
                pedidoProceso.PasoActual = PasoPedido.SeleccionCuentaPago;
                pedidoProceso.CuentaPagoSeleccionadaId = null; // Limpiar selección anterior
                
                // Mostrar de nuevo las cuentas de pago disponibles
                var cuentaPagoRepo = new CuentaPagoRepository();
                var cuentasPago = cuentaPagoRepo.ObtenerActivas();

                if (cuentasPago.Count == 0)
                {
                    await Responder("❌ No hay cuentas de pago disponibles. Por favor contacta al administrador.", message);
                    return;
                }

                var botonesCuentas = new List<List<Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton>>();
                
                foreach (var cuenta in cuentasPago)
                {
                    string emoji = cuenta.Banco.ToLower().Contains("nequi") ? "💜" : 
                                   cuenta.Banco.ToLower().Contains("daviplata") ? "🔴" :
                                   cuenta.Banco.ToLower().Contains("bancolombia") ? "🟡" : "🏦";
                    
                    var boton = new List<Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton>
                    { 
                        Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData(
                            $"{emoji} {cuenta.Banco} - {cuenta.TipoCuenta}", 
                            $"cuenta_pago_{cuenta.Id}")
                    };
                    botonesCuentas.Add(boton);
                }
                
                // Agregar botón para volver al método de pago
                botonesCuentas.Add(new List<Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton>
                {
                    Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("⬅️ Cambiar método de pago", "volver_metodo_pago")
                });

                var tecladoCuentas = new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup(botonesCuentas);
                await Responder("💳 *Selecciona tu cuenta de pago:*\n\nElige la cuenta donde realizarás la transferencia:", message, tecladoCuentas);
            }
        }

        private async Task MostrarCategoriasProductos(TelegramMessage message)
        {
            try
            {
                var categorias = _categoriaService.ObtenerCategoriasActivas();
                if (categorias.Count == 0)
                {
                    await Responder("❌ No hay categorías disponibles en este momento.", message);
                    return;
                }
                var botones = new List<List<Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton>>();
                foreach (var categoria in categorias)
                {
                    botones.Add(new List<Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton>
                    {
                        Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData(
                            $"🍽️ {categoria.Nombre}",
                            $"categoria_{categoria.Id}")
                    });
                }
                var teclado = new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup(botones);
                await Responder("🍽️ *Selecciona una categoría:*\n\nElige la categoría de productos que deseas ver:", message, teclado);
            }
            catch (Exception ex)
            {
                await Responder($"❌ Error al obtener categorías: {ex.Message}", message);
            }
        }

        private async Task MostrarProductosCategoria(TelegramMessage message, int categoriaId)
        {
            try
            {
                var productos = _productoService.ObtenerProductosActivosPorCategoria(categoriaId);
                if (productos.Count == 0)
                {
                    await Responder("❌ No hay productos disponibles en esta categoría.", message);
                    return;
                }
                var categoria = _categoriaService.ObtenerCategoriaPorId(categoriaId);
                var botones = new List<List<Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton>>();
                foreach (var producto in productos)
                {
                    var textoBoton = $"{producto.Nombre} - ${producto.Precio:N0}";
                    botones.Add(new List<Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton>
                    {
                        Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData(
                            textoBoton,
                            $"producto_{producto.Id}")
                    });
                }
                // Botones de navegación
                botones.Add(new List<Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton>
                {
                    Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("⬅️ Volver al menú", "volver_menu"),
                    Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("✅ Finalizar", "finalizar_productos")
                });
                var teclado = new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup(botones);
                await Responder($"🍽️ *{categoria?.Nombre}*\n\nSelecciona un producto:", message, teclado);
            }
            catch (Exception ex)
            {
                await Responder($"❌ Error al obtener productos: {ex.Message}", message);
            }
        }

        private async Task MostrarOpcionesProducto(TelegramMessage message, int productoId)
        {
            try
            {
                var producto = _productoService.ObtenerProductoPorId(productoId);
                if (producto == null)
                {
                    await Responder("❌ Producto no encontrado.", message);
                    return;
                }
                var botones = new List<List<Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton>>();
                // Botones de cantidad (1-5)
                var filaCantidad1 = new List<Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton>();
                var filaCantidad2 = new List<Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton>();
                for (int i = 1; i <= 3; i++)
                {
                    filaCantidad1.Add(Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData(
                        $"{i}", $"cantidad_{productoId}_{i}"));
                }
                for (int i = 4; i <= 5; i++)
                {
                    filaCantidad2.Add(Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData(
                        $"{i}", $"cantidad_{productoId}_{i}"));
                }
                botones.Add(filaCantidad1);
                botones.Add(filaCantidad2);
                // Botón para volver
                botones.Add(new List<Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton>
                {
                    Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("⬅️ Volver", "volver_menu")
                });
                var teclado = new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup(botones);
                var mensaje = $"🍽️ *{producto.Nombre}*\n";
                mensaje += $"💰 Precio: ${producto.Precio:N0}\n";
                if (!string.IsNullOrWhiteSpace(producto.Descripcion))
                {
                    mensaje += $"📝 {producto.Descripcion}\n";
                }
                mensaje += "\n¿Cuántos deseas agregar?";
                await Responder(mensaje, message, teclado);
            }
            catch (Exception ex)
            {
                await Responder($"❌ Error al obtener producto: {ex.Message}", message);
            }
        }

        private async Task AgregarProductoAlPedido(TelegramMessage message, int productoId, int cantidad)
        {
            try
            {
                long clientId = message.From.Id;
                var pedidoProceso = _pedidosEnProceso[clientId];
                
                var producto = _productoService.ObtenerProductoPorId(productoId);
                
                if (producto == null)
                {
                    await Responder("❌ Producto no encontrado.", message);
                    return;
                }

                // Verificar si el producto ya está en la lista
                var productoExistente = pedidoProceso.ProductosSeleccionados
                    .FirstOrDefault(p => p.ProductoId == productoId);

                if (productoExistente != null)
                {
                    productoExistente.Cantidad += cantidad;
                }
                else
                {
                    pedidoProceso.ProductosSeleccionados.Add(new ProductoSeleccionado
                    {
                        ProductoId = producto.Id,
                        Nombre = producto.Nombre,
                        Precio = producto.Precio,
                        Cantidad = cantidad
                    });
                }

                // Mostrar confirmación y resumen actual
                var resumen = GenerarResumenProductos(pedidoProceso.ProductosSeleccionados);
                
                var botones = new List<List<Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton>>
                {
                    new List<Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton>
                    {
                        Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("➕ Agregar más", "volver_menu"),
                        Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("✅ Finalizar", "finalizar_productos")
                    }
                };

                var teclado = new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup(botones);
                
                await Responder($"✅ *Agregado: {cantidad}x {producto.Nombre}*\n\n{resumen}", message, teclado);
            }
            catch (Exception ex)
            {
                await Responder($"❌ Error al agregar producto: {ex.Message}", message);
            }
        }        private async Task FinalizarSeleccionProductos(TelegramMessage message)
        {
            long clientId = message.From.Id;
            var pedidoProceso = _pedidosEnProceso[clientId];

            if (pedidoProceso.ProductosSeleccionados.Count == 0)
            {
                await Responder("❌ No has seleccionado ningún producto. Por favor selecciona al menos uno.", message);
                await MostrarCategoriasProductos(message);
                return;
            }

            // Generar string de productos para mantener compatibilidad con el sistema existente
            pedidoProceso.Productos = GenerarStringProductos(pedidoProceso.ProductosSeleccionados);
            
            var resumen = GenerarResumenProductos(pedidoProceso.ProductosSeleccionados);
            
            // Crear botones inline para observaciones
            var botonesObservaciones = new List<List<Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton>>
            {
                new List<Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton>
                {
                    Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("✅ Sí", "observaciones_si"),
                    Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("❌ No", "observaciones_no")
                }
            };
            
            var tecladoObservaciones = new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup(botonesObservaciones);
            
            await Responder($"✅ *Productos seleccionados:*\n\n{resumen}\n\n📝 ¿Tienes alguna observación especial? Por ejemplo, 'sin cebolla', 'extra queso', etc.", message, tecladoObservaciones);
        }

        private string GenerarResumenProductos(List<ProductoSeleccionado> productos)
        {
            var resumen = "🛍️ *Tu pedido actual:*\n";
            decimal total = 0;
            
            foreach (var producto in productos)
            {
                var subtotal = producto.Precio * producto.Cantidad;
                resumen += $"• {producto.Cantidad}x {producto.Nombre} - ${subtotal:N0}\n";
                total += subtotal;
            }
            
            resumen += $"\n💰 *Total: ${total:N0}*";
            
            return resumen;
        }

        private string GenerarStringProductos(List<ProductoSeleccionado> productos)
        {
            return string.Join(", ", productos.Select(p => $"{p.Cantidad}x{p.Nombre}"));
        }

        private string GenerarPedidoId()
        {
            var hoy = DateTime.Now.Date;
            if (hoy != _fechaUltimoPedido)
            {
                _fechaUltimoPedido = hoy;
                _contadorPedidosDelDia = 0;
            }

            string nuevoId;
            do
            {
                _contadorPedidosDelDia++;
                nuevoId = $"{hoy:ddMMyy}{_contadorPedidosDelDia:D3}";
            }
            while (new PedidoRepository().ExistePedidoConId(nuevoId));            return nuevoId;
        }

        private async Task ProcesarSeleccionCuentaPago(TelegramMessage message, int cuentaId)
        {
            long clientId = message.From.Id;
            var pedidoProceso = _pedidosEnProceso[clientId];

            try
            {
                var cuentaPagoRepo = new CuentaPagoRepository();
                var cuentaSeleccionada = cuentaPagoRepo.ObtenerPorId(cuentaId);

                if (cuentaSeleccionada == null)
                {
                    await Responder("❌ La cuenta de pago seleccionada no está disponible. Por favor selecciona otra.", message);
                    return;
                }

                // Verificar que la cuenta esté activa
                if (cuentaSeleccionada.Estado != "Activa")
                {
                    await Responder("❌ La cuenta de pago seleccionada no está activa. Por favor selecciona otra.", message);
                    return;
                }

                // Guardar la cuenta seleccionada
                pedidoProceso.CuentaPagoSeleccionadaId = cuentaId;
                pedidoProceso.MetodoPago = "Transferencia";

                // Construir el mensaje con la información de la cuenta
                var emoji = cuentaSeleccionada.Banco.ToLower().Contains("nequi") ? "💜" : 
                           cuentaSeleccionada.Banco.ToLower().Contains("daviplata") ? "🔴" :
                           cuentaSeleccionada.Banco.ToLower().Contains("bancolombia") ? "🟡" : "🏦";

                var mensaje = new StringBuilder();
                mensaje.AppendLine($"{emoji} *{cuentaSeleccionada.Banco}*");
                mensaje.AppendLine($"📱 *Tipo:* {cuentaSeleccionada.TipoCuenta}");
                mensaje.AppendLine($"🔢 *Número:* `{cuentaSeleccionada.Numero}`");
                mensaje.AppendLine($"👤 *Titular:* {cuentaSeleccionada.Titular}");
                
                if (!string.IsNullOrEmpty(cuentaSeleccionada.Instrucciones))
                {
                    mensaje.AppendLine($"\n📋 *Instrucciones:*");
                    mensaje.AppendLine(cuentaSeleccionada.Instrucciones);
                }

                mensaje.AppendLine($"\n💰 *Total a transferir:* ${CalcularTotalPedido(pedidoProceso):N0}");
                mensaje.AppendLine("\n⚠️ *Importante:* Realiza la transferencia y envía el comprobante de pago.");

                // Crear botones para continuar o cambiar cuenta
                var botones = new List<List<Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton>>
                {
                    new List<Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton>
                    {
                        Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("✅ Continuar con pedido", "continuar_pedido")
                    },
                    new List<Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton>
                    {
                        Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("🔄 Cambiar cuenta", "volver_cuentas_pago"),
                        Telegram.Bot.Types.ReplyMarkups.InlineKeyboardButton.WithCallbackData("⬅️ Cambiar método", "volver_metodo_pago")
                    }
                };

                var teclado = new Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup(botones);
                await Responder(mensaje.ToString(), message, teclado);

                // Cambiar el paso actual para que el flujo continúe correctamente
                pedidoProceso.PasoActual = PasoPedido.CuentasPago;
            }
            catch (Exception ex)
            {
                await Responder($"❌ Error al procesar la selección de cuenta: {ex.Message}", message);
            }
        }

        private decimal CalcularTotalPedido(PedidoEnProceso pedido)
        {
            decimal total = 0;
            if (pedido.ProductosSeleccionados != null)
            {
                foreach (var producto in pedido.ProductosSeleccionados)
                {
                    total += producto.Precio * producto.Cantidad;
                }
            }
            return total;
        }

        private async Task Responder(string texto, TelegramMessage mensaje, Telegram.Bot.Types.ReplyMarkups.IReplyMarkup replyMarkup = null)
        {
            long clientId = mensaje.From.Id;
            
            // Verificar si existe un pedido en proceso y obtener el paso actual
            PasoPedido? pasoActual = null;
            if (_pedidosEnProceso.ContainsKey(clientId))
            {
                pasoActual = _pedidosEnProceso[clientId].PasoActual;
            }
            
            // Si el paso actual es MetodoPago o posterior, enviar siempre mensaje nuevo
            bool debeEnviarNuevo = pasoActual.HasValue && 
                                   (pasoActual.Value >= PasoPedido.MetodoPago);
            
            // Si el mensaje tiene MessageId y viene de un callback, y NO estamos en paso de método de pago o posterior, intentar editar
            if (mensaje.MessageId != 0 && mensaje.Chat != null && !debeEnviarNuevo)
            {
                try
                {
                    await _bot.EditMessageTextAsync(
                        chatId: mensaje.Chat.Id,
                        messageId: mensaje.MessageId,
                        text: texto,
                        parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                        replyMarkup: replyMarkup as Telegram.Bot.Types.ReplyMarkups.InlineKeyboardMarkup
                    );
                }
                catch
                {
                    // Si falla la edición (por ejemplo, mensaje antiguo), enviar uno nuevo
                    await _bot.SendTextMessageAsync(
                        chatId: mensaje.Chat.Id,
                        text: texto,
                        parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                        replyMarkup: replyMarkup
                    );
                }
            }
            else
            {
                // Enviar mensaje nuevo (caso normal o cuando estamos en MetodoPago o posterior)
                await _bot.SendTextMessageAsync(
                    chatId: mensaje.Chat.Id,
                    text: texto,
                    parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown,
                    replyMarkup: replyMarkup
                );
            }
            _onMessage?.Invoke(new AppMessage
            {
                Text = texto,
                Timestamp = DateTime.Now,
                IsFromAdmin = true,
                ClientId = mensaje.From.Id
            });
        }

        /// <summary>
        /// Devuelve un emoji apropiado según la categoría del producto
        /// </summary>
        private string ObtenerEmojiCategoria(int categoriaId)
        {
            // Se pueden personalizar los emojis según las categorías específicas del restaurante
            switch (categoriaId)
            {
                case 1: return "🍕"; // Pizzas
                case 2: return "🍔"; // Hamburguesas
                case 3: return "🥤"; // Bebidas
                case 4: return "🍦"; // Postres
                case 5: return "🥗"; // Ensaladas
                case 6: return "🍗"; // Pollo
                case 7: return "🍝"; // Pastas
                case 8: return "🍱"; // Combos
                case 9: return "🌮"; // Mexicana
                case 10: return "🍣"; // Sushi
                default: return "🍽️"; // Por defecto para otras categorías
            }
        }
    }    public enum PasoPedido
    {
        Direccion,
        Productos,
        SeleccionandoProductos, // Nuevo estado para selección de productos del menú
        Observaciones,
        MetodoPago,
        SeleccionCuentaPago, // Nuevo paso para seleccionar cuenta específica
        CuentasPago,
        Confirmado
    }

    public class ProductoSeleccionado
    {
        public int ProductoId { get; set; }
        public string Nombre { get; set; }
        public decimal Precio { get; set; }
        public int Cantidad { get; set; }
    }    public class PedidoEnProceso
    {
        public long ClienteId { get; set; }
        public PasoPedido PasoActual { get; set; }
        public string Direccion { get; set; }
        public string Productos { get; set; }
        public List<ProductoSeleccionado> ProductosSeleccionados { get; set; } = new List<ProductoSeleccionado>();
        public string Observaciones { get; set; }
        public string MetodoPago { get; set; }
        public int? CuentaPagoSeleccionadaId { get; set; } // Nueva propiedad para almacenar la cuenta seleccionada
    }
}
