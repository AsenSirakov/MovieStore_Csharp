using KafkaProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KafkaProject
{
    public static class ConsoleHelper
    {
        public static void WriteCoordinates(int x, int y,string text)
            {
            Console.SetCursorPosition(x, y); 
            Console.Write(text);

         
            }

        public static void WriteMsgFromUser(ChatMessage msg)
        {
            Console.Write($"[{DateTime.Now.TimeOfDay}] [{msg.User}] {msg.Message}");

        }

        public static void WriteLocalUser()
        {
            Console.Write($"[{AppSetting.LocalUsername}]");

        }
    }
}
