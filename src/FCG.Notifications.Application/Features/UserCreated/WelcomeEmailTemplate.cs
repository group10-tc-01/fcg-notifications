using FCG.Notifications.Application.Common.EmailTemplates;

namespace FCG.Notifications.Application.Features.UserCreated
{
    public class WelcomeEmailTemplate : EmailTemplateBase
    {
        private readonly string _userName;

        public WelcomeEmailTemplate(string userName)
        {
            _userName = userName;
        }

        protected override string Subject => "Bem-vindo à Nossa Plataforma!";

        protected override string BodyContent => $@"
            <h2>Olá, {_userName}!</h2>
            
            <p>Estamos empolgados em dar as boas-vindas à nossa plataforma! 🎉</p>
            
            <p>Sua conta foi criada com sucesso e agora você faz parte da nossa comunidade em crescimento.</p>
            
            <div class=""highlight"">
                <strong>Quais são os próximos passos?</strong>
                <ul>
                    <li>Complete seu perfil</li>
                    <li>Explore nossos recursos</li>
                    <li>Conecte-se com outros usuários</li>
                </ul>
            </div>
            
            <p>Se você tiver alguma dúvida ou precisar de assistência, nossa equipe de suporte está aqui para ajudar. Sinta-se à vontade para entrar em contato a qualquer momento!</p>
            
            <p style=""margin-top: 30px;"">
                <strong>Atenciosamente,</strong><br/>
                Equipe FCG Notifications
            </p>";

        protected override string GetHeaderTitle()
        {
            return $"Bem-vindo, {_userName}!";
        }

        protected override string GetCssStyles()
        {
            return base.GetCssStyles() + @"
                .email-body h2 {
                    color: #667eea;
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
