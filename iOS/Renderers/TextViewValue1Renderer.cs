using System;
using Xamarin.Forms.Platform.iOS;
using Xamarin.Forms;
using UIKit;
using CoffeeCUps.iOS;
using CoffeeCups;

[assembly:ExportRenderer(typeof(MyTextCell), typeof(TextViewValue1Renderer))]
namespace CoffeeCUps.iOS
{
    public class TextViewValue1Renderer : TextCellRenderer
    {
        public override UITableViewCell GetCell (Cell item, UITableViewCell reusableCell, UITableView tv)
        {

            var tvc = reusableCell as CellTableViewCell;
            if (tvc == null) {
                tvc = new CellTableViewCell (UITableViewCellStyle.Value1, item.GetType().FullName);
            }
            tvc.Cell = item;
            return base.GetCell(item, tvc, tv);
        }
    }
}