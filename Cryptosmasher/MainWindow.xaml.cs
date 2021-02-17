
using Cryptosmasher.Apis.Binance;
using Cryptosmasher.Apis.Poloniex;
using Cryptosmasher.Common;
using Cryptosmasher.Extentions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

namespace Cryptosmasher
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyCollectionChanged, INotifyPropertyChanged
    {

        #region private variables

        private readonly BackgroundWorker _binanceWorker = new BackgroundWorker();
        private readonly BackgroundWorker _poloniexWorker = new BackgroundWorker();
        
        private List<TradePair> _tickers;
        private IApi _binanceApi;
        private IApi _poloniexApi;

        private TradePair _selected4Hours;
        private TradePair _selectedDaily;

        #endregion

        #region public properties

        public ObservableCollection<TradePair> Tickers4H { get { return new ObservableCollection<TradePair>(_tickers.OrderByDescending(x=>x.Color_4h).ThenByDescending(x=>x.HighestInXAmountOfCandles_4h)); } set => _tickers = value.ToList();
        }
        public ObservableCollection<TradePair> TickersDaily { get { return new ObservableCollection<TradePair>(_tickers.OrderByDescending(x=>x.Color_Daily).ThenByDescending(x => x.HighestInXAmountOfCandles_Daily)); } set => _tickers = value.ToList();
        }
        public TradePair Selected4Hours { get => _selected4Hours;
            set { _selected4Hours = value; OnPropertyChanged(); } }
        public TradePair SelectedDaily { get => _selectedDaily;
            set { _selectedDaily = value; OnPropertyChanged(); } }
       
        #endregion

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            InitGui();
        }

        private void InitGui()

        {
            _tickers = new List<TradePair>();
            _binanceWorker.DoWork += WorkerCheckBinance ;
            _poloniexWorker.DoWork += WorkerCheckPoloniex;
            _binanceApi = new BinanceApi();
            _poloniexApi = new PoloniexApi();
            Title = "Cryptosmasher";
            ShowEula();            
        }

        private void ShowEula()
        {
            var eula = new EULA();
            eula.ShowDialog();

            if (eula.DialogResult.HasValue && eula.DialogResult.Value)
            {
                _binanceWorker.RunWorkerAsync();
                _poloniexWorker.RunWorkerAsync();
            }
            else
            {
                Close();
            }
        }
        
        private void WorkerCheckPoloniex(object sender, DoWorkEventArgs e)
        {
            while (true)
            {
                UpdateTickers(_poloniexApi);
               
                Thread.Sleep(1000 * 60 * 15);
            }
        }

        private void WorkerCheckBinance(object sender,DoWorkEventArgs e)
        {
            while (true)
            {
                UpdateTickers(_binanceApi);

                Thread.Sleep(1000 * 60 * 15);
            }
        }

        public string Filters { get; set; } = "ETH;BTC";

        private void UpdateTickers(IApi api)
        {
            var filters = Filters.Split(';');
            var abc = api.GetTradePairs();
            var tradePairs = abc.Where(tp=>filters.Any(f=> tp.Id.ToLower().Contains(f.ToLower()))).ToList();
            
            for (var i = 0; i < tradePairs.Count(); i++)
            {                
                var ticker = api.GetTickersWithCandlesticks(tradePairs.ElementAt(i));

                var existingTicker = _tickers.FirstOrDefault(x => x.Id == ticker.Id && ticker.Exchange == x.Exchange);
                if (existingTicker == null)
                {
                    _tickers.Add((TradePair)ticker);
                }
                else
                {
                    existingTicker.CandleSticks_4h = ticker.CandleSticks_4h;
                    existingTicker.CandleSticks_4h = ticker.CandleSticks_Daily;
                    existingTicker.LastUpdated = ticker.LastUpdated;
                }
                ticker.GetCCICrossToPositive();
                ticker.SetLastHighest();
                ticker.LastUpdated = DateTime.Now;
                OnPropertyChanged(nameof(Tickers4H));
                OnPropertyChanged(nameof(TickersDaily));
            }
        }

        #region onpropertyChangedStuff

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        protected void OnNotifyCollectionChanged(NotifyCollectionChangedEventArgs args)
        {
            this.CollectionChanged?.Invoke(this, args);
        }

        #endregion


        #region events

        private void Hours4List_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!(hours4List.SelectedItem is TradePair selectedItem)) return;

            Selected4Hours = selectedItem;
            var daily = new List<TradePair>();
            daily.AddRange(dailyList.Items.SourceCollection.OfType<TradePair>());

            var dailyTicker = daily.FirstOrDefault(x => x.Id == selectedItem.Id);

            if (dailyTicker != null)
            {
                dailyList.SelectedItem = dailyTicker;
            }
        }

        private void DailyList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!(dailyList.SelectedItem is TradePair selectedItem)) return;

            SelectedDaily = selectedItem;
            var hours4Tickers = new List<TradePair>();
            hours4Tickers.AddRange(hours4List.Items.SourceCollection.OfType<TradePair>());
            
            var hours4Ticker = hours4Tickers.FirstOrDefault(x => x.Id == selectedItem.Id);

            if (hours4Ticker != null)
            {
                hours4List.SelectedItem = hours4Ticker;
            }
        }

        private void TickerButton_Click(object sender, RoutedEventArgs e)
        {
            if (selected_4h == null) return;

            var url = "";
            switch (_selected4Hours.Exchange)
            {
                case CryptoExchange.Binance:

                    url= "https://www.binance.com/tradeDetail.html?symbol="+ (_selected4Hours.Id.EndsWith("USDT") ? _selected4Hours.Id.Insert(_selected4Hours.Id.Length-4,"_"): _selected4Hours.Id.Insert(_selected4Hours.Id.Length - 3, "_"));                        
                    break;
                case CryptoExchange.Poloniex:
                    url = "https://poloniex.com/exchange#" + _selected4Hours.Id;
                    break;
                default:
                    break;
            }

            Process.Start(url);
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            var aboutDialog = new About();
            aboutDialog.ShowDialog();
        }
        
        #endregion

    }
}
