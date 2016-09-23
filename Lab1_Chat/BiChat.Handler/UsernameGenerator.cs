using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace BiChat.Handler
{
    public class UsernameGenerator
    {
        private static RandomNumberGenerator rnd = RandomNumberGenerator.Create();

        private static string[] animals = new string[] 
        { "Коала",
          "Панда",
          "Выдра",
          "Выхухоль",
          "Пантера",
          "Чайка",
          "Лама",
          "Лисица",
          "Макака",
          "Мышь",
          "Мартышка",
          "Нутрия",
          "Нанду",
          "Лягушка",
          "Свинка",
          "Гиена",
          "Гну",
          "Горилла",
          "Газель",
          "Анаконда",
          "Гадюка",
          "Антилопа",
          "Альпака",
          "Акула",
          "Белка",
          "Лори",
          "Землеройка",
          "Норка",
          "Шиншилла",
          "Руконожка"
        };

        private static string[] adjectives = new string[] 
        {
            "Яросная",
            "Обидчивая",
            "Заботливая",
            "Радужная",
            "Волшебная",
            "Хитрая",
            "Пушистая",
            "Злая",
            "Сердитая",
            "Добродушная",
            "Доверчивая",
            "Болтливая",
            "Многогранная",
            "Распутная",
            "Ветренная",
            "Верная",
            "Разборчивая",
            "Мимолетная",
            "Цветущая",
            "Занудная",
            "Рисковая",
            "Самоуверенная",
            "Ядерная",
            "Медная",
            "Подозрительная",
            "Начитанная",
            "Звездная",
            "Стремная",
            "Послушная",
            "Податливая",
            "Бесхребетная",
            "Ваще",
        };

        public static string CreateName()
        {
            string animal = animals[GetRandomInt() % animals.Length];
            string adjective = adjectives[GetRandomInt() % adjectives.Length];
            return adjective + " " + animal; 
        }

        private static int GetRandomInt()
        {
            using (RNGCryptoServiceProvider rg = new RNGCryptoServiceProvider())
            {
                byte[] rno = new byte[5];
                rg.GetBytes(rno);
                int randomValue = BitConverter.ToInt32(rno, 0);
                return Math.Abs(randomValue);
            }
        }

    }
}
