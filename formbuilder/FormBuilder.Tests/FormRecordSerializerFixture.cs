using FormBuilder.Builders;
using FormBuilder.Components.FormElements.Anchor;
using FormBuilder.Components.FormElements.Button;
using FormBuilder.Components.FormElements.Checkbox;
using FormBuilder.Components.FormElements.Divider;
using FormBuilder.Components.FormElements.Input;
using FormBuilder.Components.FormElements.Password;
using FormBuilder.Components.FormElements.StackLayout;
using FormBuilder.Models;
using NUnit.Framework;
using System.Text.Json;

namespace FormBuilder.Tests;

public class FormRecordSerializerFixture
{
    [Test]
    public void Can_SerializeAndDeserialize_FormRecord()
    {
        var formRecord = new FormRecord
        {
            Elements = new List<IFormElementRecord>
            {
                new FormStackLayoutRecord
                {
                    Elements = new List<IFormElementRecord>
                    {
                        // Authentication form
                        new FormStackLayoutRecord
                        {
                            IsFormEnabled = true,
                            Elements = new List<IFormElementRecord>
                            {
                                new FormInputFieldRecord
                                {
                                    Name = "Login",
                                    Value = "Login",
                                    Labels = LabelTranslationBuilder.New().AddTranslation("en", "Login").Build()
                                },
                                new FormPasswordFieldRecord
                                {
                                    Name = "Password",
                                    Value = "Password",
                                    Labels = LabelTranslationBuilder.New().AddTranslation("en", "Password").Build()
                                },
                                new FormCheckboxRecord
                                {
                                  Name = "IsRememberMe",
                                  Value = true,
                                  Labels = LabelTranslationBuilder.New().AddTranslation("en", "Remember me").Build()
                                },
                                new FormButtonRecord
                                {
                                    Labels = LabelTranslationBuilder.New().AddTranslation("en", "Authenticate").Build()
                                }
                            }
                        },
                        // Separator
                        new DividerLayoutRecord
                        {
                            Labels = LabelTranslationBuilder.New().AddTranslation("en", "OR").Build()
                        },
                        // Forget my password
                        new FormAnchorRecord
                        {
                            Labels = LabelTranslationBuilder.New().AddTranslation("en", "Forget my password").Build()
                        },
                        // Separator
                        new DividerLayoutRecord
                        {
                            Labels = LabelTranslationBuilder.New().AddTranslation("en", "OR").Build()
                        },
                        // Register
                        new FormAnchorRecord
                        {
                            Labels = LabelTranslationBuilder.New().AddTranslation("en", "Register").Build()
                        },
                        // Separator
                        new DividerLayoutRecord
                        {
                            Labels = LabelTranslationBuilder.New().AddTranslation("en", "OR").Build()
                        }
                    }
                }
            }
        };
        var json = JsonSerializer.Serialize(formRecord);
        var deserializedFormRecord = JsonSerializer.Deserialize<FormRecord>(json);
        var deserializedFormStackLayout = deserializedFormRecord.Elements.First() as FormStackLayoutRecord;
        var deserializedAuthForm = deserializedFormStackLayout.Elements.First() as FormStackLayoutRecord;
        var deserializedLogin = deserializedAuthForm.Elements.ElementAt(0) as FormInputFieldRecord;
        Assert.That(deserializedLogin.Name == "Login");
    }
}