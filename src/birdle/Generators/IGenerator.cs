namespace birdle.Generators;

public interface IGenerator
{
    public string Generate();
    
    public bool CheckIfValid(string response);
}