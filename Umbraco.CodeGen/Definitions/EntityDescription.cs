namespace Umbraco.CodeGen.Definitions
{
    public abstract class EntityDescription : Entity, IEntityDescription
    {
        public string Name { get; set; }
        public string Alias { get; set; }
        public string Description { get; set; }
    }
}