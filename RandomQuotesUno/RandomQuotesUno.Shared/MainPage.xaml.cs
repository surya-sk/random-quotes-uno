using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace RandomQuotesUno
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        Quotes quotes = new Quotes();
        HttpClient client;
        DispatcherTimer dispatcherTimer;
        string url = "https://yourrandomquotes.herokuapp.com/quote";//tp://localhost:8081/quote";
        public MainPage()
        {
            this.InitializeComponent();
            client = new HttpClient();
            client.BaseAddress = new Uri(url);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
            LoadQuote();
            DispatcherTimerSetup();
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            
        }




        /// <summary>
        /// Returns the json string from the url
        /// </summary>
        /// <returns>the string in JSON format</returns>
        public async Task<Quotes> GetStringFromURL()
        {
            System.Net.Http.HttpResponseMessage result = await client.GetAsync(url).ConfigureAwait(false);
            if (result.IsSuccessStatusCode)
            {
                string jsonString = await result.Content.ReadAsStringAsync().ConfigureAwait(false);
                quotes = JsonConvert.DeserializeObject<Quotes>(jsonString);
            }
            return quotes;
        }

        async Task<Quotes> GetResponse()
        {

            quotes = await GetStringFromURL().ConfigureAwait(false);

            return quotes;
        }

        /// <summary>
        /// A class that helps in parsing the JSON strings to simplify access in other methods
        /// </summary>
        public class Quotes : INotifyPropertyChanged
        {
            private string quote;
            private string by;

            public string By
            {
                get => by; set
                {
                    by = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(By)));
                }
            }
            public string Quote
            {
                get => quote; set
                {
                    quote = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Quote)));
                }
            }

            public event PropertyChangedEventHandler PropertyChanged;
        }

        /// <summary>
        /// Displays a new quote
        /// </summary>
        private void LoadQuote()
        {
            quotes = GetResponse().Result;
            DataContext = new Quotes() { By = quotes.By, Quote = quotes.Quote };
            if(quotes.Quote.Length < 300)
            {
                Quote.TextWrapping = TextWrapping.Wrap;
            }
        }

        /// <summary>
        /// Start a timer to display a new quote using intervals
        /// </summary>
        private void DispatcherTimerSetup()
        {
            dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += dispatcher_Tick;
            dispatcherTimer.Interval = new TimeSpan(0, 0, 60);
            dispatcherTimer.Start();

        }

        private void dispatcher_Tick(object sender, object e)
        {
            LoadQuote();
        }

    }
}
