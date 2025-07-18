using ElCriolloAPI.Models;

namespace ElCriolloAPI.Services
{
    public interface IEmailService
    {
        Task<bool> SendWelcomeEmailAsync(string email, string nombreCompleto);
        Task<bool> SendPedidoConfirmationAsync(string email, string nombreCliente, int pedidoId);
        Task<bool> SendAdminNotificationAsync(string message, string subject);
        Task<bool> SendReservaConfirmationAsync(string email, string nombreCliente, DateTime fechaReserva);
        Task<bool> SendMesaStatusChangeAsync(int mesaNumero, string nuevoEstado);
    }

    public class EmailService : IEmailService
    {
        private readonly ILogger<EmailService> _logger;
        private readonly IConfiguration _configuration;

        public EmailService(ILogger<EmailService> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        // 🎉 Email de bienvenida para nuevos usuarios
        public async Task<bool> SendWelcomeEmailAsync(string email, string nombreCompleto)
        {
            try
            {
                var emailHtml = GenerateWelcomeEmailHtml(nombreCompleto);

                // Simular envío
                await SimulateEmailSending();

                _logger.LogInformation("📧 ===== EMAIL ENVIADO EXITOSAMENTE =====");
                _logger.LogInformation("📩 Para: {Email}", email);
                _logger.LogInformation("🎯 Asunto: ¡Bienvenido a El Criollo Restaurant!");
                _logger.LogInformation("👤 Destinatario: {Nombre}", nombreCompleto);
                _logger.LogInformation("🎨 Tipo: Email de Bienvenida");
                _logger.LogInformation("⏰ Enviado: {Timestamp}", DateTime.Now);
                _logger.LogInformation(" Contenido: Email HTML con branding dominicano");
                _logger.LogInformation("========================================");

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error enviando email de bienvenida a {Email}", email);
                return false;
            }
        }

        // 🍽️ Email de confirmación de pedido
        public async Task<bool> SendPedidoConfirmationAsync(string email, string nombreCliente, int pedidoId)
        {
            try
            {
                var emailHtml = GeneratePedidoConfirmationHtml(nombreCliente, pedidoId);

                await SimulateEmailSending();

                _logger.LogInformation("📧 ===== EMAIL PEDIDO ENVIADO =====");
                _logger.LogInformation("📩 Para: {Email}", email);
                _logger.LogInformation("🎯 Asunto: ¡Tu pedido #{PedidoId} está confirmado!", pedidoId);
                _logger.LogInformation("👤 Cliente: {Nombre}", nombreCliente);
                _logger.LogInformation("🆔 Pedido ID: {PedidoId}", pedidoId);
                _logger.LogInformation("🍖 Restaurante: El Criollo - Sabor Dominicano");
                _logger.LogInformation("⏰ Enviado: {Timestamp}", DateTime.Now);
                _logger.LogInformation("==================================");

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error enviando confirmación de pedido {PedidoId}", pedidoId);
                return false;
            }
        }

        // 🚨 Email de notificación para administradores
        public async Task<bool> SendAdminNotificationAsync(string message, string subject)
        {
            try
            {
                var adminEmail = "admin@elcriollo.com";
                var emailHtml = GenerateAdminNotificationHtml(message, subject);

                await SimulateEmailSending();

                _logger.LogInformation("📧 ===== NOTIFICACIÓN ADMIN =====");
                _logger.LogInformation("📩 Para: {AdminEmail}", adminEmail);
                _logger.LogInformation("🎯 Asunto: [ADMIN] {Subject}", subject);
                _logger.LogInformation("💬 Mensaje: {Message}", message);
                _logger.LogInformation("🔔 Tipo: Notificación Administrativa");
                _logger.LogInformation("⏰ Enviado: {Timestamp}", DateTime.Now);
                _logger.LogInformation("===============================");

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error enviando notificación admin");
                return false;
            }
        }

        // 📅 Email de confirmación de reserva
        public async Task<bool> SendReservaConfirmationAsync(string email, string nombreCliente, DateTime fechaReserva)
        {
            try
            {
                var emailHtml = GenerateReservaConfirmationHtml(nombreCliente, fechaReserva);

                await SimulateEmailSending();

                _logger.LogInformation("📧 ===== EMAIL RESERVA ENVIADO =====");
                _logger.LogInformation("📩 Para: {Email}", email);
                _logger.LogInformation("🎯 Asunto: ¡Reserva confirmada en El Criollo!");
                _logger.LogInformation("👤 Cliente: {Nombre}", nombreCliente);
                _logger.LogInformation("📅 Fecha: {FechaReserva:yyyy-MM-dd HH:mm}", fechaReserva);
                _logger.LogInformation("🏢 Restaurante: El Criollo Restaurant");
                _logger.LogInformation("⏰ Enviado: {Timestamp}", DateTime.Now);
                _logger.LogInformation("===================================");

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error enviando confirmación de reserva");
                return false;
            }
        }

        // 🔄 Notificación de cambio de estado de mesa
        public async Task<bool> SendMesaStatusChangeAsync(int mesaNumero, string nuevoEstado)
        {
            try
            {
                await SimulateEmailSending();

                _logger.LogInformation("📧 ===== CAMBIO ESTADO MESA =====");
                _logger.LogInformation("🔔 Tipo: Notificación Interna");
                _logger.LogInformation("🪑 Mesa: #{MesaNumero}", mesaNumero);
                _logger.LogInformation("🔄 Nuevo Estado: {Estado}", nuevoEstado);
                _logger.LogInformation("👨‍💼 Para: Staff del restaurante");
                _logger.LogInformation("⏰ Timestamp: {Timestamp}", DateTime.Now);
                _logger.LogInformation("===============================");

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error notificando cambio de estado mesa {Mesa}", mesaNumero);
                return false;
            }
        }

        // 🎨 Templates HTML simulados
        private string GenerateWelcomeEmailHtml(string nombreCompleto)
        {
            return $@"
                <!DOCTYPE html>
                <html>
                <head><title>Bienvenido a El Criollo</title></head>
                <body style='font-family: Arial; background: #f4f4f4; padding: 20px;'>
                    <div style='background: white; padding: 30px; border-radius: 10px; max-width: 600px; margin: 0 auto;'>
                        <h1 style='color: #d63384; text-align: center;'> El Criollo Restaurant</h1>
                        <h2>¡Bienvenido, {nombreCompleto}!</h2>
                        <p>Gracias por registrarte en El Criollo, donde el sabor dominicano cobra vida.</p>
                        <p>Disfruta de nuestros platos tradicionales: Pollo Guisado, Chivo, Moro de Guandules y mucho más.</p>
                        <p style='text-align: center; margin-top: 30px;'>
                            <strong style='color: #d63384;'>¡Sabor Dominicano Auténtico! 🇩🇴</strong>
                        </p>
                    </div>
                </body>
                </html>";
        }

        private string GeneratePedidoConfirmationHtml(string nombreCliente, int pedidoId)
        {
            return $@"
                <!DOCTYPE html>
                <html>
                <body style='font-family: Arial; padding: 20px;'>
                    <div style='background: white; padding: 20px; border: 2px solid #d63384;'>
                        <h2> Pedido Confirmado</h2>
                        <p>Estimado/a {nombreCliente},</p>
                        <p>Su pedido #{pedidoId} ha sido recibido y está en preparación.</p>
                        <p><strong>¡Gracias por elegir El Criollo!</strong></p>
                    </div>
                </body>
                </html>";
        }

        private string GenerateAdminNotificationHtml(string message, string subject)
        {
            return $@"
                <!DOCTYPE html>
                <html>
                <body style='font-family: Arial;'>
                    <div style='background: #ffe6e6; padding: 15px; border-left: 4px solid #d63384;'>
                        <h3>🚨 {subject}</h3>
                        <p>{message}</p>
                        <p><em>Sistema El Criollo</em></p>
                    </div>
                </body>
                </html>";
        }

        private string GenerateReservaConfirmationHtml(string nombreCliente, DateTime fechaReserva)
        {
            return $@"
                <!DOCTYPE html>
                <html>
                <body style='font-family: Arial;'>
                    <div style='background: #e8f5e8; padding: 20px;'>
                        <h2>📅 Reserva Confirmada</h2>
                        <p>Estimado/a {nombreCliente},</p>
                        <p>Su reserva para el {fechaReserva:dd/MM/yyyy} está confirmada.</p>
                        <p>¡Lo esperamos en El Criollo!</p>
                    </div>
                </body>
                </html>";
        }

        // ⏰ Simular delay de envío de email
        private async Task SimulateEmailSending()
        {
            // Simular tiempo de envío realista
            await Task.Delay(Random.Shared.Next(500, 1500));
        }
    }
}
    
