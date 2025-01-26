using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace ClientApp
{
    class Program
    {
        private static HttpClient httpClient = new HttpClient();
        private static string baseUrl = "http://localhost:5062"; // URL вашего сервера
        private static string historyFolder = "history"; // Папка для хранения истории вычислений

        static async Task Main(string[] args)
        {
            await RunApp();
        }

        static async Task RunApp()
        {
            Console.WriteLine("Добро пожаловать!");
            Console.WriteLine("Нажмите любую клавишу для продолжения...");
            Console.ReadKey();

            // Создаем папку для истории, если она не существует
            if (!Directory.Exists(historyFolder))
            {
                Directory.CreateDirectory(historyFolder);
                Console.WriteLine($"Создана папка для истории: {historyFolder}");
            }

            while (true)
            {
                Console.Clear();
                Console.WriteLine("1. Войти");
                Console.WriteLine("2. Зарегистрироваться");
                Console.Write("Выберите действие: ");
                var choice = Console.ReadLine();

                if (choice == "1")
                {
                    await Login();
                }
                else if (choice == "2")
                {
                    await SignUp();
                }
                else
                {
                    Console.WriteLine("Неверный выбор. Попробуйте снова.");
                }
            }
        }

        // Регистрация
        private static async Task SignUp()
        {
            Console.Write("Введите логин: ");
            var login = Console.ReadLine();
            Console.Write("Введите пароль: ");
            var password = Console.ReadLine();

            var signUpRequest = new SignUpRequest { Login = login, Password = password };
            var response = await httpClient.PostAsJsonAsync($"{baseUrl}/auth/signup", signUpRequest);

            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("Регистрация прошла успешно!");
                Console.ReadKey();
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Ошибка регистрации: {errorContent}");
            }
        }

        // Вход
        private static async Task Login()
        {
            Console.Write("Введите логин: ");
            var login = Console.ReadLine();
            Console.Write("Введите пароль: ");
            var password = Console.ReadLine();

            var loginRequest = new LoginRequest { Login = login, Password = password };
            var response = await httpClient.PostAsJsonAsync($"{baseUrl}/auth/login", loginRequest);

            if (response.IsSuccessStatusCode)
            {
                var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponse>();
                Console.WriteLine("Вход выполнен успешно!");
                Console.WriteLine($"Token: {loginResponse.Token}");
                await ShowUserMenu(loginResponse.Token, login); // Передаем логин в меню
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Ошибка входа: {errorContent}");
            }
        }

        // Меню пользователя
        private static async Task ShowUserMenu(string token, string login)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("\nМеню:");
                Console.WriteLine("1. Решить Решето Эратосфена");
                Console.WriteLine("2. Посмотреть историю вычислений");
                Console.WriteLine("3. Удалить историю вычислений");
                Console.WriteLine("4. Удалить пользователя");
                Console.WriteLine("5. Изменить пароль");
                Console.WriteLine("6. Выйти");
                Console.Write("Выберите действие: ");
                var choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        await SolveSieve(token);
                        Console.WriteLine("Нажмите любую клавишу для продолжения...");
                        Console.ReadKey(); // Пауза после вычислений
                        break;
                    case "2":
                        ViewHistory();
                        Console.WriteLine("Нажмите любую клавишу для продолжения...");
                        Console.ReadKey(); // Пауза после вывода истории
                        break;
                    case "3":
                        DeleteHistory();
                        Console.WriteLine("Нажмите любую клавишу для продолжения...");
                        Console.ReadKey(); // Пауза после удаления истории
                        break;
                    case "4":
                        await DeleteUser(token, login);
                        Console.WriteLine("Нажмите любую клавишу для продолжения...");
                        Console.ReadKey();
                        return; // Выход из меню после удаления пользователя
                    case "5":
                        await ChangePassword(token, login); // Передаем логин
                        Console.WriteLine("Нажмите любую клавишу для продолжения...");
                        Console.ReadKey(); // Пауза после изменения пароля
                        break;
                    case "6":
                        return; // Выход из меню
                    default:
                        Console.WriteLine("Неверный выбор. Попробуйте снова.");
                        break;
                }
            }
        }

        // Решение Решета Эратосфена
        private static async Task SolveSieve(string token)
        {
            Console.Write("Введите границу для Решета Эратосфена: ");
            if (int.TryParse(Console.ReadLine(), out int border))
            {
                var sieveRequest = new Border { border = border };
                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                try
                {
                    var response = await httpClient.PostAsJsonAsync($"{baseUrl}/sieve/sieve", sieveRequest);

                    if (response.IsSuccessStatusCode)
                    {
                        var responseContent = await response.Content.ReadAsStringAsync();
                        Console.WriteLine($"Ответ сервера: {responseContent}");

                        var options = new JsonSerializerOptions
                        {
                            PropertyNamingPolicy = JsonNamingPolicy.CamelCase // Учитываем camelCase
                        };

                        var jsonResponse = JsonSerializer.Deserialize<JsonElement>(responseContent, options);

                        // Проверяем, есть ли свойство "primes" в JSON-ответе
                        if (jsonResponse.TryGetProperty("primes", out var primesProperty))
                        {
                            var primes = primesProperty.GetString();
                            Console.WriteLine("Простые числа: " + primes);

                            // Сохраняем результат в историю
                            SaveHistory(primes);
                        }
                        else if (jsonResponse.TryGetProperty("message", out var messageProperty))
                        {
                            // Если сервер вернул сообщение об ошибке
                            var errorMessage = messageProperty.GetString();
                            Console.WriteLine($"Ошибка сервера: {errorMessage}");
                        }
                        else
                        {
                            Console.WriteLine("Ошибка: не удалось обработать ответ сервера.");
                        }
                    }
                    else
                    {
                        var errorContent = await response.Content.ReadAsStringAsync();
                        Console.WriteLine($"Ошибка: {response.StatusCode} - {errorContent}");
                    }
                }
                catch (HttpRequestException ex)
                {
                    Console.WriteLine($"Ошибка при отправке запроса: {ex.Message}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Неизвестная ошибка: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine("Неверный ввод. Пожалуйста, введите целое число.");
            }
        }

        // Сохранение истории вычислений
        private static void SaveHistory(string result)
        {
            var historyFile = Path.Combine(historyFolder, "history.txt");
            File.AppendAllText(historyFile, $"{DateTime.Now}: {result}\n");
            Console.WriteLine("Результат сохранен в истории.");
        }

        // Просмотр истории вычислений
        private static void ViewHistory()
        {
            var historyFile = Path.Combine(historyFolder, "history.txt");
            if (File.Exists(historyFile))
            {
                Console.WriteLine("История вычислений:");
                Console.WriteLine(File.ReadAllText(historyFile));
            }
            else
            {
                Console.WriteLine("История вычислений пуста.");
            }
        }

        // Удаление истории вычислений
        private static void DeleteHistory()
        {
            var historyFile = Path.Combine(historyFolder, "history.txt");
            if (File.Exists(historyFile))
            {
                File.Delete(historyFile);
                Console.WriteLine("История вычислений удалена.");
            }
            else
            {
                Console.WriteLine("История вычислений уже пуста.");
            }
        }

        // Удаление пользователя
        private static async Task DeleteUser(string token, string login)
        {
            var deleteRequest = new DeleteUserRequest { Login = login };
            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var response = await httpClient.PostAsJsonAsync($"{baseUrl}/auth/deleteuser", deleteRequest);

            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("Пользователь удален успешно.");
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Ошибка: {error}");
            }
        }

        // Изменение пароля
        private static async Task ChangePassword(string token, string login)
        {
            Console.Write("Введите старый пароль: ");
            var oldPassword = Console.ReadLine();
            Console.Write("Введите новый пароль: ");
            var newPassword = Console.ReadLine();

            var changePasswordRequest = new ChangePasswordRequest
            {
                Login = login, // Добавляем логин
                OldPassword = oldPassword,
                NewPassword = newPassword
            };

            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            try
            {
                var response = await httpClient.PostAsJsonAsync($"{baseUrl}/auth/changepassword", changePasswordRequest);

                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("Пароль изменен успешно.");
                    Console.WriteLine("Сессия завершена. Войдите с новым паролем.");
                    return; // Завершаем сессию
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Ошибка: {error}");
                    Console.WriteLine("Сессия продолжена. Пароль не изменен.");
                }
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Ошибка при отправке запроса: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Неизвестная ошибка: {ex.Message}");
            }
        }
    }

    // Модели запросов и ответов
    public class SignUpRequest
    {
        public string Login { get; set; }
        public string Password { get; set; }
    }

    public class LoginRequest
    {
        public string Login { get; set; }
        public string Password { get; set; }
    }

    public class LoginResponse
    {
        public string Token { get; set; }
    }

    public class DeleteUserRequest
    {
        public string Login { get; set; }
    }

    public class ChangePasswordRequest
    {
        public string Login { get; set; }
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
    }

    public class Border
    {
        public int border { get; set; }
    }
}