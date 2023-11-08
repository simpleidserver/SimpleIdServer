namespace SimpleIdServer.IdServer.ExternalEvents
{
    public class AddCertificateAuthoritySuccessEvent : IExternalEvent
    {
        public string EventName => nameof(AddCertificateAuthoritySuccessEvent);
        public string Realm { get; set; }
        public string SubjectName { get; set; }
    }
}
