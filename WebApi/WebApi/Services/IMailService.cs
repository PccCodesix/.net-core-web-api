﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi.Services
{
    public interface IMailService
    {
        void Send(string subject, string msg);

    }
}
