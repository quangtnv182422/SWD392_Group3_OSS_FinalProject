using Data.Models.Vnpay;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.vnPay.Interface
{
	public interface IVnPayProxy
	{
		string CreatePaymentUrl(PaymentInformationModel model, HttpContext context);
		PaymentResponseModel PaymentExecute(IQueryCollection collections);

	}
}
