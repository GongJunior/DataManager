using System.Windows;
using System;

namespace DataManager
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class ProgressBarTask : Window
    {
        #region PROPERTIES
        public string Message
        {
            set { workStatus.Text = value; }
        }
        #endregion

        #region METHODS

        public ProgressBarTask()
        {
            InitializeComponent();
            this.Title = "Work Progress";
        }

        #endregion

        #region EVENTS

        public event EventHandler<EventArgs> Canceled;

        private void CancelButton_Click(object sender, EventArgs e)
        {
            EventHandler<EventArgs> ea = Canceled;
            if (ea != null) ea(this, e);
        }

        #endregion
    }
}
