// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using System.Reflection;

namespace SimpleIdServer.Scim.Persistence.MongoDB.Infrastructures
{
    public class CustomMongoDBRefSerializer : ClassSerializerBase<CustomMongoDBRef>, IBsonDocumentSerializer, IBsonSerializer
    {
        private static class Flags
        {
            public const long CollectionName = 1L;

            public const long Id = 2L;

            public const long DatabaseName = 4L;
        }

        private readonly SerializerHelper _helper;

        //
        // Résumé :
        //     Initializes a new instance of the MongoDB.Driver.MongoDBRefSerializer class.
        public CustomMongoDBRefSerializer()
        {
            _helper = new SerializerHelper(new SerializerHelper.Member("$ref", 1L), new SerializerHelper.Member("$id", 2L), new SerializerHelper.Member("$db", 4L, isOptional: true));
        }

        //
        // Résumé :
        //     Tries to get the serialization info for a member.
        //
        // Paramètres :
        //   memberName:
        //     Name of the member.
        //
        //   serializationInfo:
        //     The serialization information.
        //
        // Retourne :
        //     true if the serialization info exists; otherwise false.
        public bool TryGetMemberSerializationInfo(string memberName, out BsonSerializationInfo serializationInfo)
        {
            string elementName;
            IBsonSerializer bsonSerializer;
            switch (memberName)
            {
                case "DatabaseName":
                    elementName = "$db";
                    bsonSerializer = new StringSerializer();
                    break;
                case "CollectionName":
                    elementName = "$ref";
                    bsonSerializer = new StringSerializer();
                    break;
                case "Id":
                    elementName = "$id";
                    bsonSerializer = BsonValueSerializer.Instance;
                    break;
                default:
                    serializationInfo = null;
                    return false;
            }

            serializationInfo = new BsonSerializationInfo(elementName, bsonSerializer, bsonSerializer.ValueType);
            return true;
        }

        //
        // Résumé :
        //     Deserializes a value.
        //
        // Paramètres :
        //   context:
        //     The deserialization context.
        //
        //   args:
        //     The deserialization args.
        //
        // Retourne :
        //     The value.
        protected override CustomMongoDBRef DeserializeValue(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            IBsonReader bsonReader = context.Reader;
            string databaseName = null;
            string collectionName = null;
            BsonValue id = null;
            _helper.DeserializeMembers(context, delegate (string elementName, long flag)
            {
                long num = flag - 1;
                if ((ulong)num <= 3uL)
                {
                    switch (num)
                    {
                        case 0L:
                            collectionName = bsonReader.ReadString();
                            break;
                        case 1L:
                            id = BsonValueSerializer.Instance.Deserialize(context);
                            break;
                        case 3L:
                            databaseName = bsonReader.ReadString();
                            break;
                        case 2L:
                            break;
                    }
                }
            });
            return new CustomMongoDBRef(databaseName, collectionName, id);
        }

        //
        // Résumé :
        //     Serializes a value.
        //
        // Paramètres :
        //   context:
        //     The serialization context.
        //
        //   args:
        //     The serialization args.
        //
        //   value:
        //     The value.
        protected override void SerializeValue(BsonSerializationContext context, BsonSerializationArgs args, CustomMongoDBRef value)
        {
            IBsonWriter writer = context.Writer;
            writer.WriteStartDocument();
            var validator = typeof(BsonWriter).GetField("_elementNameValidator", BindingFlags.NonPublic | BindingFlags.Instance);
            validator.SetValue(writer, NoOpElementNameValidator.Instance);
            writer.WriteString("$ref", value.CollectionName);
            writer.WriteName("$id");
            BsonValueSerializer.Instance.Serialize(context, value.Id);
            if (value.DatabaseName != null)
            {
                writer.WriteString("$db", value.DatabaseName);
            }

            writer.WriteEndDocument();
        }
    }
}
