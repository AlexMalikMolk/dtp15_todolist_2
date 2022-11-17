using System;
using System.ComponentModel.Design;
using System.Threading.Tasks;
using static dtp15_todolist.Todo;

namespace dtp15_todolist
{
    public class Todo
    {
        public static List<TodoItem> list = new List<TodoItem>();

        public const int Active = 1;
        public const int Waiting = 2;
        public const int Ready = 3;
        public static string StatusToString(int status)
        {
            switch (status)
            {
                case Active: return "aktiv";
                case Waiting: return "väntande";
                case Ready: return "avklarad";
                default: return "(felaktig)";
            }
        }
        public class TodoItem
        {
            public int status;
            public int priority;
            public string task;
            public string taskDescription;
            public TodoItem()
            {
                status = Active;
                priority = 1;
                task = "";
                taskDescription = "";
            }
            public TodoItem(string todoLine)
            {
                string[] field = todoLine.Split('|');
                status = Int32.Parse(field[0]);
                priority = Int32.Parse(field[1]);
                task = field[2];
                taskDescription = field[3];
            }
            public void Print(bool verbose = false)
            {
                string statusString = StatusToString(status);
                Console.Write($"|{statusString,-12}|{priority,-6}|{task,-20}|");
                if (verbose)
                    Console.WriteLine($"{taskDescription,-40}|");
                else
                    Console.WriteLine();
            }
        }
        public static void ReadListFromFile()
        {
            string todoFileName = "todo.lis";
            Console.Write($"Läser från fil {todoFileName} ... ");
            StreamReader sr = new StreamReader(todoFileName);
            int numRead = 0;

            string line;
            while ((line = sr.ReadLine()) != null)
            {
                TodoItem item = new TodoItem(line);
                list.Add(item);
                numRead++;
            }
            sr.Close();
            Console.WriteLine($"Läste {numRead} rader.");
        }
        private static void PrintHeadOrFoot(bool head, bool verbose)
        {
            if (head)
            {
                Console.Write("|status      |prio  |namn                |");
                if (verbose) Console.WriteLine("beskrivning                             |");
                else Console.WriteLine();
            }
            Console.Write("|------------|------|--------------------|");
            if (verbose) Console.WriteLine("----------------------------------------|");
            else Console.WriteLine();
        }
        private static void PrintHead(bool verbose)
        {
            PrintHeadOrFoot(head: true, verbose);
        }
        private static void PrintFoot(bool verbose)
        {
            PrintHeadOrFoot(head: false, verbose);
        }


        //Lägg till funktioner för att kunna lista "aktiva"(active) med kommandot "lista"

        public static void PrintTodoList(bool verbose = false, bool active = false)
        {
            PrintHead(verbose);
            foreach (TodoItem item in list)
            {
                if (active)
                {
                    if (item.status is Active) item.Print(verbose);
                }
                else
                {
                    item.Print(verbose);
                }

            }
            PrintFoot(verbose);
        }
        
        public static void PrintTodoList_Active(bool verbose = false)
        {
            PrintHead(verbose);
            foreach (TodoItem item in list)
            {
                if (item.status is Active) item.Print(verbose);
            }
            PrintFoot(verbose);
        }

        //Lägg till funktion för att hantera "bekriv" i TodoItem
        public static bool CreateNewTask()
        {
            TodoItem task = new TodoItem();
            task.task = MyIO.ReadCommand("Uppgiftens namn:");
            var command = MyIO.ReadCommand("Prioritet:");
            if (MyIO.Equals(command, "1") || MyIO.Equals(command, "2") || MyIO.Equals(command, "3") || MyIO.Equals(command, "4"))
            {
                task.priority = int.Parse(command);
            }
            else
            {
                return false;
            }
            task.taskDescription = MyIO.ReadCommand("Beskrivning:");
            list.Add(task);
            return true;
        }










