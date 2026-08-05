using GAMEHOSTING_APIREST.Dtos;

namespace GAMEHOSTING_APIREST.Services.Interfaces;

public interface IPaypalService
{
    Task<PayPalOrderResponseDto> CreateOrderAsync(CartDto cart, string successUrl, string cancelUrl);
    Task<PayPalCaptureResult> CaptureOrderAsync(string orderId);
}
