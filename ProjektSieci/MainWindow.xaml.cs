using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
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

namespace ProjektSieci
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        int IloscPowtorzen;
        public MainWindow()
        {
            InitializeComponent();
            cbxPowtorz.Items.Add("1");
            cbxPowtorz.Items.Add("2");
            cbxPowtorz.Items.Add("3");
            cbxPowtorz.Items.Add("4");
            cbxPowtorz.SelectedItem = "1";
            IloscPowtorzen = 1;
        }


        private string WyslijPing(string adres, int timeout, byte[] bufor, PingOptions opcje)
        {
            Ping ping = new Ping();
            try
            {
                PingReply odpowiedz = ping.Send(adres, timeout, bufor, opcje);
                if (odpowiedz.Status == IPStatus.Success)
                    return "Odpowiedź z " + adres + " bajtów=" +
                    odpowiedz.Buffer.Length + " czas=" + odpowiedz.RoundtripTime +
                    "ms TTL=" + odpowiedz.Options.Ttl;
                else
                    return "Błąd:" + adres + " " + odpowiedz.Status.ToString();
            }
            catch (Exception ex)
            {
                return "Błąd:" + adres + " " + ex.Message;
            }
        }

        private void btnPing_Click(object sender, RoutedEventArgs e)
        {
            if (textBox1.Text != "" || listBox2.Items.Count > 0)
            {
                PingOptions opcje = new PingOptions();
                opcje.Ttl = 128;
                opcje.DontFragment = true;
                string dane = "aaaaaaaaaaaaaaaaaaaaaaa";
                byte[] bufor = Encoding.ASCII.GetBytes(dane);
                int timeout = 120;
                if (textBox1.Text != "")
                {
                    for (int i = 0; i < IloscPowtorzen; i++)
                        listBox1.Items.Add(this.WyslijPing(textBox1.Text, timeout, bufor, opcje));
                    listBox1.Items.Add("--------------------");
                }
                if (listBox2.Items.Count > 0)
                {
                    foreach (string host in listBox2.Items)
                    {
                        for (int i = 0; i < IloscPowtorzen; i++)
                        {
                            listBox1.Items.Add(this.WyslijPing(host, timeout, bufor, opcje));
                        }
                        listBox1.Items.Add("--------------------");
                    }
                }
            }
            else
            {
                MessageBox.Show("Nie wprowadzono żadnych adresów", "Błąd");
            }
        }

        private void btnDodaj_Click(object sender, RoutedEventArgs e)
        {
            if (textBox2.Text != String.Empty)
                if (textBox2.Text.Trim().Length > 0)
                {
                    listBox2.Items.Add(textBox2.Text);
                    textBox1.Clear();
                }
        }

        private void DisplayIPAddresses()
        {
            string NazwaHosta = Dns.GetHostName();
            IPHostEntry AdresyIP = Dns.GetHostEntry(NazwaHosta);

            int licznik = 0;
            foreach (IPAddress AdresIP in AdresyIP.AddressList)
            {
                if (AdresIP.ToString() == "127.0.0.1")
                {
                    listBox1.Items.Add("Komputer nie jest podłączony do sieci. Adres IP: " + AdresIP.ToString());
                }
                else
                {
                    listBox1.Items.Add("Adres IP #" + ++licznik + ": " + AdresIP.ToString());
                }
            }
        }

        private void btnIPCheck_Click(object sender, RoutedEventArgs e)
        {
            DisplayIPAddresses();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            listBox1.Items.Clear();
        }

        private void btnUsun_Click(object sender, RoutedEventArgs e)
        {
            if(listBox2.SelectedItem != null)
            {
                listBox2.Items.Remove(listBox2.SelectedItem);
            }
        }

        private void cbxPowtorz_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            IloscPowtorzen = Convert.ToInt32(cbxPowtorz.SelectedItem);
        }

    private void DisplayNetworkInfo()
    {
        IPGlobalProperties wlasnosciIP = IPGlobalProperties.GetIPGlobalProperties();
        listBox1.Items.Add("Nazwa hosta: " + wlasnosciIP.HostName);
        listBox1.Items.Add("Nazwa domeny: " + wlasnosciIP.DomainName);
        listBox1.Items.Add("");

        int licznik = 0;
        foreach (NetworkInterface kartySieciowe in NetworkInterface.GetAllNetworkInterfaces())
        {
            listBox1.Items.Add("Karta #" + ++licznik + ": " + kartySieciowe.Id);
            listBox1.Items.Add("  Adres MAC: " + kartySieciowe.GetPhysicalAddress().ToString());
            listBox1.Items.Add("  Nazwa: " + kartySieciowe.Name);
            listBox1.Items.Add("  Opis: " + kartySieciowe.Description);
            listBox1.Items.Add("  Status: " + kartySieciowe.OperationalStatus);
            listBox1.Items.Add("  Szybkość: " + (kartySieciowe.Speed) / (double)1000000 + " Mb/s");
            listBox1.Items.Add("  Adresy bram sieciowych:");
            foreach (GatewayIPAddressInformation adresBramy in kartySieciowe.GetIPProperties().GatewayAddresses)
                listBox1.Items.Add("    " + adresBramy.Address.ToString());
            listBox1.Items.Add("  Serwery DNS:");
            foreach (IPAddress adresIP in kartySieciowe.GetIPProperties().DnsAddresses)
                listBox1.Items.Add("    " + adresIP.ToString());
            listBox1.Items.Add("  Serwery DHCP:");
            foreach (IPAddress adresIP in kartySieciowe.GetIPProperties().DhcpServerAddresses)
                listBox1.Items.Add("    " + adresIP.ToString());
            listBox1.Items.Add("  Serwery WINS:");
            foreach (IPAddress adresIP in kartySieciowe.GetIPProperties().WinsServersAddresses)
                listBox1.Items.Add("   " + adresIP.ToString());

            listBox1.Items.Add("");
        }

        listBox1.Items.Add("  Aktualne połączenia TCP/IP typu klient:");
        foreach (TcpConnectionInformation polaczenieTCP in IPGlobalProperties.GetIPGlobalProperties().GetActiveTcpConnections())
        {
            listBox1.Items.Add("    Zdalny adres: " + polaczenieTCP.RemoteEndPoint.Address.ToString() + ":" + polaczenieTCP.RemoteEndPoint.Port);
            listBox1.Items.Add("    Status: " + polaczenieTCP.State.ToString());
        }
        listBox1.Items.Add("  Aktualne połączenia TCP/IP typu serwer:");
        foreach (IPEndPoint polaczenieTCP in IPGlobalProperties.GetIPGlobalProperties().GetActiveTcpListeners())
            listBox1.Items.Add("    Zdalny adres: " + polaczenieTCP.Address.ToString() + ":" + polaczenieTCP.Port);
        listBox1.Items.Add("  Aktualne połączenia UDP:");
        foreach (IPEndPoint polaczenieUDP in IPGlobalProperties.GetIPGlobalProperties().GetActiveUdpListeners())
            listBox1.Items.Add("    Zdalny adres" + polaczenieUDP.Address.ToString() + ":" + polaczenieUDP.Port);
    }

        private void btnInfo_Click(object sender, RoutedEventArgs e)
        {
            DisplayNetworkInfo();
        }
    }
   
}
