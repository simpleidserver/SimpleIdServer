using FormBuilder.Components.FormElements.Anchor;
using FormBuilder.Components.FormElements.Button;
using FormBuilder.Components.FormElements.Checkbox;
using FormBuilder.Components.FormElements.Divider;
using FormBuilder.Components.FormElements.Input;
using FormBuilder.Components.FormElements.Password;
using FormBuilder.Components.FormElements.StackLayout;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace FormBuilder.Models;

[JsonDerivedType(typeof(DividerLayoutRecord), typeDiscriminator: "Divider")]
[JsonDerivedType(typeof(FormAnchorRecord), typeDiscriminator: "Anchor")]
[JsonDerivedType(typeof(FormButtonRecord), typeDiscriminator: "Button")]
[JsonDerivedType(typeof(FormCheckboxRecord), typeDiscriminator: "Checkbox")]
[JsonDerivedType(typeof(FormInputFieldRecord), typeDiscriminator: "Input")]
[JsonDerivedType(typeof(FormPasswordFieldRecord), typeDiscriminator: "Password")]
[JsonDerivedType(typeof(FormStackLayoutRecord), typeDiscriminator: "StackLayout")]
public interface IFormElementRecord
{
    List<LabelTranslation> Labels { get; set; }
    void ExtractJson(JsonObject json);
}