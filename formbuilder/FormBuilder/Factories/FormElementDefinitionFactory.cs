namespace FormBuilder.Factories;

public interface IFormElementDefinitionFactory
{
    List<IFormElementDefinition> GetAll();
    IFormElementDefinition Build(string type);
}

public class FormElementDefinitionFactory : IFormElementDefinitionFactory
{
    private readonly IEnumerable<IFormElementDefinition> _definitions;

    public FormElementDefinitionFactory(IEnumerable<IFormElementDefinition> definitions)
    {
        _definitions = definitions;
    }

    public List<IFormElementDefinition> GetAll() => _definitions.ToList();

    public IFormElementDefinition Build(string type)
        => _definitions.Single(d => d.Type == type);
}
