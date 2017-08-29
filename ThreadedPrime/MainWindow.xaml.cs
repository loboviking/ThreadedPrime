using System;
using System.Collections.Generic;
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
using System.ComponentModel;
using System.Windows.Threading;


namespace ThreadedPrime
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private BackgroundWorker worker;
        private bool timesUp = false;
        private DispatcherTimer timer;
        private int ticks = 0;
        private int largestPrime = 0;

        public MainWindow()
        {
            InitializeComponent();

            PrintButton.IsEnabled = false;

            worker = new BackgroundWorker();
            worker.DoWork += worker_DoWork;
            worker.RunWorkerCompleted += worker_RunWorkerCompleted;

            timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(0, 0, 1);
            timer.Tick += timer_Tick;

        }

        void timer_Tick(object sender, EventArgs e)
        {
            ticks++;

            if (ticks > 60)
            {
                timesUp = true;
            }
            // Show in UI as largest prime every 60 seconds
            PrimeBox.Text = largestPrime.ToString();
        }

        /// <summary>
        /// Updates the UI with the value of the current prime
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            string prime = e.ProgressPercentage.ToString();

            PrimeBox.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, new Action(() => { PrimeBox.Text = prime; }));
        }


        void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // Enable PrimeButton and PrintButton now that it is finished generating primes
            PrimeButton.IsEnabled = true;
            PrintButton.IsEnabled = true;
            timesUp = false;

            PrimeBox.Text = largestPrime.ToString();
        }

        /// <summary>
        /// Generate as many prime numbers as possible in 60 seconds
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            int i = 2;

            // stop loop after 60 seconds
            timer.Start();

            while(!timesUp)
            {
                // Check if i is prime and show if prime
                if (IsPrimeNumber(i))
                {
                    largestPrime = i;
                }
                i++;
            }
        }


        private void PrimeButton_Click(object sender, RoutedEventArgs e)
        {
            worker.RunWorkerAsync();

            // Disable PrimeButton and PrintButton while it is generating primes
            this.PrimeButton.IsEnabled = false;
            this.PrintButton.IsEnabled = false;
        }

        // Determines if passed in number is prime by checking if the number is divisible
        // (with no remainder) by 2 up to the square root of the number. This heuristic 
        // provides for an efficient yet simple implementation.
        private bool IsPrimeNumber(int number)
        {
            bool retValue = true;

            if (number <= 1)
            {
                retValue = false;
            }
            // 1, 2, and 3 (the obvious cases) return true as prime
            else if (number > 3)
            {
                double squareNum = System.Math.Sqrt(number);

                for (int i = 2; i <= squareNum; i++ )
                {
                    if (number % i == 0)
                    {
                        retValue = false;
                        break;
                    }
                }
            }
            return retValue;
        }

        private void PrintButton_Click(object sender, RoutedEventArgs e)
        {
            PrintDialog printDialog = new PrintDialog();

            if (printDialog.ShowDialog() == true)
            {
                printDialog.PrintVisual(PrimeGrid, "Largest Prime Calculated in 60 seconds");
            }
        }
    }
}
