using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
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

namespace WpfClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string userName="vova";
        //private const string host = "127.0.0.1";
        private const string host = "91.238.103.51";
        private const int port = 8888;
        static TcpClient client;
        static NetworkStream stream;

        public MainWindow()
        {
            InitializeComponent();
            
            client = new TcpClient();
            try
            {
                client.Connect(host, port); //подключение клиента
                stream = client.GetStream(); // получаем поток

                string message = userName;
                byte[] data = Encoding.Unicode.GetBytes(message);
                stream.Write(data, 0, data.Length);

                // запускаем новый поток для получения данных
                Thread receiveThread = new Thread(new ThreadStart(ReceiveMessage));
                receiveThread.Start(); //старт потока
                //Console.WriteLine("Добро пожаловать, {0}", userName);
                //SendMessage();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            //finally
            //{
            //    Disconnect();
            //}

        }

        private void btnSendMesage_Click(object sender, RoutedEventArgs e)
        {
            var message = txtMessage.Text;
            //MessageBox.Show(message);
            SendMessage(message);
        }

        // отправка сообщений
        void SendMessage(string message)
        {
            //Console.WriteLine("Введите сообщение: ");

            //while (true)
            //{
                //string message = Console.ReadLine();
                byte[] data = Encoding.Unicode.GetBytes(message);
                stream.Write(data, 0, data.Length);
            //}
        }
        // получение сообщений
        void ReceiveMessage()
        {
            while (true)
            {
                try
                {
                    byte[] data = new byte[64]; // буфер для получаемых данных
                    StringBuilder builder = new StringBuilder();
                    int bytes = 0;
                    do
                    {
                        bytes = stream.Read(data, 0, data.Length);
                        builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                    }
                    while (stream.DataAvailable);

                    string message = builder.ToString();
                    Console.WriteLine(message);//вывод сообщения
                }
                catch
                {
                    Console.WriteLine("Подключение прервано!"); //соединение было прервано
                    Console.ReadLine();
                    Disconnect();
                }
            }
        }

        void Disconnect()
        {
            if (stream != null)
                stream.Close();//отключение потока
            if (client != null)
                client.Close();//отключение клиента
            Environment.Exit(0); //завершение процесса
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Disconnect();
        }
    }
}
