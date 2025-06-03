using System;
using System.Diagnostics;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using Telegram.Bot;
using TelegramFoodBot.Business.Configuration;

namespace TelegramFoodBot.Business.Diagnostics
{
    /// <summary>
    /// Herramienta de diagnóstico para resolver problemas de conectividad específicos de PC
    /// Ejecuta pruebas exhaustivas para identificar la causa de los botones en loading
    /// </summary>
    public static class NetworkDiagnostics
    {
        private const string TELEGRAM_API_URL = "https://api.telegram.org";

        public static async Task RunFullDiagnostics()
        {
            Console.WriteLine("╔══════════════════════════════════════════════════════════════╗");
            Console.WriteLine("║               DIAGNÓSTICO DE CONECTIVIDAD TELEGRAM          ║");
            Console.WriteLine("║              Resolución de botones en loading               ║");
            Console.WriteLine("╚══════════════════════════════════════════════════════════════╝");
            Console.WriteLine();

            await RunSystemInfo();
            await RunNetworkTests();
            await RunTelegramApiTests();
            await RunProxyDetection();
            await RunFirewallTests();
            await RunRecommendations();
        }

        private static async Task RunSystemInfo()
        {
            Console.WriteLine("📋 INFORMACIÓN DEL SISTEMA");
            Console.WriteLine("═".PadRight(50, '═'));
            
            Console.WriteLine($"🖥️  Sistema Operativo: {Environment.OSVersion}");
            Console.WriteLine($"💻 Máquina: {Environment.MachineName}");
            Console.WriteLine($"👤 Usuario: {Environment.UserName}");
            Console.WriteLine($"⚙️  .NET Version: {Environment.Version}");
            Console.WriteLine($"📁 Directorio actual: {Environment.CurrentDirectory}");
            Console.WriteLine($"🕒 Fecha/Hora: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            Console.WriteLine();
        }

        private static async Task RunNetworkTests()
        {
            Console.WriteLine("🌐 PRUEBAS DE CONECTIVIDAD DE RED");
            Console.WriteLine("═".PadRight(50, '═'));

            await TestPing("8.8.8.8", "Google DNS");
            await TestPing("149.154.167.50", "Telegram Server");
            await TestDnsResolution();
            
            Console.WriteLine();
        }

        private static async Task TestPing(string host, string description)
        {
            try
            {
                using var ping = new Ping();
                var reply = await ping.SendPingAsync(host, 5000);
                
                if (reply.Status == IPStatus.Success)
                {
                    Console.WriteLine($"✅ {description} ({host}): {reply.RoundtripTime}ms");
                }
                else
                {
                    Console.WriteLine($"❌ {description} ({host}): {reply.Status}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ {description} ({host}): Error - {ex.Message}");
            }
        }

        private static async Task TestDnsResolution()
        {
            try
            {
                var stopwatch = Stopwatch.StartNew();
                var hostEntry = await System.Net.Dns.GetHostEntryAsync("api.telegram.org");
                stopwatch.Stop();
                
                Console.WriteLine($"✅ DNS Resolution (api.telegram.org): {stopwatch.ElapsedMilliseconds}ms");
                foreach (var ip in hostEntry.AddressList)
                {
                    Console.WriteLine($"   🔗 IP: {ip}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ DNS Resolution: {ex.Message}");
            }
        }

        private static async Task RunTelegramApiTests()
        {
            Console.WriteLine("🤖 PRUEBAS DE API DE TELEGRAM");
            Console.WriteLine("═".PadRight(50, '═'));

            await TestBasicHttpClient();
            await TestConfiguredHttpClient();
            await TestBotApi();
            
            Console.WriteLine();
        }

        private static async Task TestBasicHttpClient()
        {
            try
            {
                using var client = new HttpClient();
                client.Timeout = TimeSpan.FromSeconds(10);
                
                var stopwatch = Stopwatch.StartNew();
                var response = await client.GetAsync($"{TELEGRAM_API_URL}/bot{BotConfiguration.BotToken}/getMe");
                stopwatch.Stop();
                
                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"✅ HttpClient básico: {stopwatch.ElapsedMilliseconds}ms - {response.StatusCode}");
                }
                else
                {
                    Console.WriteLine($"❌ HttpClient básico: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ HttpClient básico: {ex.GetType().Name} - {ex.Message}");
            }
        }

        private static async Task TestConfiguredHttpClient()
        {
            try
            {
                var handler = new HttpClientHandler()
                {
                    UseProxy = false,
                    UseCookies = false,
                    UseDefaultCredentials = false
                };

                using var client = new HttpClient(handler);
                client.Timeout = TimeSpan.FromSeconds(10);
                client.DefaultRequestHeaders.Add("User-Agent", "TelegramBot/1.0 (Windows; .NET)");
                
                var stopwatch = Stopwatch.StartNew();
                var response = await client.GetAsync($"{TELEGRAM_API_URL}/bot{BotConfiguration.BotToken}/getMe");
                stopwatch.Stop();
                
                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"✅ HttpClient configurado: {stopwatch.ElapsedMilliseconds}ms - {response.StatusCode}");
                }
                else
                {
                    Console.WriteLine($"❌ HttpClient configurado: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ HttpClient configurado: {ex.GetType().Name} - {ex.Message}");
            }
        }

        private static async Task TestBotApi()
        {
            try
            {
                var handler = new HttpClientHandler()
                {
                    UseProxy = false,
                    UseCookies = false
                };

                using var httpClient = new HttpClient(handler) { Timeout = TimeSpan.FromSeconds(10) };
                var botClient = new TelegramBotClient(BotConfiguration.BotToken, httpClient);
                
                var stopwatch = Stopwatch.StartNew();
                var me = await botClient.GetMeAsync();
                stopwatch.Stop();
                
                Console.WriteLine($"✅ Bot API: {stopwatch.ElapsedMilliseconds}ms - {me.FirstName} (@{me.Username})");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Bot API: {ex.GetType().Name} - {ex.Message}");
            }
        }

        private static async Task RunProxyDetection()
        {
            Console.WriteLine("🔍 DETECCIÓN DE PROXY Y FIREWALL");
            Console.WriteLine("═".PadRight(50, '═'));

            try
            {
                var proxy = System.Net.WebRequest.GetSystemWebProxy();
                var telegramUri = new Uri("https://api.telegram.org");
                var proxyUri = proxy.GetProxy(telegramUri);
                
                if (proxyUri.ToString() != telegramUri.ToString())
                {
                    Console.WriteLine($"⚠️  Proxy detectado: {proxyUri}");
                    Console.WriteLine("   🔧 Recomendación: Configurar bypass o deshabilitar proxy");
                }
                else
                {
                    Console.WriteLine("✅ No se detectó proxy del sistema");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error detectando proxy: {ex.Message}");
            }

            var httpProxy = Environment.GetEnvironmentVariable("HTTP_PROXY") ?? Environment.GetEnvironmentVariable("http_proxy");
            var httpsProxy = Environment.GetEnvironmentVariable("HTTPS_PROXY") ?? Environment.GetEnvironmentVariable("https_proxy");
            
            if (!string.IsNullOrEmpty(httpProxy))
            {
                Console.WriteLine($"⚠️  HTTP_PROXY variable encontrada: {httpProxy}");
            }
            
            if (!string.IsNullOrEmpty(httpsProxy))
            {
                Console.WriteLine($"⚠️  HTTPS_PROXY variable encontrada: {httpsProxy}");
            }
            
            if (string.IsNullOrEmpty(httpProxy) && string.IsNullOrEmpty(httpsProxy))
            {
                Console.WriteLine("✅ No se encontraron variables de proxy en entorno");
            }

            Console.WriteLine();
        }

        private static async Task RunFirewallTests()
        {
            Console.WriteLine("🛡️  PRUEBAS DE FIREWALL Y ANTIVIRUS");
            Console.WriteLine("═".PadRight(50, '═'));

            var telegramPorts = new[] { 80, 443, 8080 };
            
            foreach (var port in telegramPorts)
            {
                await TestTcpConnection("149.154.167.50", port);
            }
            
            Console.WriteLine();
        }

        private static async Task TestTcpConnection(string host, int port)
        {
            try
            {
                using var client = new System.Net.Sockets.TcpClient();
                var connectTask = client.ConnectAsync(host, port);
                
                if (await Task.WhenAny(connectTask, Task.Delay(5000)) == connectTask)
                {
                    Console.WriteLine($"✅ TCP {host}:{port} - Conectado");
                }
                else
                {
                    Console.WriteLine($"⏱️  TCP {host}:{port} - Timeout");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ TCP {host}:{port} - {ex.Message}");
            }
        }

        private static async Task RunRecommendations()
        {
            Console.WriteLine("💡 RECOMENDACIONES PARA RESOLVER EL PROBLEMA");
            Console.WriteLine("═".PadRight(50, '═'));
            
            Console.WriteLine("1. 🔥 FIREWALL/ANTIVIRUS:");
            Console.WriteLine("   • Agregar excepción para tu aplicación en Windows Defender");
            Console.WriteLine("   • Deshabilitar temporalmente antivirus de terceros");
            Console.WriteLine("   • Verificar reglas de firewall corporativo");
            Console.WriteLine();
            
            Console.WriteLine("2. 🌐 CONEXIÓN DE RED:");
            Console.WriteLine("   • Probar desde una red diferente (datos móviles)");
            Console.WriteLine("   • Usar VPN si hay restricciones regionales");
            Console.WriteLine("   • Verificar configuración DNS (usar 8.8.8.8)");
            Console.WriteLine();
            
            Console.WriteLine("3. ⚙️  CONFIGURACIÓN DE APLICACIÓN:");
            Console.WriteLine("   • Usar EnhancedTelegramService en lugar de TelegramService");
            Console.WriteLine("   • Incrementar timeouts si la red es lenta");
            Console.WriteLine("   • Habilitar logging detallado");
            Console.WriteLine();
            
            Console.WriteLine("4. 🔧 SOLUCIONES ESPECÍFICAS:");
            Console.WriteLine("   • Ejecutar como administrador");
            Console.WriteLine("   • Verificar configuración de proxy empresarial");
            Console.WriteLine("   • Actualizar .NET Framework/Core");
            Console.WriteLine();

            Console.WriteLine("🚀 PARA IMPLEMENTAR LA SOLUCIÓN:");
            Console.WriteLine("   1. Reemplaza TelegramService por EnhancedTelegramService");
            Console.WriteLine("   2. Reinicia la aplicación");
            Console.WriteLine("   3. Observa los logs detallados en la consola");
            Console.WriteLine("   4. Si persiste, ejecuta desde red diferente para confirmar el problema");
        }
    }
}
