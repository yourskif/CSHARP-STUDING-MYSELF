/*
 * Створи програму, яка моделює систему повідомлень.
    Є клас NotificationSystem, який генерує повідомлення.
    Клас MessageEventArgs містить текст повідомлення.
    Подія MessageReceived спрацьовує, коли нове повідомлення приходить.
    Має бути два обробники:
        EmailNotifier: цей обробник виводить повідомлення у консоль з текстом "Відправлено на email: <повідомлення>".
        SMSNotifier: цей обробник виводить повідомлення у консоль з текстом "Відправлено на SMS: <повідомлення>".
Підписуємо кожен з обробників на подію. Коли приходить нове повідомлення, обидва обробники мають бути викликані.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyMessageNotification
{
    public class NotificationSystem
    {
        public event EventHandler<MessageEventArgs> MessageReсeived;
        public string Message { get; set; }
        public void MessageArrived(string message, MessageType type)
        {
            MessageReсeived?.Invoke(this, new MessageEventArgs(message, type));
        }
    }

    public class MessageEventArgs : EventArgs
    {
        public string Message { get; set; }
        public MessageType Type { get; set; }
        public MessageEventArgs(string message, MessageType type)
        {
            Message = message;
            Type = type;
        }
    }

    public enum MessageType
    {
        Email,
        SMS
    }


    internal class Program
    {
        static void Main(string[] args)
        {
            var notification = new NotificationSystem();
            //notification.MessageReсeived += EmailNotifier;
            //notification.MessageReсeived += SMSNotifier;
            notification.MessageReсeived += MessageNotifier;

            // Отримуємо повідомлення з консолі
            Console.WriteLine("Введіть повідомлення:");
            var message = Console.ReadLine();

            Console.WriteLine("Введіть тип повідомлення (Email або SMS):");
            var typeInput = Console.ReadLine();

            // Конвертуємо в enum
            MessageType type = (MessageType)Enum.Parse(typeof(MessageType), typeInput, true);

            // Викликаємо метод, який викличе подію
            notification.MessageArrived(message, type);

        }

        static void MessageNotifier(object sender, MessageEventArgs e)
        {
            switch (e.Type)
            {
                case MessageType.Email:
                    Console.WriteLine($"Відправлено на email: {e.Message}");
                    break;
                case MessageType.SMS:
                    Console.WriteLine($"Відправлено на SMS: {e.Message}");
                    break;
            }
        }

        //static void EmailNotifier(object sender, MessageEventArgs e)
        //{
        //    if (e.Type == MessageType.Email)
        //        Console.WriteLine($"Відправлено на email:, {e.Message}");
        //}
        //static void SMSNotifier(object sender, MessageEventArgs e)
        //{
        //    if (e.Type == MessageType.Email)
        //        Console.WriteLine($"Відправлено на SMS:, {e.Message}");
        //}


    }
}