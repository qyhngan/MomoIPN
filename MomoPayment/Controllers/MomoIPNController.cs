using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MomoPayment.Services.ViewModels;
using Services.Interfaces;

namespace WebAPI.Controllers
{
    [Route("api/v0/momo")]
    [ApiController]
    public class MomoIPNController : ControllerBase
    {
        private readonly ITransactionServices _transactionServices;

        public MomoIPNController(ITransactionServices transactionServices)
        {
            _transactionServices = transactionServices;
        }

        [HttpPost("/callback-ipn")]
        public async Task<IActionResult> MomoCallBackIpn([FromBody] CallbackViaMomo transaction)
        {
            try
            {
                var result = await _transactionServices.MomoCallBackIpn(transaction);

                if (result == false)
                {
                    return BadRequest(new
                    {
                        Message = "Cập nhật thanh toán thất bại"
                    });
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    Message = ex.Message
                });
            }
        }
    }
}
