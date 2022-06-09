using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using LBComandaPrism.Interface;
using Xamarin.Forms;

[assembly: Dependency(typeof(LBComandaPrism.Droid.CloseApplication))]
namespace LBComandaPrism.Droid
{
    public class CloseApplication : ICloseApplication
    {
        [Obsolete]
        public void closeApplication()
        {
            Activity activity = (Activity)Forms.Context;
            activity.FinishAffinity();
        }
    }
}