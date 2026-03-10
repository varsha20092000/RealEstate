using Microsoft.Extensions.Options;
using Razorpay.Api;
using RealEstate.Application.Settings;

namespace RealEstate.API.Services;

public class PaymentSettings
{
    public string KeyId { get; set; } = string.Empty;
    public string KeySecret { get; set; } = string.Empty;
}

public class PaymentService
{
    private readonly string _keyId;
    private readonly string _keySecret;

    public PaymentService(IConfiguration configuration)
    {
        _keyId = configuration["RazorpaySettings:KeyId"] ?? "";
        _keySecret = configuration["RazorpaySettings:KeySecret"] ?? "";
    }

    public Dictionary<string, string> CreateOrder(decimal amount, string currency = "INR")
    {
        RazorpayClient client = new RazorpayClient(_keyId, _keySecret);

        Dictionary<string, object> options = new Dictionary<string, object>
        {
            { "amount", (int)(amount * 100) }, // amount in paise
            { "currency", currency },
            { "receipt", Guid.NewGuid().ToString("N").Substring(0, 20) },
            { "payment_capture", 1 }
        };

        Order order = client.Order.Create(options);

        return new Dictionary<string, string>
        {
            { "orderId", order["id"].ToString() },
            { "amount", order["amount"].ToString() },
            { "currency", order["currency"].ToString() },
            { "keyId", _keyId }
        };
    }

    public bool VerifyPayment(string orderId, string paymentId, string signature)
    {
        try
        {
            RazorpayClient client = new RazorpayClient(_keyId, _keySecret);
            Dictionary<string, string> attributes = new Dictionary<string, string>
            {
                { "razorpay_order_id", orderId },
                { "razorpay_payment_id", paymentId },
                { "razorpay_signature", signature }
            };
            Utils.verifyPaymentSignature(attributes);
            return true;
        }
        catch
        {
            return false;
        }
    }
}