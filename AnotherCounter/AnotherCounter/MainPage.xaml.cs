using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Xamarin.Essentials;
using Xamarin.Forms;
using System.Diagnostics;

namespace AnotherCounter
{
    public class CounterModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private int count = 0;
        public int Count
        {
            get => count;
            set
            {
                count = value;
                OnPropertyChanged("Count");
            }
        }
        private double cps = 0;
        public double Cps
        {
            get => cps;
            set
            {
                cps = value;
                OnPropertyChanged("Cps");
            }
        }
        public CounterModel()
        {
        }
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
    public partial class MainPage : ContentPage
    {
        private bool isClicking = false;
        private readonly CounterModel model = new CounterModel();
        private readonly Stopwatch timer = new Stopwatch();
        public MainPage()
        {
            InitializeComponent();
            BindingContext = model;
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            if (!isClicking)
            {
                timer.Reset();
                timer.Start();
                isClicking = true;
                Clicker.BackgroundColor = Color.LightGreen;
                model.Count = 1;
                model.Cps = 0;
                Task.Delay(5000)
                     .ContinueWith(async (_) =>
                     {
                         await MainThread.InvokeOnMainThreadAsync(() =>
                         {
                             isClicking = false;
                             model.Cps = model.Count / timer.Elapsed.TotalSeconds;
                             timer.Stop();
                             Clicker.IsEnabled = false;
                             Clicker.BackgroundColor = Color.Red;
                         });
                         await Task.Delay(2000)
                            .ContinueWith(async (__) =>
                            {
                                await MainThread.InvokeOnMainThreadAsync(() =>
                                {
                                    Clicker.IsEnabled = true;
                                    Clicker.BackgroundColor = Color.LightBlue;
                                });
                            });
                     });
                Task.Run(async () =>
                {
                    while (isClicking)
                    {
                        model.Cps = model.Count / timer.Elapsed.TotalSeconds;
                        await Task.Delay(100);
                    }
                });
            }
            else
            {
                model.Count += 1;
            }
        }
    }
}
