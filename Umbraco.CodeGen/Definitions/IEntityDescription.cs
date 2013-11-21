namespace Umbraco.CodeGen.Definitions
{
    public interface IEntityDescription
    {
        string Name { get; }
        string Alias { get; }
        string Description { get; }
    }
}