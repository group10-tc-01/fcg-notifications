using FCG.Notifications.Application.Common.Abstractions;
using System.Text;

namespace FCG.Notifications.Application.Common.EmailTemplates
{
    public abstract class EmailTemplateBase : IEmailTemplate
    {
        protected abstract string Subject { get; }
        protected abstract string BodyContent { get; }

        public string GetSubject() => Subject;

        public string GetHtmlContent()
        {
            var html = new StringBuilder();

            html.AppendLine("<!DOCTYPE html>");
            html.AppendLine("<html lang=\"pt-BR\">");
            html.AppendLine(GetHeadSection());
            html.AppendLine("<body>");
            html.AppendLine(GetBodyWrapper());
            html.AppendLine("</body>");
            html.AppendLine("</html>");

            return html.ToString();
        }

        protected virtual string GetHeadSection()
        {
            return @"
            <head>
                <meta charset=""UTF-8"">
                <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
                <title>" + Subject + @"</title>
                <style>
                    " + GetCssStyles() + @"
                </style>
            </head>";
        }

        protected virtual string GetCssStyles()
        {
            return @"
                body {
                    font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
                    line-height: 1.6;
                    color: #333;
                    background-color: #f4f4f4;
                    margin: 0;
                    padding: 0;
                }
                .email-container {
                    max-width: 600px;
                    margin: 20px auto;
                    background-color: #ffffff;
                    border-radius: 8px;
                    box-shadow: 0 2px 4px rgba(0,0,0,0.1);
                    overflow: hidden;
                }
                .email-header {
                    background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
                    color: #ffffff;
                    padding: 30px;
                    text-align: center;
                }
                .email-header h1 {
                    margin: 0;
                    font-size: 28px;
                    font-weight: 600;
                }
                .email-body {
                    padding: 40px 30px;
                }
                .email-body p {
                    margin: 15px 0;
                    font-size: 16px;
                }
                .email-footer {
                    background-color: #f8f9fa;
                    padding: 20px 30px;
                    text-align: center;
                    font-size: 14px;
                    color: #6c757d;
                    border-top: 1px solid #dee2e6;
                }
                .button {
                    display: inline-block;
                    padding: 12px 30px;
                    margin: 20px 0;
                    background-color: #667eea;
                    color: #ffffff;
                    text-decoration: none;
                    border-radius: 5px;
                    font-weight: 600;
                    transition: background-color 0.3s ease;
                }
                .button:hover {
                    background-color: #764ba2;
                }
                .highlight {
                    background-color: #fff3cd;
                    padding: 15px;
                    border-left: 4px solid #ffc107;
                    margin: 20px 0;
                    border-radius: 4px;
                }
                img.logo {
                    max-width: 150px;
                    margin-bottom: 10px;
                }
            ";
        }

        protected virtual string GetBodyWrapper()
        {
            return $@"
            <div class=""email-container"">
                {GetHeaderSection()}
                <div class=""email-body"">
                    {BodyContent}
                </div>
                {GetFooterSection()}
            </div>";
        }

        protected virtual string GetHeaderSection()
        {
            return @"
            <div class=""email-header"">
                <h1>" + GetHeaderTitle() + @"</h1>
            </div>";
        }

        protected virtual string GetHeaderTitle()
        {
            return Subject;
        }

        protected virtual string GetFooterSection()
        {
            return @"
            <div class=""email-footer"">
                <p>Esta é uma mensagem automatizada, por favor não responda.</p>
                <p>&copy; 2025 FCG Notifications. Todos os direitos reservados.</p>
            </div>";
        }
    }
}
