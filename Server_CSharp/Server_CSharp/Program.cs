using System;

namespace Server_CSharp
{
    class Program
    {
        static void Main(string[] args)
        {
            UDPSocket s = new UDPSocket();
            s.Server("147.96.119.235", 54000);
            Console.WriteLine("Servidor creado en puerto 54000 y con la ip 147.96.119.235");



            Console.ReadKey();
        }
    }
}
