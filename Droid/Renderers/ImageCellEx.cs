using System;
using Xamarin.Forms;
using CoffeeCups.Droid;
using Xamarin.Forms.Platform.Android;
using Android.Widget;
using Android.Content;
using Android.Util;
using Android.Views;
using CoffeeCups;

[assembly:ExportCell(typeof(MyTextCell), typeof(TetCellEx))]
namespace CoffeeCups.Droid
{
    public class TetCellEx: TextCellRenderer
    {
        protected override Android.Views.View GetCellCore(Cell item, Android.Views.View convertView, Android.Views.ViewGroup parent, Context context)
        {
            var cell = (LinearLayout)base.GetCellCore(item, convertView, parent, context);
           

            var layout = cell.GetChildAt(1) as LinearLayout;
            if(layout == null)
                return cell;

            var label = layout.GetChildAt(0) as TextView;
            if (label == null)
                return cell;

            label.SetTextSize(ComplexUnitType.Dip, 16);
            label.SetTextColor(context.Resources.GetColor(Android.Resource.Color.PrimaryTextLight));

            var secondaryLabel = layout.GetChildAt(1) as TextView;
            if (secondaryLabel == null)
                return cell;

            secondaryLabel.SetTextSize(ComplexUnitType.Dip, 13);
            secondaryLabel.SetTextColor(context.Resources.GetColor(Android.Resource.Color.SecondaryTextLight));
            return cell;
        }

    }
}