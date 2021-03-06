﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Xml.Serialization;

namespace druh_1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<Druh> druhowie = new List<Druh>();
        DispatcherTimer timer = new DispatcherTimer();
        DruhComparer comparer = new DruhComparer();
        bool edytowanie;
        Druh edytowany = new Druh();
        Style s;


        public MainWindow()
        {
            //Zegareczek
            timer.Tick += new EventHandler(timer_Click);
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Start();
            s = (Style)this.Resources["CalendarDayButtonStyle"];
            InitializeComponent();
            PomalujDni(druhowie);
            Clock();
            
            
            //dg.ItemsSource = druhowie;
            
        }

        private void UpdateControls()
        {
            lvCommingSoon.Items.Clear();
            comparer.SortBy = SortCriteria.DateThenType;
            druhowie.Sort(comparer);
            List<Druh> koniecKolejki = new List<Druh>();
            foreach (var item in druhowie)
            {
                if (item.DataUrodzin.DayOfYear >= DateTime.Today.DayOfYear)
                    lvCommingSoon.Items.Add(item);
                else
                    koniecKolejki.Add(item);
            }
            koniecKolejki.Sort(comparer);
            foreach (var item in koniecKolejki)
            {
                lvCommingSoon.Items.Add(item);
            }
            foreach (var item in druhowie)
            {
                if(item.DataUrodzin.DayOfYear >= DateTime.Today.DayOfYear)
                    tbSoonBd.Text = "Najbliższe urodziny obchodzi druh " + item.ToString2();
                break;
            }
           
            tbGreeting.Text = "Dziś jest " + DateTime.Now.ToString("dddd, dd MMMM yyyy");
        }

        private void timer_Click(object sender, EventArgs e)
        {
            Clock();
            UpdateControls();
        }

        private void Clock()
        {
            DateTime teraz = DateTime.Now;
            if (teraz.Minute < 10)
                tbClock.Text = teraz.Hour + ":0" + teraz.Minute;
            else
                tbClock.Text = teraz.Hour + ":" + teraz.Minute;
    
        }

        private void calendar_DisplayDateChanged(object sender, CalendarDateChangedEventArgs e)
        {
            //KONFIGURACJA WYŚWIETLANIA DRUGIEGO KALENDARZA
            if(calendar.DisplayDate.Month<12)
                calendar1.DisplayDate = new DateTime(calendar.DisplayDate.Year, calendar.DisplayDate.Month+1, 1);
            else
                calendar1.DisplayDate = new DateTime(calendar.DisplayDate.Year+1, 1, 1);
        }
        //konfiguracja kalendarzy
        private void calendar1_Loaded(object sender, RoutedEventArgs e)
        {
            calendar.BlackoutDates.Add(new CalendarDateRange(new DateTime(1990, 1, 1), DateTime.Now.AddDays(-1)));
            calendar1.BlackoutDates.Add(new CalendarDateRange(new DateTime(1990, 1, 1), DateTime.Now.AddDays(-1)));
            calendar.SelectedDate = DateTime.Today;
            calendar1.DisplayDateStart = new DateTime(DateTime.Today.Year, DateTime.Today.Month + 1, 1);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            save();
        }

        private void btAdd_Click(object sender, RoutedEventArgs e)
        {
            dpDataNowego.SelectedDate = calendar.SelectedDate;
            btCancelNewEvent.Visibility = Visibility.Visible;
            gAddMember.Visibility = Visibility.Visible;
        }

        private void btAcceptNewEvent_Click(object sender, RoutedEventArgs e)
        {
            if (edytowanie)
                druhowie.Remove(edytowany);
            Druh nowy = new Druh();
            nowy.Imie = tbImieNowego.Text;
            nowy.Nazwisko = tbNazwiskoNowego.Text;
            nowy.DataUrodzin = dpDataNowego.SelectedDate.Value;
            switch (cbStatusNowego.SelectedIndex)
            {
                case 0:
                    nowy.Status = StatusDruha.Zwykły1;
                    break;
                case 1:
                    nowy.Status = StatusDruha.Zwykły2;
                    break;
                case 2:
                    nowy.Status = StatusDruha.Młody;
                    break;
                case 3:
                    nowy.Status = StatusDruha.Zarząd;
                    break;
                case 4:
                    nowy.Status = StatusDruha.CzłonekHonorowy;
                    break;
            }
            druhowie.Add(nowy);
            save();
            tbImieNowego.Text = "";
            tbNazwiskoNowego.Text = "";
            dpDataNowego.SelectedDate = calendar.SelectedDate;
            cbStatusNowego.SelectedIndex = 0;
            gAddMember.Visibility = Visibility.Collapsed;
            UpdateControls();
        }

        private void btCancelNewEvent_Click(object sender, RoutedEventArgs e)
        {
            gAddMember.Visibility = Visibility.Collapsed;
            tbImieNowego.Text = "";
            tbNazwiskoNowego.Text = "";
            dpDataNowego.SelectedDate = calendar.SelectedDate;
            cbStatusNowego.SelectedIndex = 0;
            gAddMember.Visibility = Visibility.Collapsed;
        }

        private void save()
        {
            try
            {
                ReadWriteXml.savedata(druhowie, "supernazwa.xml");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btEdit_Click(object sender, RoutedEventArgs e)
        {
            
        }


        private void Window_Initialized(object sender, EventArgs e)
        {
            druhowie = (List<Druh>)ReadWriteXml.readdata("", "");
            List<Druh> koniecKolejki = new List<Druh>();
            foreach (var item in druhowie)
            {
                if (item.DataUrodzin.DayOfYear >= DateTime.Today.DayOfYear)
                    lvCommingSoon.Items.Add(item);
                else
                    koniecKolejki.Add(item);
            }
            koniecKolejki.Sort(comparer);
            foreach (var item in koniecKolejki)
            {
                lvCommingSoon.Items.Add(item);
            }
        }

        private void btRemove_Click(object sender, RoutedEventArgs e)
        {
            druhowie.Remove(edytowany);
            UpdateControls();
        }

        private void lvCommingSoon_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            edytowanie = true;
            edytowany = (Druh)lvCommingSoon.SelectedItem;
            tbImieNowego.Text = edytowany.Imie;
            tbNazwiskoNowego.Text = edytowany.Nazwisko;
            dpDataNowego.SelectedDate = edytowany.DataUrodzin;
            cbStatusNowego.SelectedIndex = (int)edytowany.Status;
            
            btCancelNewEvent.Visibility = Visibility.Hidden;
            gAddMember.Visibility = Visibility.Visible;
        }

        private void lvCommingSoon_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(lvCommingSoon.SelectedIndex>=0)
            edytowany = (Druh)lvCommingSoon.SelectedItem;
        }

        private void PomalujDni(List<Druh> wydarzenia)
        {
            Style s = (Style)this.Resources["CalendarDayButtonStyle"];

            foreach (Druh druh in druhowie)
            {
                DateTime dataTegoroczna;
                if (druh.DataUrodzin.DayOfYear < DateTime.Today.DayOfYear)
                {
                    dataTegoroczna = new DateTime(DateTime.Today.Year + 1, druh.DataUrodzin.Month, druh.DataUrodzin.Day);
                }
                else
                {
                    dataTegoroczna = new DateTime(DateTime.Today.Year, druh.DataUrodzin.Month, druh.DataUrodzin.Day);
                }

                if (druh.Status == StatusDruha.Zwykły1)
                {
                    DataTrigger dataTrigger = new DataTrigger() { Binding = new Binding("Date"), Value = dataTegoroczna };
                    dataTrigger.Setters.Add(new Setter(CalendarDayButton.BackgroundProperty, Brushes.Red));
                    s.Triggers.Add(dataTrigger);
                }

                else if (druh.Status == StatusDruha.Zwykły2)
                {
                    DataTrigger dataTrigger = new DataTrigger() { Binding = new Binding("Date"), Value = dataTegoroczna };
                    dataTrigger.Setters.Add(new Setter(CalendarDayButton.BackgroundProperty, Brushes.LawnGreen));
                    s.Triggers.Add(dataTrigger);
                }

                else if (druh.Status == StatusDruha.Młody)
                {
                    DataTrigger dataTrigger = new DataTrigger() { Binding = new Binding("Date"), Value = dataTegoroczna };
                    dataTrigger.Setters.Add(new Setter(CalendarDayButton.BackgroundProperty, Brushes.Gold));
                    s.Triggers.Add(dataTrigger);
                }

                else if (druh.Status == StatusDruha.Zarząd)
                {
                    DataTrigger dataTrigger = new DataTrigger() { Binding = new Binding("Date"), Value = dataTegoroczna };
                    dataTrigger.Setters.Add(new Setter(CalendarDayButton.BackgroundProperty, Brushes.HotPink));
                    s.Triggers.Add(dataTrigger);
                }

                else if (druh.Status == StatusDruha.CzłonekHonorowy)
                {
                    DataTrigger dataTrigger = new DataTrigger() { Binding = new Binding("Date"), Value = dataTegoroczna };
                    dataTrigger.Setters.Add(new Setter(CalendarDayButton.BackgroundProperty, Brushes.LightSkyBlue));
                    s.Triggers.Add(dataTrigger);
                }
            }
        }
    }
}
