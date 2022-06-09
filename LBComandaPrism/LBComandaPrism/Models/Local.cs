using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Text;

namespace LBComandaPrism.Models
{
    public class Local: BindableBase
    {
        private decimal _id_local;
        public decimal Id_local { get { return _id_local; } set { SetProperty(ref _id_local, value); } }
        private string _ds_local = string.Empty;
        public string Ds_local { get { return _ds_local; } set { SetProperty(ref _ds_local, value); } }
    }
}
