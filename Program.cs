using System.ComponentModel.Design;
using System.Formats.Asn1;
using System.Globalization;
using System.IO;
using System.Linq.Expressions;
using System.Net.Mail;
using System.Numerics;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics.X86;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Xml.Linq;
using CsvHelper;
using Microsoft.VisualBasic;

namespace GWJ_Assignement1_FileIO;
    class Program
{
    static string DisplayMenu()
    {
        string input = "";

        Console.Clear();
        Console.WriteLine("Main Menu\r\n");
        Console.WriteLine("1. Display Characters");
        Console.WriteLine("2. Add Character");
        Console.WriteLine("3. Level Up Character");
        Console.WriteLine("\r\n0. Exit");
        Console.WriteLine("\r\nPlease enter a number from the menu above.");

        input = Console.ReadLine();

        return input;
    }
    static bool ConfirmYN()
    {
        char response = '1';
        bool ans = false;

        while (response == '1')
        {
            Console.WriteLine("Yes (Y) or No (N)?");
            response = char.ToUpper(Convert.ToChar(Console.Read()));

            switch (response)
            {
                case 'Y':
                    ans = true;
                    response = '0';
                    break;

                case 'N':
                    ans = false;
                    response = '0';
                    break;

                default:
                    Console.WriteLine("You can only answer with Yes (Y) or No (N)");
                    Console.Clear();
                    response = '1';
                    break;
            }
        }
        return ans;
    }
    static void AddCharacter(List<PlayerCharacter> tmpPlayerCharacter)
    {
        bool EndInput = false;
        bool EndEquipInput = false;

        string? name = null;
        string? charactertype = null;
        string? level = null;
        string? equiplst = null;
        string? equipname = null;
        PlayerCharacter character = null;

        while (EndInput == false)
        {
            Console.Clear();
            Console.Write("\r\nEnter your character's name (Type 'end' to exit add): ");
            name = Console.ReadLine();
            if (name == null) {
                Console.Write("You must enter a character's name or type end to return to the menu.");
                EndEquipInput = true;
            }
            else
            {
                if (name.ToLower() == "end") {
                    EndEquipInput = true;
                }
                else { 
                    Console.Write("Enter your character's class (Type 'end' to exit add): ");
                    charactertype = Console.ReadLine();
                    if (charactertype == null)
                    {
                        Console.Write("You must enter a character's class or type end to return to the menu.");
                        EndEquipInput = true;
                    }
                    else
                    {
                        if (name.ToLower() == "end")
                        {
                            EndEquipInput = true;
                            EndInput = true;
                        }
                        else
                        {
                            Console.Write("Enter your character's level  (Type 'End' to exit): ");
                            level = Console.ReadLine();
                            if (level?.ToLower() == "end")
                            {
                                Console.Write("You must enter a character's level or 'End' to return to the menu.");
                                EndEquipInput = true;
                            }
                            else
                            {
                                // If no level was entered the character starts at zero.
                                if (level == "")
                                {
                                    level = "0";
                                }
                                EndEquipInput = false;
                                EndInput = true;
                            }
                        }
                    }
                }
            }

            if (EndEquipInput == false) {

                // Ask for more equipment until the user indicates they are done
                character = new PlayerCharacter(name, level, charactertype);
                while (EndEquipInput == false) {
                    Console.Write("Enter your character's equipment (Type 'Done' when finished): ");
                    equipname = Console.ReadLine();
                    if (equipname == null) {
                        Console.WriteLine("Type 'Done' when finished entering equipment.");
                    }
                    else {
                        if (equipname.ToLower() == "done") {
                            EndEquipInput = true;
                        }
                        else {
                            // Add the user input into the list of items
                            if (equiplst == null) { 
                                character.AddEquipment(equipname);
                            }
                        }
                    }
                }
            }

            string? n = character.Name;
            string? l = character.Level;
            string? c = character.Classname;
            string? e = character.GetEquipment(",");
            string? str = null;

            Console.Clear();
            Console.WriteLine($"Add, {n} the {c} at level {l} with the following equipment: {e}.");
            bool AddCharacter = ConfirmYN();
            if (AddCharacter) {

                Console.Clear();
                str = String.Format("Welcome, {0} the {1}! Congradulations on achieving level {2}. Your supplies include: {3}.", n, c, l, e);
                tmpPlayerCharacter.Add(new PlayerCharacter(n,l,c));
                tmpPlayerCharacter[tmpPlayerCharacter.Count() - 1].Id = tmpPlayerCharacter.Count();
                tmpPlayerCharacter[tmpPlayerCharacter.Count() - 1].AddEquipment(character.GetEquipment("|"));
            }
            else {
                str = String.Format("Sorry you will not be part of the campaign, {0} the {1}!", n, c);
            }
            Console.WriteLine(str);
        }
    }
    public static List<PlayerCharacter> GetCharacters()
    {
        List<PlayerCharacter> pc_list = new List<PlayerCharacter>();

        string? path = Directory.GetCurrentDirectory();

        //using (var reader = new StreamReader("C:\\Users\\User\\source\\repos\\Assignment1_FileIO\\Assignment1_FileIO\\input.csv"))
        using (var reader = new StreamReader(path + "\\input.csv"))
        using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
        {
            var records = new List<PlayerCharacter>();
            csv.Read();
            csv.ReadHeader();
            while (csv.Read())
            {
                PlayerCharacter record = new PlayerCharacter()
                {
                    Name = csv.GetField("Name"),
                    Classname = csv.GetField("Class"),
                    Level = csv.GetField("Level"),
                    HP = csv.GetField("HP"),
                    Equipment = csv.GetField("Equipment"),
                    Id = csv.Context.Parser.RawRow-1
                };  
                records.Add(record);
            }
            pc_list = records;
        }
        return pc_list;
    }
    public static void UpdateCharacters(List<PlayerCharacter> updatedcampaign)
    {
        string? path = Directory.GetCurrentDirectory();

        using (var writer = new StreamWriter(path + "\\input.csv"))
        using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
        {
            csv.WriteRecords(updatedcampaign);
        }
    }
    public static void ListCharacters(List<PlayerCharacter> tmpPlayerCharacter)
    {
        Console.Clear();

        Console.WriteLine("The campaign includes the following player characters:\r\n");

        if (tmpPlayerCharacter.Count == 0)
        {
            Console.WriteLine("There are no player characters in the current campaign.");
        }
        else
        {
            foreach (var player in tmpPlayerCharacter)
            {
                player.PrintCharacterProfile();
            }

        }
    }
    public static void LevelUpCharacters(List<PlayerCharacter> tmpPlayerCharacter)
    {
        string? input = null;
        bool End = false;

        while (End != true)
        {
            Console.Clear();
            ListCharacters(tmpPlayerCharacter);
            if (tmpPlayerCharacter.Count() > 0) {
                Console.WriteLine("\r\n\r\nEnter a player number to level up.");
                input = Console.ReadLine();
                if (input == null) {
                    Console.WriteLine("You must select a player number or enter '0' (zero) to exit.");
                    End = false;
                    break;
                }
                else
                {
                    int choice = Int32.Parse(input);
                    if (string.Compare((choice-1).ToString(), (tmpPlayerCharacter.Count() - 1).ToString()) > 0)
                    {
                        Console.WriteLine("Player number {0} is not valid, please select another player number.", input);
                        End = false;
                        break;
                    }
                    switch (input)
                    {
                        case "0":
                            End = true;
                            break;
                        default:
                            int idx = 0;
                            if (Int32.TryParse(input, out idx))
                            {
                                Console.Clear();
                                tmpPlayerCharacter[idx - 1].PrintCharacterProfile();

                                string? n = tmpPlayerCharacter[idx - 1].Name;
                                string? l = tmpPlayerCharacter[idx - 1].Level;
                                string? c = tmpPlayerCharacter[idx - 1].Classname;

                                int lvl2 = 0;
                                bool success = int.TryParse(l, out lvl2);
                                lvl2 = lvl2 + 1;

                                Console.WriteLine("\r\nIs {0} the {1} ready to level up from level {2} to level {3}?", n, c, l, (lvl2).ToString());
                                bool UpdCharacter = ConfirmYN();
                                Console.Clear();
                                if (UpdCharacter) {
                                    Console.WriteLine("The Dungeon Master has confirmed that {0} the {1} is ready for the responsibility that comes with level {2} power.", n, c, lvl2);
                                    tmpPlayerCharacter[idx - 1].LevelUp();
                                }
                                else
                                {
                                    Console.WriteLine("The Dungeon Master has confirmed that {0} the {1} is not ready for such great power.", n, c);
                                }
                                End = true;
                            }
                            break;
                    }
                }
            }
            else
            {
                // Message is displayed by the ListCharacters method so no need for one here
                End = true;
                break;
            }
        }
    }
    public static void Main()
    {
        string ret_val;
        string? input = "";
        bool EndProg = false;
        List<PlayerCharacter> campaignList = new List<PlayerCharacter>();

        campaignList = GetCharacters();
     
        while (EndProg != true)
        {
            ret_val = DisplayMenu();
            switch(ret_val)
            { 
                case "1":
                    ListCharacters(campaignList);
                    Console.WriteLine("\r\n\r\nPress <Enter> key when you are ready to continue...");
                    input = Console.ReadLine();
                    break;
                case "2":
                    AddCharacter(campaignList);
                    Console.WriteLine("\r\n\r\nPress <Enter> key when you are ready to continue...");
                    input = Console.ReadLine();
                    break;
                case "3":
                    LevelUpCharacters(campaignList);
                    Console.WriteLine("\r\n\r\nPress <Enter> key when you are ready to continue...");
                    input = Console.ReadLine();
                    break;
                case "0":
                    // Leave the program
                    UpdateCharacters(campaignList);
                    Console.WriteLine("\r\n\r\\nExiting...");
                    Console.Clear();
                    EndProg = true;
                    break;  
                default:
                    Console.WriteLine("\r\n\r\nThat is an invalid select, please try again.");
                    break;
            }

            if (ret_val != "0") {
                // Pause the screen before refreshing
                Thread.Sleep(2000);
                Console.Clear();
            }
        }
    }
}