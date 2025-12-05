namespace FCG.Notifications.Infrastructure.EmailTemplates
{
    public class PaymentProcessedEmailTemplate : EmailTemplateBase
    {
        private readonly string _userEmail;
        private readonly bool _isSuccessful;

        public PaymentProcessedEmailTemplate(string userEmail, bool isSuccessful)
        {
            _userEmail = userEmail;
            _isSuccessful = isSuccessful;
        }

        protected override string Subject => _isSuccessful
            ? "Pagamento Confirmado - Transação Bem-sucedida"
            : "Pagamento Falhou - Ação Necessária";

        protected override string BodyContent => _isSuccessful
            ? GetSuccessContent()
            : GetFailureContent();

        private string GetSuccessContent()
        {
            return @"
            <h2>Pagamento Realizado com Sucesso! 💳✅</h2>
            
            <p>Ótima notícia! Seu pagamento foi processado com sucesso.</p>
            
            <div class=""highlight"" style=""background-color: #d4edda; border-left-color: #28a745;"">
                <strong>Detalhes da Transação</strong>
                <ul>
                    <li><strong>Status:</strong> <span style=""color: #28a745; font-weight: bold;"">Confirmado</span></li>
                    <li><strong>Conta:</strong> " + _userEmail + @"</li>
                    <li><strong>Data:</strong> " + DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm:ss UTC") + @"</li>
                </ul>
            </div>
            
            <p>Você receberá um recibo detalhado em breve.</p>
            
            <p>Obrigado por fazer negócios conosco!</p>
            
            <p style=""margin-top: 30px;"">
                <strong>Atenciosamente,</strong><br/>
                Equipe FCG Notifications
            </p>";
        }

        private string GetFailureContent()
        {
            return @"
            <h2>Pagamento Falhou ⚠️</h2>
            
            <p>Infelizmente, não conseguimos processar seu pagamento.</p>
            
            <div class=""highlight"" style=""background-color: #f8d7da; border-left-color: #dc3545;"">
                <strong>Detalhes da Transação</strong>
                <ul>
                    <li><strong>Status:</strong> <span style=""color: #dc3545; font-weight: bold;"">Falhou</span></li>
                    <li><strong>Conta:</strong> " + _userEmail + @"</li>
                    <li><strong>Data:</strong> " + DateTime.UtcNow.ToString("dd/MM/yyyy HH:mm:ss UTC") + @"</li>
                </ul>
            </div>
            
            <p><strong>Motivos comuns para falha no pagamento:</strong></p>
            <ul>
                <li>Fundos insuficientes</li>
                <li>Informações de pagamento incorretas</li>
                <li>Cartão expirado ou bloqueado</li>
                <li>Banco recusou a transação</li>
            </ul>
            
            <p>Por favor, verifique suas informações de pagamento e tente novamente.</p>
            
            <p style=""margin-top: 30px;"">
                Se precisar de assistência, entre em contato com nossa equipe de suporte.<br/>
                <strong>Atenciosamente,</strong><br/>
                Equipe FCG Notifications
            </p>";
        }

        protected override string GetHeaderTitle()
        {
            return _isSuccessful ? "Pagamento Confirmado" : "Pagamento Falhou";
        }

        protected override string GetCssStyles()
        {
            return base.GetCssStyles() + @"
        .email-body h2 {
            color: " + (_isSuccessful ? "#28a745" : "#dc3545") + @";
            margin-top: 0;
            font-size: 24px;
        }
        .email-body ul {
            margin: 10px 0;
            padding-left: 20px;
        }
        .email-body ul li {
            margin: 8px 0;
        }
    ";
        }
    }
}