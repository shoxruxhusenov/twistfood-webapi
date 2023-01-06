﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwistFood.Service.Dtos;
using TwistFood.Service.Dtos.Accounts;

namespace TwistFood.Service.Interfaces.Accounts
{
    public interface ISendToPhoneNumberService
    {
        public Task<int> SendCodeAsync(SendToPhoneNumberDto sendToPhoneNumberDto);
    }
}
