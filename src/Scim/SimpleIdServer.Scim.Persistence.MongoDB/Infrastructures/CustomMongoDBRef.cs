// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace SimpleIdServer.Scim.Persistence.MongoDB.Infrastructures
{
    [BsonSerializer(typeof(CustomMongoDBRefSerializer))]
    public class CustomMongoDBRef : IEquatable<CustomMongoDBRef>
    {
        private string _databaseName;

        private string _collectionName;

        private BsonValue _id;

        //
        // Résumé :
        //     Gets the name of the database that contains the document.
        public string DatabaseName => _databaseName;

        //
        // Résumé :
        //     Gets the name of the collection that contains the document.
        public string CollectionName => _collectionName;

        //
        // Résumé :
        //     Gets the Id of the document.
        public BsonValue Id => _id;

        private CustomMongoDBRef()
        {
        }

        //
        // Résumé :
        //     Creates a MongoDBRef.
        //
        // Paramètres :
        //   collectionName:
        //     The name of the collection that contains the document.
        //
        //   id:
        //     The Id of the document.
        public CustomMongoDBRef(string collectionName, BsonValue id)
            : this(null, collectionName, id)
        {
        }

        //
        // Résumé :
        //     Creates a MongoDBRef.
        //
        // Paramètres :
        //   databaseName:
        //     The name of the database that contains the document.
        //
        //   collectionName:
        //     The name of the collection that contains the document.
        //
        //   id:
        //     The Id of the document.
        public CustomMongoDBRef(string databaseName, string collectionName, BsonValue id)
        {
            if (collectionName == null)
            {
                throw new ArgumentNullException("collectionName");
            }

            if (id == null)
            {
                throw new ArgumentNullException("id");
            }

            _databaseName = databaseName;
            _collectionName = collectionName;
            _id = id;
        }

        //
        // Résumé :
        //     Determines whether two specified MongoDBRef objects have different values.
        //
        // Paramètres :
        //   lhs:
        //     The first value to compare, or null.
        //
        //   rhs:
        //     The second value to compare, or null.
        //
        // Retourne :
        //     True if the value of lhs is different from the value of rhs; otherwise, false.
        public static bool operator !=(CustomMongoDBRef lhs, CustomMongoDBRef rhs)
        {
            return !Equals(lhs, rhs);
        }

        //
        // Résumé :
        //     Determines whether two specified MongoDBRef objects have the same value.
        //
        // Paramètres :
        //   lhs:
        //     The first value to compare, or null.
        //
        //   rhs:
        //     The second value to compare, or null.
        //
        // Retourne :
        //     True if the value of lhs is the same as the value of rhs; otherwise, false.
        public static bool operator ==(CustomMongoDBRef lhs, CustomMongoDBRef rhs)
        {
            return Equals(lhs, rhs);
        }

        //
        // Résumé :
        //     Determines whether two specified MongoDBRef objects have the same value.
        //
        // Paramètres :
        //   lhs:
        //     The first value to compare, or null.
        //
        //   rhs:
        //     The second value to compare, or null.
        //
        // Retourne :
        //     True if the value of lhs is the same as the value of rhs; otherwise, false.
        public static bool Equals(CustomMongoDBRef lhs, CustomMongoDBRef rhs)
        {
            return lhs?.Equals(rhs) ?? ((object)rhs == null);
        }

        //
        // Résumé :
        //     Determines whether this instance and another specified MongoDBRef object have
        //     the same value.
        //
        // Paramètres :
        //   rhs:
        //     The MongoDBRef object to compare to this instance.
        //
        // Retourne :
        //     True if the value of the rhs parameter is the same as this instance; otherwise,
        //     false.
        public bool Equals(CustomMongoDBRef rhs)
        {
            if ((object)rhs == null || GetType() != rhs.GetType())
            {
                return false;
            }

            if ((object)this == rhs)
            {
                return true;
            }

            if (string.Equals(_databaseName, rhs._databaseName) && _collectionName.Equals(rhs._collectionName))
            {
                return _id.Equals(rhs._id);
            }

            return false;
        }

        //
        // Résumé :
        //     Determines whether this instance and a specified object, which must also be a
        //     MongoDBRef object, have the same value.
        //
        // Paramètres :
        //   obj:
        //     The MongoDBRef object to compare to this instance.
        //
        // Retourne :
        //     True if obj is a MongoDBRef object and its value is the same as this instance;
        //     otherwise, false.
        public override bool Equals(object obj)
        {
            return Equals(obj as CustomMongoDBRef);
        }

        //
        // Résumé :
        //     Returns the hash code for this MongoDBRef object.
        //
        // Retourne :
        //     A 32-bit signed integer hash code.
        public override int GetHashCode()
        {
            int num = 17;
            num = 37 * num + ((_databaseName != null) ? _databaseName.GetHashCode() : 0);
            num = 37 * num + _collectionName.GetHashCode();
            return 37 * num + _id.GetHashCode();
        }

        //
        // Résumé :
        //     Returns a string representation of the value.
        //
        // Retourne :
        //     A string representation of the value.
        public override string ToString()
        {
            if (_databaseName == null)
            {
                return $"new MongoDBRef(\"{_collectionName}\", {_id})";
            }

            return $"new MongoDBRef(\"{_databaseName}\", \"{_collectionName}\", {_id})";
        }
    }
}
