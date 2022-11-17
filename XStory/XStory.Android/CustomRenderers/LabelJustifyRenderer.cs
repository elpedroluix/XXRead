using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Forms.Platform.Android;
using Xamarin.Forms;
using XStory.Droid.CustomRenderers;
using XStory.Helpers.CustomRenderers;

[assembly: ExportRenderer(typeof(LabelJustify), typeof(LabelJustifyRenderer))]
namespace XStory.Droid.CustomRenderers
{
    class LabelJustifyRenderer : LabelRenderer
    {
        public LabelJustifyRenderer(Context context) : base(context)
        {

        }
        protected override void OnElementChanged(ElementChangedEventArgs<Label> e)
        {
            base.OnElementChanged(e);

            if (Control != null)
            {
                Control.JustificationMode = Android.Text.JustificationMode.InterWord;
            }
        }
    }
}