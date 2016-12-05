using 客里表Library.Database;
using Routing_Info_Viewer.layout_class;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

/// <summary>
/// Ref from:
/// https://msdn.microsoft.com/en-us/library/system.windows.controls.itemscontrol.itemtemplateselector%28v=vs.110%29.aspx
/// </summary>
namespace Routing_Info_Viewer
{
    class ClassRoutingTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            FrameworkElement element = container as FrameworkElement;

            if (element != null && item != null)
            {
                if (item is Class线路里程)
                {
                    return element.FindResource("StationInfoTemplate") as DataTemplate;
                }
                if (item is ClassIntervalInfo)
                {
                    return element.FindResource("IntervalInfoTemplate") as DataTemplate;
                }
            }

            return null;
        }

    }
}
