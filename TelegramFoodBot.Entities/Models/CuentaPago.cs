namespace TelegramFoodBot.Entities.Models
{
    public class CuentaPago
    {
        public int Id { get; set; }
        public string TipoCuenta { get; set; }        // Ej: "Cuenta Bancaria"
        public string Banco { get; set; }             // Ej: "Bancolombia"
        public string Numero { get; set; }
        public string Titular { get; set; }
        public string Instrucciones { get; set; }
        public string Estado { get; set; }            // "Activa", "Inactiva"
    }
}
