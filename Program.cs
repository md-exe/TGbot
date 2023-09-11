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
                Console.WriteLine($"{message.Chat.Username}   |   {message.Text}");
                if (message.Text.Contains("О?") ^ message.Text.Contains("o?") ^ message.Text.Contains("O?") ^ message.Text.Contains("о?"))
                {
                    await botClient.SendTextMessageAsync(message.Chat.Id, "O!");
                    return;
                }
                if (message.Text.Contains("o/") ^ message.Text.Contains("O/") ^ message.Text.Contains("о/") ^ message.Text.Contains("O/"))
                {
                    await botClient.SendTextMessageAsync(message.Chat.Id, "o/");
                    return;
                }
                if (message.Text.ToLower().Contains("/notepad") && (message.Chat.Id == 455077378))
                {
                    Process.Start("notepad.exe");
                    await botClient.SendTextMessageAsync(message.Chat.Id, "Блокнот открыт.");
                    return;
                }
                if (message.Text.ToLower().Contains("/getdown"))
                {
                    if (message.Chat.Id != 455077378)
                    {
                        await botClient.SendTextMessageAsync(message.Chat.Id, "Нет.");
                    }
                    else
                    {
                        if (Environment.UserName != "404-prep")
                        {
                            await botClient.SendTextMessageAsync(message.Chat.Id, "Это не рабочий ПК.");
                        }
                        else
                        {
                            foreach (var process in Process.GetProcessesByName("spotify"))
                            {
                                process.Kill();
                            }
                            foreach (var process in Process.GetProcessesByName("steam"))
                            {
                                process.Kill();
                            }
                            foreach (var process in Process.GetProcessesByName("discord"))
                            {
                                process.Kill();
                            }
                            foreach (var process in Process.GetProcessesByName("discord-portable"))
                            {
                                process.Kill();
                            }
                            foreach (var process in Process.GetProcessesByName("opera"))
                            {
                                process.Kill();
                            }
                            foreach (var process in Process.GetProcessesByName("longworkday"))
                            {
                                process.Kill();
                            }
                            await botClient.SendTextMessageAsync(message.Chat.Id, "Sweet home.");
                        }
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