namespace SimpleIdServer.Scim.Domain
{
    public enum SCIMSchemaAttributeMutabilities
    {
        /// <summary>
        /// The attribute SHALL NOT be modified.
        /// </summary>
        READONLY = 0,
        /// <summary>
        /// The attribute MAY be updated and read at any time. This is the default value.
        /// </summary>
        READWRITE = 1,
        /// <summary>
        /// The attribute MAY be defined at resource creation (e.g., POST) or at record replacement via a request (e.g., a PUT).  The attribute SHALL NOT be updated.
        /// </summary>
        IMMUTABLE = 2,
        /// <summary>
        /// The attribute MAY be updated at any time.  Attribute values SHALL NOT be returned (e.g., because the value is a stored hash).  Note: An attribute with a mutability of "writeOnly" usually also has a returned setting of "never".
        /// </summary>
        WRITEONLY = 3
    }
}