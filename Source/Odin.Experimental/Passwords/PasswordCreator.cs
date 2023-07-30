using System;
using Odin.DesignContracts;

namespace Odin.Passwords
{
    /// <summary>
    /// Utility to create passwords
    /// </summary>
    public static class PasswordCreator
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="numberofNumericDigits"></param>
        /// <returns></returns>
        public static string CreateSimpleWordAndNumberPassword(short numberofNumericDigits)
        {
            PreCondition.Requires<ArgumentOutOfRangeException>(numberofNumericDigits >= 0);

            string password = GetRandomShortWord();
            Random rnd = new Random();
            for (int i = 0; i < numberofNumericDigits; i++)
            {
                password = password + rnd.Next(0, 10);
            }
            return password;
        }

        private static string GetRandomShortWord()
        {
            int random = new Random().Next(0, 18);
            switch (random  )
            {
                case 0:
                    return "Lion";
                case 1:
                    return "Tiger";
                case 2:
                    return "Sheep";
                case 3:
                    return "Mazda";
                case 4:
                    return "Ford";
                case 5:
                    return "Soap";
                case 6:
                    return "Zebra";
                case 7:
                    return "Apple";
                case 8:
                    return "Orange";
                case 9:
                    return "Yellow";
                case 10:
                    return "Green";
                case 11:
                    return "Blue";
                case 12:
                    return "Happy";
                case 13:
                    return "Funny";
                case 14:
                    return "Flower";
                case 15:
                    return "Festive";
                case 16:
                    return "Water";
                case 17:
                    return "Train";
                case 18:
                    return "Road";
                default:
                    return "Purple";
            }
        }
        
    }
}