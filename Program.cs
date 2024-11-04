using System;
using System.Net;
using System.Net.Sockets;
using System.Text;


namespace _24_10_UDP_MulticastGroup_Server
{
    internal class Program
    {
        private static List<string> elektronicList = new List<string>();
        private static List<string> clousesList = new List<string>();
        private static List<string> productsList = new List<string>();
        private  static char[] symb = { ' ', ',', '.', ';', ':', '-', '!', '?' };

        private static IPAddress brodcastAddress_elektronic = IPAddress.Parse("239.0.0.1");
        private static IPAddress brodcastAddress_clouses = IPAddress.Parse("239.0.0.2");
        private static IPAddress brodcastAddress_products = IPAddress.Parse("239.0.0.3");

        private static int port_elektronic = 8001;
        private static int port_clouses = 8002;
        private static int port_products = 8003;

        static async Task Main(string[] args)
        {
            while (true)
            {
               
                await Menu();
                Console.Clear();
                // Console.ReadKey();
            }
        }
        private static async Task Menu()
        {
            Console.WriteLine("Выберите:");
            Console.WriteLine("1 - добавить новый товар");
            Console.WriteLine("0 - выход");
            int input;
            while (!Int32.TryParse(Console.ReadLine(), out input) || input < 0 || input > 1)
            {
                Console.WriteLine("Не верный ввод.Введите число 0 ии 1:");
                Console.Write("категория - ");
            }
            switch (input)
            {
                case 0: break;
                case 1: 
                    GetNewProductsName();
                    break;
            }
        }

       
        private static async Task GetNewProductsName()
        {
            Console.WriteLine("Выберите категорию товара:");
            Console.WriteLine("1 -Электроника ");
            Console.WriteLine("2 - Одежда");
            Console.WriteLine("3 - Продукты");

            int inputCategory;
            while (!Int32.TryParse(Console.ReadLine(), out inputCategory) || inputCategory < 0 || inputCategory > 3)
            {
                Console.WriteLine("Не верный ввод.Введите число от 1 до 3:");
                Console.Write("категория - ");
            }


            Console.WriteLine("укажите список названий новых товаров из выбранной категории,который появился в магазине: ");
            var listName = Console.ReadLine().Split(symb, StringSplitOptions.RemoveEmptyEntries).ToList();

            switch (inputCategory)
            {
                case 1:
                    
                    elektronicList.AddRange(listName);
                    break;
                case 2:
                   
                    clousesList.AddRange(listName);
                    break;
                case 3:
                    
                    productsList.AddRange(listName);
                    break;

            }

            Console.WriteLine("Выбрать еще категорию товара? (Y/N)");

            string flag = Console.ReadLine().ToUpper();
            while (flag != "Y" && flag != "N")
            {
                Console.WriteLine("Не верный ввод.Выбрать еще категорию товара? (Y/N)");
                flag = Console.ReadLine().ToUpper();
            }
            if (flag == "Y")
            {
                await GetNewProductsName();
            }
            else
            {
                Console.WriteLine("Выполнить рассылку? (Y / N)");

                flag = Console.ReadLine().ToUpper();
                while (flag != "Y" && flag != "N")
                {
                    Console.WriteLine("Не верный ввод.Выполнить рассылку? (Y/N)");
                    flag = Console.ReadLine().ToUpper();
                }
                if (flag == "Y")
                {
                    var tasks = new List<Task>();
                    if (elektronicList.Any())
                    {
                        tasks.Add(Task.Run(() => SendMsg(brodcastAddress_elektronic, port_elektronic, elektronicList)));
                    }
                    if (clousesList.Any())
                    {
                        tasks.Add(Task.Run(() => SendMsg(brodcastAddress_clouses, port_clouses, clousesList)));
                    }
                    if (productsList.Any())
                    {
                        tasks.Add(Task.Run(() => SendMsg( brodcastAddress_products, port_products, productsList)));
                    }

                    if (tasks.Any())
                    {
                        try
                        {
                            await Task.WhenAll(tasks);
                        }
                        catch (Exception e) { }
                    }
                    elektronicList.Clear();
                    clousesList.Clear();
                    productsList.Clear();
                    await Menu();
                }

               

            }
        }
        private static async Task SendMsg(IPAddress broadcastAddress, int port, List<string> messages)
        {
            using var udpSender = new UdpClient();
            Console.WriteLine("Начало отправки сообщений...");


            foreach (var message in messages)
            {
                byte[] data = Encoding.UTF8.GetBytes(message);
                var endPoint = new IPEndPoint(broadcastAddress, port);
                await udpSender.SendAsync(data, data.Length, endPoint);
                Console.WriteLine($"Отправлено сообщение: {message}");
                await Task.Delay(100);
            }


        }



    }
}
