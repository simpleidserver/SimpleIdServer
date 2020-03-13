// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using SimpleIdServer.Scim.Domain;
using SimpleIdServer.Scim.Helpers;
using SimpleIdServer.Scim.Persistence.EF.Extensions;
using SimpleIdServer.Scim.Persistence.EF.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace SimpleIdServer.Scim.Persistence.EF.Tests
{
    public class FilterFixture
    {
        [Fact]
        public void When_Parse_And_Execute_Filter()
        {
            var firstRepresentation = new SCIMRepresentationModel
            {
                Id = Guid.NewGuid().ToString(),
                ExternalId = "externalId",
                Created = DateTime.UtcNow,
                LastModified = DateTime.UtcNow,
                ResourceType = "user",
                Attributes = new List<SCIMRepresentationAttributeModel>
                {
                    new SCIMRepresentationAttributeModel
                    {
                        Id = Guid.NewGuid().ToString(),
                        SchemaAttribute = new SCIMSchemaAttributeModel
                        {
                            Name  = "userName",
                            Type = SCIMSchemaAttributeTypes.STRING
                        },
                        Values = new List<SCIMRepresentationAttributeValueModel>
                        {
                            new SCIMRepresentationAttributeValueModel
                            {
                                Id = Guid.NewGuid().ToString(),
                                ValueString = "bjensen"
                            }
                        }
                    },
                    new SCIMRepresentationAttributeModel
                    {
                        Id = Guid.NewGuid().ToString(),
                        SchemaAttribute = new SCIMSchemaAttributeModel
                        {
                            Name = "name",
                            Type = SCIMSchemaAttributeTypes.COMPLEX
                        },
                        Children = new List<SCIMRepresentationAttributeModel>
                        {
                            new SCIMRepresentationAttributeModel
                            {
                                Id = Guid.NewGuid().ToString(),
                                SchemaAttribute = new SCIMSchemaAttributeModel
                                {
                                    Name = "familyName",
                                    Type = SCIMSchemaAttributeTypes.STRING
                                },
                                Values = new List<SCIMRepresentationAttributeValueModel>
                                {
                                    new SCIMRepresentationAttributeValueModel
                                    {
                                        Id = Guid.NewGuid().ToString(),
                                        ValueString = "O'Malley"
                                    }
                                }
                            }
                        }
                    },
                    new SCIMRepresentationAttributeModel
                    {
                        Id = Guid.NewGuid().ToString(),
                        SchemaAttribute = new SCIMSchemaAttributeModel
                        {
                            Name  = "title",
                            Type = SCIMSchemaAttributeTypes.STRING
                        },
                        Values = new List<SCIMRepresentationAttributeValueModel>
                        {
                            new SCIMRepresentationAttributeValueModel
                            {
                                Id = Guid.NewGuid().ToString(),
                                ValueString = "title"
                            }
                        }
                    },
                    new SCIMRepresentationAttributeModel
                    {
                        Id = Guid.NewGuid().ToString(),
                        SchemaAttribute = new SCIMSchemaAttributeModel
                        {
                            Name  = "userType",
                            Type = SCIMSchemaAttributeTypes.STRING
                        },
                        Values = new List<SCIMRepresentationAttributeValueModel>
                        {
                            new SCIMRepresentationAttributeValueModel
                            {
                                Id = Guid.NewGuid().ToString(),
                                ValueString = "Employee"
                            }
                        }
                    },
                    new SCIMRepresentationAttributeModel
                    {
                        Id = Guid.NewGuid().ToString(),
                        SchemaAttribute = new SCIMSchemaAttributeModel
                        {
                            Name  = "emails",
                            Type = SCIMSchemaAttributeTypes.COMPLEX
                        },
                        Children = new List<SCIMRepresentationAttributeModel>
                        {
                            new SCIMRepresentationAttributeModel
                            {
                                Id = Guid.NewGuid().ToString(),
                                SchemaAttribute = new SCIMSchemaAttributeModel
                                {
                                    Name  = "value",
                                    Type = SCIMSchemaAttributeTypes.STRING
                                },
                                Values = new List<SCIMRepresentationAttributeValueModel>
                                {
                                    new SCIMRepresentationAttributeValueModel
                                    {
                                        Id = Guid.NewGuid().ToString(),
                                        ValueString = "example.com"
                                    }
                                }
                            },
                            new SCIMRepresentationAttributeModel
                            {
                                Id = Guid.NewGuid().ToString(),
                                SchemaAttribute = new SCIMSchemaAttributeModel
                                {
                                    Name  = "type",
                                    Type = SCIMSchemaAttributeTypes.STRING
                                },
                                Values = new List<SCIMRepresentationAttributeValueModel>
                                {
                                    new SCIMRepresentationAttributeValueModel
                                    {
                                        Id = Guid.NewGuid().ToString(),
                                        ValueString = "work"
                                    }
                                }
                            }
                        }
                    },
                    new SCIMRepresentationAttributeModel
                    {
                        Id = Guid.NewGuid().ToString(),
                        SchemaAttribute = new SCIMSchemaAttributeModel
                        {
                            Name  = "emails",
                            Type = SCIMSchemaAttributeTypes.COMPLEX
                        },
                        Children = new List<SCIMRepresentationAttributeModel>
                        {
                            new SCIMRepresentationAttributeModel
                            {
                                Id = Guid.NewGuid().ToString(),
                                SchemaAttribute = new SCIMSchemaAttributeModel
                                {
                                    Name  = "value",
                                    Type = SCIMSchemaAttributeTypes.STRING
                                },
                                Values = new List<SCIMRepresentationAttributeValueModel>
                                {
                                    new SCIMRepresentationAttributeValueModel
                                    {
                                        Id = Guid.NewGuid().ToString(),
                                        ValueString = "example.org"
                                    }
                                }
                            }
                        }
                    },
                    new SCIMRepresentationAttributeModel
                    {
                        Id = Guid.NewGuid().ToString(),
                        SchemaAttribute = new SCIMSchemaAttributeModel
                        {
                            Name  = "phoneNumbers",
                            Type = SCIMSchemaAttributeTypes.COMPLEX
                        },
                        Children = new List<SCIMRepresentationAttributeModel>
                        {
                            new SCIMRepresentationAttributeModel
                            {
                                Id = Guid.NewGuid().ToString(),
                                SchemaAttribute = new SCIMSchemaAttributeModel
                                {
                                    Name  = "primary",
                                    Type = SCIMSchemaAttributeTypes.BOOLEAN
                                },
                                Values = new List<SCIMRepresentationAttributeValueModel>
                                {
                                    new SCIMRepresentationAttributeValueModel
                                    {
                                        Id = Guid.NewGuid().ToString(),
                                        ValueBoolean = true
                                    }
                                }
                            }
                        }
                    }
                }
            };
            firstRepresentation.LastModified = DateTime.Parse("2012-05-13T04:42:34Z");
            firstRepresentation.Version = "2";
            var secondRepresentation = new SCIMRepresentationModel
            {
                Id = Guid.NewGuid().ToString(),
                ExternalId = "externalId",
                Created = DateTime.UtcNow,
                LastModified = DateTime.UtcNow,
                ResourceType = "user",
                Attributes = new List<SCIMRepresentationAttributeModel>
                {
                    new SCIMRepresentationAttributeModel
                    {
                        Id = Guid.NewGuid().ToString(),
                        SchemaAttribute = new SCIMSchemaAttributeModel
                        {
                            Name  = "userName",
                            Type = SCIMSchemaAttributeTypes.STRING
                        },
                        Values = new List<SCIMRepresentationAttributeValueModel>
                        {
                            new SCIMRepresentationAttributeValueModel
                            {
                                Id = Guid.NewGuid().ToString(),
                                ValueString = "Justine"
                            }
                        }
                    },
                    new SCIMRepresentationAttributeModel
                    {
                        Id = Guid.NewGuid().ToString(),
                        SchemaAttribute = new SCIMSchemaAttributeModel
                        {
                            Name  = "title",
                            Type = SCIMSchemaAttributeTypes.STRING
                        },
                        Values = new List<SCIMRepresentationAttributeValueModel>
                        {
                            new SCIMRepresentationAttributeValueModel
                            {
                                Id = Guid.NewGuid().ToString(),
                                ValueString = "title"
                            }
                        }
                    },
                    new SCIMRepresentationAttributeModel
                    {
                        Id = Guid.NewGuid().ToString(),
                        SchemaAttribute = new SCIMSchemaAttributeModel
                        {
                            Name  = "userType",
                            Type = SCIMSchemaAttributeTypes.STRING
                        },
                        Values = new List<SCIMRepresentationAttributeValueModel>
                        {
                            new SCIMRepresentationAttributeValueModel
                            {
                                Id = Guid.NewGuid().ToString(),
                                ValueString = "Intern"
                            }
                        }
                    },
                    new SCIMRepresentationAttributeModel
                    {
                        Id = Guid.NewGuid().ToString(),
                        SchemaAttribute = new SCIMSchemaAttributeModel
                        {
                            Name  = "emails",
                            Type = SCIMSchemaAttributeTypes.COMPLEX
                        },
                        Children = new List<SCIMRepresentationAttributeModel>
                        {
                            new SCIMRepresentationAttributeModel
                            {
                                Id = Guid.NewGuid().ToString(),
                                SchemaAttribute = new SCIMSchemaAttributeModel
                                {
                                    Name  = "value",
                                    Type = SCIMSchemaAttributeTypes.STRING
                                },
                                Values = new List<SCIMRepresentationAttributeValueModel>
                                {
                                    new SCIMRepresentationAttributeValueModel
                                    {
                                        Id = Guid.NewGuid().ToString(),
                                        ValueString = "example.be"
                                    }
                                }
                            }
                        }
                    },
                    new SCIMRepresentationAttributeModel
                    {
                        Id = Guid.NewGuid().ToString(),
                        SchemaAttribute = new SCIMSchemaAttributeModel
                        {
                            Name  = "ims",
                            Type = SCIMSchemaAttributeTypes.COMPLEX
                        },
                        Children = new List<SCIMRepresentationAttributeModel>
                        {
                            new SCIMRepresentationAttributeModel
                            {
                                Id = Guid.NewGuid().ToString(),
                                SchemaAttribute = new SCIMSchemaAttributeModel
                                {
                                    Name  = "type",
                                    Type = SCIMSchemaAttributeTypes.STRING
                                },
                                Values = new List<SCIMRepresentationAttributeValueModel>
                                {
                                    new SCIMRepresentationAttributeValueModel
                                    {
                                        Id = Guid.NewGuid().ToString(),
                                        ValueString = "xmpp"
                                    }
                                }
                            },
                            new SCIMRepresentationAttributeModel
                            {
                                Id = Guid.NewGuid().ToString(),
                                SchemaAttribute = new SCIMSchemaAttributeModel
                                {
                                    Name  = "value",
                                    Type = SCIMSchemaAttributeTypes.STRING
                                },
                                Values = new List<SCIMRepresentationAttributeValueModel>
                                {
                                    new SCIMRepresentationAttributeValueModel
                                    {
                                        Id = Guid.NewGuid().ToString(),
                                        ValueString = "foo.com"
                                    }
                                }
                            }
                        }
                    }
                }
            };
            secondRepresentation.LastModified = DateTime.Parse("2010-05-13T04:42:34Z");
            var representations = new List<SCIMRepresentationModel>
            {
                firstRepresentation,
                secondRepresentation
            };

            var firstResult = ParseAndExecuteFilter(representations.AsQueryable(), "userName eq \"bjensen\"");
            var secondResult = ParseAndExecuteFilter(representations.AsQueryable(), "name.familyName co \"O'Malley\"");
            var thirdResult = ParseAndExecuteFilter(representations.AsQueryable(), "userName sw \"J\"");
            var fourthResult = ParseAndExecuteFilter(representations.AsQueryable(), "title pr");
            var fifthResult = ParseAndExecuteFilter(representations.AsQueryable(), "meta.lastModified gt \"2011-05-13T04:42:34Z\"");
            var sixResult = ParseAndExecuteFilter(representations.AsQueryable(), "meta.lastModified ge \"2011-05-13T04:42:34Z\"");
            var sevenResult = ParseAndExecuteFilter(representations.AsQueryable(), "meta.lastModified lt \"2011-05-13T04:42:34Z\"");
            var eightResult = ParseAndExecuteFilter(representations.AsQueryable(), "meta.lastModified le \"2011-05-13T04:42:34Z\"");
            var nineResult = ParseAndExecuteFilter(representations.AsQueryable(), "title pr and userType eq \"Employee\"");
            var tenResult = ParseAndExecuteFilter(representations.AsQueryable(), "title pr or userType eq \"Intern\"");
            var elevenResult = ParseAndExecuteFilter(representations.AsQueryable(), "userType eq \"Employee\" and (emails.value co \"example.org\" or emails.value co \"example.org\")");
            var twelveResult = ParseAndExecuteFilter(representations.AsQueryable(), "userType ne \"Employee\" and not (emails co \"example.com\" or emails.value co \"example.org\")");
            var thirteenResult = ParseAndExecuteFilter(representations.AsQueryable(), "userType eq \"Employee\" and (emails.type eq \"work\")");
            var fourteenResult = ParseAndExecuteFilter(representations.AsQueryable(), "userType eq \"Employee\" and emails[type eq \"work\" and value co \"example.com\"]");
            var fifteenResult = ParseAndExecuteFilter(representations.AsQueryable(), "emails[type eq \"work\" and value co \"example.com\"] or ims[type eq \"xmpp\" and value co \"foo.com\"]");
            var sixteenResult = ParseAndExecuteFilter(representations.AsQueryable(), "meta.lastModified gt \"2011-05-13T04:42:34Z\" and meta.version eq \"2\"");
            var seventeenResult = ParseAndExecuteFilter(representations.AsQueryable(), "meta.lastModified pr");
            var eighteenResult = ParseAndExecuteFilter(representations.AsQueryable(), "phoneNumbers[primary eq \"true\"]");

            Assert.Equal(1, firstResult.Count());
            Assert.Equal(1, secondResult.Count());
            Assert.Equal(1, thirdResult.Count());
            Assert.Equal(2, fourthResult.Count());
            Assert.Equal(1, fifthResult.Count());
            Assert.Equal(1, sixResult.Count());
            Assert.Equal(1, sevenResult.Count());
            Assert.Equal(1, eightResult.Count());
            Assert.Equal(1, nineResult.Count());
            Assert.Equal(2, tenResult.Count());
            Assert.Equal(1, elevenResult.Count());
            Assert.Equal(1, twelveResult.Count());
            Assert.Equal(1, thirteenResult.Count());
            Assert.Equal(1, fourteenResult.Count());
            Assert.Equal(2, fifteenResult.Count());
            Assert.Equal(1, sixteenResult.Count());
            Assert.Equal(2, seventeenResult.Count());
            Assert.Equal(1, eighteenResult.Count());
        }

        private IQueryable<SCIMRepresentationModel> ParseAndExecuteFilter(IQueryable<SCIMRepresentationModel> representations, string filter)
        {
            var parsed = SCIMFilterParser.Parse(filter, new List<SCIMSchema> { SCIMConstants.StandardSchemas.UserSchema });
            var evaluatedExpression = parsed.Evaluate(representations);
            return (IQueryable<SCIMRepresentationModel>)evaluatedExpression.Compile().DynamicInvoke(representations);
        }
    }
}
