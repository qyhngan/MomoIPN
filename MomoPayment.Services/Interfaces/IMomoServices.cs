using MomoPayment.Services.ViewModels;

namespace Services.Interfaces
{
    public interface IMomoServices
    {
        bool VerifyMomoCallback(CallbackViaMomo callbackViaMomo);
    }
}
