namespace FormBuilder.Factories;

public interface IFormElementDefinitionFactory
{
    IFormElementDefinition Build(string type);
}

public class FormElementDefinitionFactory : IFormElementDefinitionFactory
{
    private readonly IEnumerable<IFormElementDefinition> _definitions;

    public FormElementDefinitionFactory(IEnumerable<IFormElementDefinition> definitions)
    {
        _definitions = definitions;
    }

    public IFormElementDefinition Build(string type)
        => _definitions.Single(d => d.Type == type);
}
