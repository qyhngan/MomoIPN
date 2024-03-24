using Microsoft.Extensions.Options;
using MomoPayment.Services.ViewModels;
using Repositories;
using Repositories.Models;
using Services.Interfaces;

namespace Services.Services
{
    public class TransactionServices : ITransactionServices
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMomoServices _momoServices;
        private readonly IOptions<MomoModel> _options;

        public TransactionServices(IUnitOfWork unitOfWork, IMomoServices momoServices, IOptions<MomoModel> options)
        {
            _unitOfWork = unitOfWork;
            _momoServices = momoServices;
            _options = options;
        }

        public async Task<bool> MomoCallBackIpn(CallbackViaMomo callbackViaMomo)
        {
            try
            {
                Console.WriteLine("call back đã đc gọi");
                var verify = _momoServices.VerifyMomoCallback(callbackViaMomo);
                if (verify)
                {
                    var isSuccess = callbackViaMomo.ResultCode == 0;
                    if (!isSuccess)
                    {
                        throw new Exception("Giao dịch Momo không thành công");
                    }

                    var orderId = callbackViaMomo.OrderId;
                    var orderInfo = callbackViaMomo.OrderInfo;
                    var pointValue = callbackViaMomo.Amount;
                    Guid currentUserId = Guid.Parse(callbackViaMomo.ExtraData);
                    
                    var transaction = new Transaction()
                    {
                        UserId = currentUserId,
                        PointValue = (int)pointValue,
                        TransactionCode = callbackViaMomo.TransId.ToString(),
                        CreatedOn = DateTime.Now,
                        Type = 0,

                    };
                    _unitOfWork.TransactionRepo.AddAsync(transaction);
                    
                    var user = await _unitOfWork.UserRepo.FindByField(user => user.UserId == currentUserId);
                    user.Point += (int)pointValue;
                    _unitOfWork.UserRepo.Update(user);

                    var result = await _unitOfWork.SaveChangesAsync();

                    return result > 0 ? true : false;                                                   
                }
                else
                {
                    throw new Exception("Giao dịch Momo không hợp lệ");
                }
            }
            catch (Exception e)
            {
                throw new Exception("Lỗi ở TransactionServices - CallbackMomo: " + e.Message);
            }
        }
    }
}
