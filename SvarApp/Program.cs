using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Svara
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            var query = new Svara.Query("temp.txt");

            var test = query.GetUserInput("Hello there: ");

            if(test.answered)
            {
                Console.WriteLine($"User answered: {test.answer}");
            }
            else
                Console.WriteLine("User provided no input");

            var listTest = query.GetUserInput(new List<string> { "first", "2nd", "  third", "4th" });
            if(listTest.anaswered)
            {
                Console.WriteLine("The user selected: ");
                foreach(var ans in listTest.answers)
                    Console.WriteLine(ans);
            }

            else
            {
                Console.WriteLine("User did no selection");
            }

        }
    }
}