        public static void PrintHelp()
        {
            Console.WriteLine("Kommandon:");
            Console.WriteLine("hjälp                \t lista denna hjälp");
            Console.WriteLine("lista                \t lista alla Active uppgifter, status, prioritet, och namn på uppgiften");
            Console.WriteLine("sluta                \t spara senast laddade filen och avsluta programmet!");
            //lägg till:  ny, beskriv, spara, ladda, aktivera/uppgift/, klar/uppgift/, vänta/uppgift/, sluta  
            //Gör raderna mer läsbara genom att lägga till tab mellan kolumnerna
            Console.WriteLine("ny                   \t lägg till ny uppgift");
            Console.WriteLine("beskriv              \t lista alla Active uppgifter, status, prioritet, namn och beskrivning");
            Console.WriteLine("spara                \t spara uppgifterna");
            Console.WriteLine("ladda                \t ladda listan todo.lis");
            Console.WriteLine("aktivera/uppgift/    \t sätt status på uppgift till Active");
            Console.WriteLine("klar/uppgift/        \t sätt status på uppgift till Ready");
            Console.WriteLine("vänta/uppgift/       \t sätt status på uppgift till Waiting");
            


        }
    }



    
    class MainClass
    {

        public static void Main(string[] args)
        {
            Console.WriteLine("Välkommen till att-göra-listan!");
            //Todo.ReadListFromFile();
            //Todo.PrintTodoList();

            Todo.PrintHelp();
            string command;


            //Lägg till funktioner för ny, beskriv, spara, ladda, aktivera/uppgift/, klar/uppgift/, vänta/uppgift/

            do
            {
                command = MyIO.ReadCommand("> ");
                if (MyIO.Equals(command, "hjälp"))
                {
                    Todo.PrintHelp();
                }
                else if (MyIO.Equals(command, "sluta"))
                {
                    Console.WriteLine("Hej då!");
                    break;
                }

                //Göra om lista funktionen så den endast visar aktiva tasks 

                else if (MyIO.Equals(command, "lista"))
                {
                    if (MyIO.HasArgument(command, "allt"))
                        Todo.PrintTodoList(verbose: false, active: false);
                    else
                        Todo.PrintTodoList(verbose: false, active: true);
                }




                //skapa en ny uppgift
                else if (MyIO.Equals(command, "ny"))
                {
                    if (Todo.CreateNewTask())
                    {
                        Console.WriteLine("Uppgiften tillagd");
                    }
                    else
                    {
                        Console.WriteLine("Försök igen");
                    }
                }
                //Lista alla aktiva uppgifter inklusive beskrivning
                else if (MyIO.Equals(command, "beskriv"))
                {
                    Todo.PrintTodoList_Active(verbose: true);
                }



                //Spara information i todo.lis
                else if (MyIO.Equals(command, "spara"))
                {
                    string todoFileName = "todo.lis";
                    Console.Write($"Sparar till fil {todoFileName} ... ");
                    StreamWriter sw = new StreamWriter(todoFileName);
                    foreach (Todo.TodoItem item in Todo.list)
                    {
                        sw.WriteLine($"{item.status}|{item.priority}|{item.task}|{item.taskDescription}");
                    }
                    sw.Close();
                    Console.WriteLine("listan sparad.");
                }

                else if (MyIO.Equals(command, "ladda"))
                {
                    Todo.ReadListFromFile();
                }






                else
                {
                    Console.WriteLine("Okänt kommando.");

                }
            }
            while (true);

            
        }
    }
    class MyIO
    {


        public static string ReadString()
        {
            string text = Console.ReadLine();
            return text;
        }
        static public string ReadInt(string prompt)
        {
            Console.Write(prompt);
            return Console.ReadLine();
        }


        static public string ReadString(string prompt)
        {
            Console.Write(prompt);
            return Console.ReadLine();
        }



        static public string ReadCommand(string prompt)
        {
            Console.Write(prompt);
            return Console.ReadLine();
        }
        static public bool Equals(string rawCommand, string expected)
        {
            string command = rawCommand.Trim();
            if (command == "") return false;
            else
            {
                string[] cwords = command.Split(' ');
                if (cwords[0] == expected) return true;
            }
            return false;
        }
        static public bool HasArgument(string rawCommand, string expected)
        {
            string command = rawCommand.Trim();
            if (command == "") return false;
            else
            {
                string[] cwords = command.Split(' ');
                if (cwords.Length < 2) return false;
                if (cwords[1] == expected) return true;
            }
            return false;
        }
    }
}
