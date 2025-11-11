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
            ? "Payment Confirmed - Transaction Successful" 
            : "Payment Failed - Action Required";

        protected override string BodyContent => _isSuccessful 
            ? GetSuccessContent() 
            : GetFailureContent();

        private string GetSuccessContent()
        {
            return @"
            <h2>Payment Successful! 💳✅</h2>
            
            <p>Great news! Your payment has been processed successfully.</p>
            
            <div class=""highlight"" style=""background-color: #d4edda; border-left-color: #28a745;"">
                <strong>Transaction Details</strong>
                <ul>
                    <li><strong>Status:</strong> <span style=""color: #28a745; font-weight: bold;"">Confirmed</span></li>
                    <li><strong>Account:</strong> " + _userEmail + @"</li>
                    <li><strong>Date:</strong> " + DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss UTC") + @"</li>
                </ul>
            </div>
            
            <p>You will receive a detailed receipt shortly.</p>
            
            <p>Thank you for your business!</p>
            
            <p style=""margin-top: 30px;"">
                <strong>Best regards,</strong><br/>
                The FCG Notifications Team
            </p>";
        }

        private string GetFailureContent()
        {
            return @"
            <h2>Payment Failed ⚠️</h2>
            
            <p>Unfortunately, we were unable to process your payment.</p>
            
            <div class=""highlight"" style=""background-color: #f8d7da; border-left-color: #dc3545;"">
                <strong>Transaction Details</strong>
                <ul>
                    <li><strong>Status:</strong> <span style=""color: #dc3545; font-weight: bold;"">Failed</span></li>
                    <li><strong>Account:</strong> " + _userEmail + @"</li>
                    <li><strong>Date:</strong> " + DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss UTC") + @"</li>
                </ul>
            </div>
            
            <p><strong>Common reasons for payment failure:</strong></p>
            <ul>
                <li>Insufficient funds</li>
                <li>Incorrect payment information</li>
                <li>Card expired or blocked</li>
                <li>Bank declined the transaction</li>
            </ul>
            
            <p>Please verify your payment information and try again.</p>
            
            <a href=""#"" class=""button"" style=""background-color: #dc3545;"">Try Again</a>
            
            <p style=""margin-top: 30px;"">
                If you need assistance, please contact our support team.<br/>
                <strong>Best regards,</strong><br/>
                The FCG Notifications Team
            </p>";
        }

        protected override string GetHeaderTitle()
        {
            return _isSuccessful ? "Payment Confirmed" : "Payment Failed";
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
