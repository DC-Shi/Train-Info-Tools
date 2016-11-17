using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Routing_Info_Viewer
{
    /// <summary>
    /// Interaction logic for UserControlNumericUpdown.xaml
    /// </summary>
    public partial class UserControlNumericUpdown : UserControl
    {
        public UserControlNumericUpdown()
        {
            InitializeComponent();
            /// http://stackoverflow.com/questions/34869302/c-sharp-defaultvalue-attribute-not-working
            Value = 3;
            Maximum = 10;
            Minimum = 1;
            PrefixIntro = "Prefix";
            PostfixMetric = "KM";
        }

        private void buttonIncrease_Click(object sender, RoutedEventArgs e)
        {
            slider.Value += slider.LargeChange;
        }

        private void buttonDecrease_Click(object sender, RoutedEventArgs e)
        {
            slider.Value -= slider.LargeChange;
        }

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(double), typeof(UserControlNumericUpdown));
        /// <summary>
        /// The value that this control represent.
        /// </summary>
        [Description("Current slider value."), DefaultValue(3.0)]
        public double Value
        {
            get
            {
                return (double)GetValue(ValueProperty);
            }
            set
            {
                SetValue(ValueProperty, value);
            }
        }

        public static readonly DependencyProperty PrefixIntroProperty =
            DependencyProperty.Register("PrefixIntro", typeof(string), typeof(UserControlNumericUpdown));
        /// <summary>
        /// Prefix of the shown text.
        /// </summary>
        [Description("Prefix of the shown text."), DefaultValue("Prefix")]
        public string PrefixIntro
        {
            get
            {
                return GetValue(PrefixIntroProperty).ToString();
            }
            set
            {
                SetValue(PrefixIntroProperty, value);
            }
        }

        public static readonly DependencyProperty PostfixMetricProperty =
            DependencyProperty.Register("PostfixMetric", typeof(string), typeof(UserControlNumericUpdown));
        /// <summary>
        /// Postfix of the shown text, such as: km, kg, ℃, °, etc.
        /// </summary>
        [Description("Current slider value."), DefaultValue("meters")]
        public string PostfixMetric
        {
            get
            {
                return GetValue(PostfixMetricProperty).ToString();
            }
            set
            {
                SetValue(PostfixMetricProperty, value);
            }
        }

        public static readonly DependencyProperty MaximumProperty =
             DependencyProperty.Register("Maximum", typeof(double), typeof(UserControlNumericUpdown));
        /// <summary>
        /// Max number allowed.
        /// </summary>
        [Description("The Maximum"), DefaultValue(10.0)]
        public double Maximum
        {
            get
            {
                return (double)GetValue(MaximumProperty);
            }
            set
            {
                SetValue(MaximumProperty, value);
            }
        }

        public static readonly DependencyProperty MinimumProperty =
             DependencyProperty.Register("Minimum", typeof(double), typeof(UserControlNumericUpdown));
        /// <summary>
        /// Min number allowed.
        /// </summary>
        [Description("The minimum"), DefaultValue(1)]
        public double Minimum
        {
            get
            {
                return (double)GetValue(MinimumProperty);
            }
            set
            {
                SetValue(MinimumProperty, value);
            }
        }

    }
}
