﻿using Net.payOS.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.Payos.Interface
{
	public interface IPayosService
	{
		Task<CreatePaymentResult> CreatePayOSPaymentUrl(PaymentData paymentData);
		Task<PaymentLinkInformation> GetPaymentLinkInfor(long id);
		Task<PaymentLinkInformation> CancelPaymentLink(long orderCode, string cancellationReason);

	}
}
