using Api.Payos.Interface;
using Microsoft.Extensions.Configuration;
using Net.payOS;
using Net.payOS.Types;


namespace Api.Payos.Implementation
{
	public class PayosProxy : IPayosProxy
	{

		
		private readonly PayOS payOS;

		public PayosProxy(IConfiguration configuration)
		{
			var clientId = configuration["PayOS:ClientID"];
			var apiKey = configuration["PayOS:APIKey"];
			var checksumKey = configuration["PayOS:ChecksumKey"];

			payOS = new PayOS(clientId, apiKey, checksumKey);

		}

		public async Task<CreatePaymentResult> CreatePayOSPaymentUrl(PaymentData paymentData)
		{
			return await payOS.createPaymentLink(paymentData);
		}

		public async Task<PaymentLinkInformation> GetPaymentLinkInfor(long id)
		{
			return await payOS.getPaymentLinkInformation(id);
		}

		public async Task<PaymentLinkInformation> CancelPaymentLink(long orderCode, string cancellationReason)
		{
			return await payOS.cancelPaymentLink(orderCode, cancellationReason);
		}

	}
}
