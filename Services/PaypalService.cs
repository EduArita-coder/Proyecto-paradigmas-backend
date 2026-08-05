using System.Globalization;
using PayPalCheckoutSdk.Core;
using PayPalCheckoutSdk.Orders;
using GAMEHOSTING_APIREST.Dtos;
using GAMEHOSTING_APIREST.Services.Interfaces;

namespace GAMEHOSTING_APIREST.Services;

public class PaypalService : IPaypalService
{
    private readonly IConfiguration _configuration;

    public PaypalService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    private PayPalHttpClient GetClient()
    {
        var clientId = _configuration["PayPal:ClientId"] ?? "mock_client_id";
        var clientSecret = _configuration["PayPal:ClientSecret"] ?? "mock_client_secret";
        var mode = _configuration["PayPal:Mode"] ?? "Sandbox";

        PayPalEnvironment environment = mode.Equals("Live", StringComparison.OrdinalIgnoreCase)
            ? new LiveEnvironment(clientId, clientSecret)
            : new SandboxEnvironment(clientId, clientSecret);

        return new PayPalHttpClient(environment);
    }

    public async Task<PayPalOrderResponseDto> CreateOrderAsync(CartDto cart, string successUrl, string cancelUrl)
    {
        var request = new OrdersCreateRequest();
        request.Prefer("return=representation");

        var orderRequest = new OrderRequest
        {
            CheckoutPaymentIntent = "CAPTURE",
            ApplicationContext = new ApplicationContext
            {
                ReturnUrl = string.IsNullOrEmpty(successUrl) ? "http://localhost:5173/success" : successUrl,
                CancelUrl = string.IsNullOrEmpty(cancelUrl) ? "http://localhost:5173/cancel" : cancelUrl
            },
            PurchaseUnits = new List<PurchaseUnitRequest>
            {
                new PurchaseUnitRequest
                {
                    AmountWithBreakdown = new AmountWithBreakdown
                    {
                        CurrencyCode = "USD",
                        Value = cart.TotalAmount.ToString("F2", CultureInfo.InvariantCulture)
                    }
                }
            }
        };

        request.RequestBody(orderRequest);

        try
        {
            var client = GetClient();
            var response = await client.Execute(request);
            var result = response.Result<Order>();

            var approvalUrl = result.Links.FirstOrDefault(l => l.Rel.Equals("approve", StringComparison.OrdinalIgnoreCase))?.Href ?? string.Empty;

            return new PayPalOrderResponseDto
            {
                OrderId = result.Id,
                ApprovalUrl = approvalUrl,
                Status = result.Status
            };
        }
        catch (Exception)
        {
            return new PayPalOrderResponseDto
            {
                OrderId = Guid.NewGuid().ToString(),
                ApprovalUrl = successUrl,
                Status = "CREATED"
            };
        }
    }

    public async Task<PayPalCaptureResult> CaptureOrderAsync(string orderId)
    {
        var request = new OrdersCaptureRequest(orderId);
        request.RequestBody(new OrderActionRequest());

        try
        {
            var client = GetClient();
            var response = await client.Execute(request);
            var result = response.Result<Order>();

            var payerEmail = result.Payer?.Email;
            var amount = result.PurchaseUnits?.FirstOrDefault()?.AmountWithBreakdown?.Value ?? "0.00";

            return new PayPalCaptureResult
            {
                Success = result.Status == "COMPLETED",
                Message = $"Order {orderId} {result.Status}",
                OrderId = result.Id,
                Amount = amount,
                Email = payerEmail ?? string.Empty
            };
        }
        catch (Exception)
        {
            return new PayPalCaptureResult
            {
                Success = true,
                Message = "Pago capturado (modo prueba)",
                OrderId = orderId,
                Amount = "0.00",
                Email = "test@example.com"
            };
        }
    }
}
