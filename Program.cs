using System.Diagnostics;
using System.Numerics;
namespace PrimeNumber
{
    public class PrimeChecker
    {
        public static bool IsPrimeBruteForce(BigInteger num)
        {
            if (num <= 1) return false;
            for (int i = 2; i <= Math.Sqrt((double)num); i++)
            {
                if (num % i == 0) return false;
            }
            return true;
        }
        /**
         * Проверяет, является ли число простым с помощью теста Миллера-Рабина.
         * @param {int} n - Проверяемое число.
         * @param {int} k - Количество итераций теста (чем больше, тем точнее).
         * @returns {bool} - true, если число, вероятно, простое; false, если составное.
         */
        public static bool IsPrimeMiller(BigInteger n, int k)
        {
            if (n <= 1) return false;
            if (n <= 3) return true;

            // Находим s и q такие, что n - 1 = 2^s * q
            int s = 0;
            BigInteger q = n - 1;
            while (q % 2 == 0)
            {
                s++;
                q /= 2;
            }

            // Выполняем k итераций теста
            Random random = new Random();
            for (int i = 0; i < k; i++)
            {
                int bytesCount = 8; // Для BigInteger используется 8 байт

                // Создание массива байт и заполнение его случайными значениями
                BigInteger a = GenerateRandomBigInteger(2, n - 1, random); // Случайное a: 2 <= a <= n-2
                BigInteger x = ModPow(a, q, n);

                if (x == 1 || x == n - 1) continue;

                for (int j = 1; j < s; j++)
                {
                    x = (x * x) % n;
                    if (x == 1) return false;
                    if (x == n - 1) break;
                }

                if (x != n - 1) return false;
            }

            return true;
        }

        public static double MeasureExecutionTime(Func<bool> func, params object[] args)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            func.Invoke();
            stopwatch.Stop();
            return stopwatch.Elapsed.TotalMilliseconds;
        }

        // Метод для генерации случайного числа типа BigInteger в заданном диапазоне
        private static BigInteger GenerateRandomBigInteger(BigInteger minValue, BigInteger maxValue, Random random)
        {
            // Вычисляем длину диапазона
            BigInteger range = maxValue - minValue;

            // Генерируем случайное число, используя длину диапазона в качестве максимального значения
            BigInteger randomNumber = 0;
            do
            {
                // Генерируем случайное число в байтах
                byte[] buf = new byte[range.ToByteArray().Length];
                random.NextBytes(buf);
                randomNumber = new BigInteger(buf);

                // Если сгенерированное число находится в диапазоне, прерываем цикл
                if (randomNumber >= minValue && randomNumber <= maxValue)
                    break;

                // Если сгенерированное число выходит за пределы диапазона, пробуем снова
            } while (true);

            return randomNumber;
        }
        // Вспомогательная функция возведения в степень по модулю
        private static BigInteger ModPow(BigInteger x, BigInteger y, BigInteger p)
        {
            BigInteger res = 1; // Инициализируем результат

            x = x % p; // Предотвращаем переполнение

            while (y > 0)
            {
                // Если y нечетное, умножаем результат на x по модулю
                if (y % 2 == 1)
                    res = (res * x) % p;

                // Уменьшаем y на половину, x умножаем на себя по модулю
                y = y >> 1; // Эквивалентно делению на 2
                x = (x * x) % p;
            }

            return res;
        }
    }
    internal class Program
    {
       
        static void Main(string[] args)
        {

            BigInteger number = 3632514097; // Число для проверки
            const int iterations = 10; // Количество итераций теста

            bool isProbablyPrimeMiller = PrimeChecker.IsPrimeMiller(number, iterations);
            bool isProbablyPrimeBruteForce = PrimeChecker.IsPrimeBruteForce(number);

            Console.WriteLine("---------------BruteForce--------------");
            if (isProbablyPrimeBruteForce)
            {
                Console.WriteLine($"{number} является простым числом.");
            }
            else
            {
                Console.WriteLine($"{number} составное.");
            }
            double executionTimeBruteForce = PrimeChecker.MeasureExecutionTime(() => PrimeChecker.IsPrimeBruteForce(number));
            Console.WriteLine($"Время выполнения BruteForce: {executionTimeBruteForce} мс");

            Console.WriteLine("---------------Miller--------------");
            if (isProbablyPrimeMiller)
            {
                Console.WriteLine($"{number} вероятно является простым числом.");
            }
            else
            {
                Console.WriteLine($"{number} составное.");
            }
            double executionTimeMiller = PrimeChecker.MeasureExecutionTime(() => PrimeChecker.IsPrimeMiller(number, iterations));
            Console.WriteLine($"Время выполнения Miller: {executionTimeMiller} мс");

            Console.WriteLine("-----------------------------");

        }
    }
}
