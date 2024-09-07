using System.Data.Common;
using System.Reflection;
using CsvHelper.Configuration.Attributes;

public class PlayerCharacter
{
    private string? _name;
    private string? _classname;
    private string? _level;
    private string? _hitpoints;
    private List<string>? _equipment;
    private int _id;
    public string? Name
    {
        get { return _name; }
        set { _name = value; }
    }
    public string? Classname
    {
        get { return _classname; }
        set { _classname = value; }
    }
    public string? Level
    {
        get { return _level; }
        set { _level = value; }
    }
    public string? HP
    {
        get { return _hitpoints; }
        set { _hitpoints = value; }
    }
    public int Id
    {
        get { return _id; }
        set { _id = value; }
    }
    public string Equipment
    {
        set {
            List<string>? equip = new List<string>();
            equip = value.Split("|").ToList();
            this._equipment = equip;
        }
    }
    public PlayerCharacter()
    {
        // This is a do nothing constructor.  It is needed because we have a constructor all ready and without it
        // the CSV Helper would not work.  It expects to use the default get/set and cannot because I created
        // the constructor with parameters. 
    }
    public PlayerCharacter(string Name, string Level, string Classname)
    {
        this._name = Name;
        this._level = Level;
        this._classname = Classname;
        this._hitpoints = "0";
        this._equipment = new List<string>();
    }
    public void Add(PlayerCharacter character)
    {
        this.Add(character);
    }
    public void LevelUp(string next_level = "1")
    {
        // Everything is a string in the base clase so I have to convert the values to integers to automatically
        // level up by 1 unless a different level is specified (forward thing here because I don't prompt for a level
        // to move to.  Might need this ability in the future.
        int tmp = 0;
        tmp = Int32.Parse(this._level) + Int32.Parse(next_level);
        this._level = tmp.ToString();
    }
    public void AddEquipment(string item)
    {
        this._equipment.Add(item);
    }
    public string? GetEquipment(string delimiter="|")
    {
        // This allows me to output the list of equipment with a unique delimiter based on need of where it is being called.
        string? list = null;
        for (int i = 0; i < this._equipment.Count(); i++)
        {
            if (list == null) {
                list = this._equipment[i];
            }
            else {
                list += delimiter + this._equipment[i];
            }
        };

        return list;
    }
    public void PrintCharacterProfile()
    {
        string? i = this.Id.ToString();
        string? n = this.Name;
        string? l = this.Level;
        string? c = this.Classname;
        string? e = this.GetEquipment(",");
        Console.WriteLine($"Player #{i}: {n} is a level {l} {c} with the following equipment: {e}.");
    }
}
