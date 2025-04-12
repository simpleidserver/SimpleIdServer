using FormBuilder.Components.FormElements.Input;
using FormBuilder.Models;
using FormBuilder.Models.Rules;

namespace FormBuilder.Builders;

public class BaseFormRecordCollectionBuilder<B, T> 
    where B : class
    where T : IFormRecordCollection
{
    public BaseFormRecordCollectionBuilder(T record)
    {
        Record = record;
    }

    public T Record { get; private set; }

    public B AddDivider(Action<DividerLayoutRecordBuilder> cb = null)
    {
        var builder = new DividerLayoutRecordBuilder();
        if (cb != null)
        {
            cb(builder);
        }

        Record.Elements.Add(builder.Build());
        return this as B;
    }

    public B AddInputHiddenField(string name, List<ITransformationRule> transformationRules = null)
    {
        transformationRules ??= new List<ITransformationRule>();
        Record.Elements.Add(new FormInputFieldRecord
        {
            Id = Guid.NewGuid().ToString(),
            Name = name,
            FormType = FormInputTypes.HIDDEN,
            Transformations = transformationRules
        });
        return this as B;
    }

    public B AddInputTextField(string name, Action<FormInputBuilder> cb = null)
    {
        var builder = new FormInputBuilder(name, FormInputTypes.TEXT);
        if(cb != null)
        {
            cb(builder);
        }

        Record.Elements.Add(builder.Build());
        return this as B;
    }

    public B AddAnchor(string id = null, string correlationId = null, Action<FormAnchorRecordBuilder> cb = null)
    {
        var builder = new FormAnchorRecordBuilder(id, correlationId);
        if (cb != null)
        {
            cb(builder);
        }
        Record.Elements.Add(builder.Build());
        return this as B;
    }

    public B AddElement(IFormElementRecord elt)
    {
        Record.Elements.Add(elt);
        return this as B;
    }

    public B AddStackLayout(string id = null, string correlationId = null, Action<FormStackLayoutRecordBuilder> cb = null)
    {
        var builder = new FormStackLayoutRecordBuilder(id, correlationId);
        if(cb != null)
        {
            cb(builder);
        }

        Record.Elements.Add(builder.Build());
        return this as B;
    }

    public T Build()
    {
        return Record;
    }
}
