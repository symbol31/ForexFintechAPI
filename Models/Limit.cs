namespace ForexFintechAPI.Models;

[AttributeUsage(AttributeTargets.Method)]
public class Limit : Attribute
{
    public string Name { get; set; }
}
