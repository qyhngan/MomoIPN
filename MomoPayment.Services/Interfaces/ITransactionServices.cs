using MomoPayment.Services.ViewModels;

namespace Services.Interfaces
{
    public interface ITransactionServices
    {
        Task<bool> MomoCallBackIpn(CallbackViaMomo callbackViaMomo);
    }
}
