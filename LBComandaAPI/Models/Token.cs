﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LBComandaAPI.Models
{
    public class Token
    {
        public string Cd_empresa { get; set; } = string.Empty;
        public int Id_token { get; set; }
        public DateTime Dt_token { get; set; }
        public int Temp_validade { get; set; }
    }
}
