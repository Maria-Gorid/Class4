using System.Text;

namespace Task2
{
    class Program
    {
        static void Main()
        {
            Console.Write("Введите путь к файлу: ");
            var path = Console.ReadLine();

            Console.Write("Введите исходную кодировку: ");
            var fromEncodingName = Console.ReadLine();

            Console.Write("Введите конечную кодировку: ");
            var toEncodingName = Console.ReadLine();

            if (!File.Exists(path))
            {
                Console.WriteLine("Файл не существует.");
                return;
            }

            Encoding fromEncoding;
            try
            {
                fromEncoding = Encoding.GetEncoding(fromEncodingName);
            }
            catch (ArgumentException)
            {
                Console.WriteLine($"Кодировка {fromEncodingName} не поддерживается.");
                return;
            }

            Encoding toEncoding;
            try
            {
                toEncoding = Encoding.GetEncoding(toEncodingName);
            }
            catch (ArgumentException)
            {
                Console.WriteLine($"Кодировка {toEncodingName} не поддерживается.");
                return;
            }

            try
            {
                var text = File.ReadAllText(path, fromEncoding);
                File.WriteAllText(path, text, toEncoding);
                Console.WriteLine("Файл перекодирован успешно.");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Ошибка при чтении/записи файла: {e.Message}");
            }
        }
    }
}


