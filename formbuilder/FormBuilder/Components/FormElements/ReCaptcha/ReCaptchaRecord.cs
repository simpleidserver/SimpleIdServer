using FormBuilder.Models;
using System.Text.Json.Nodes;

namespace FormBuilder.Components.FormElements.ReCaptcha
{
    public class ReCaptchaRecord : IFormElementRecord
    {
        public string Id 
        { 
            get; set; 
        }

        public string CorrelationId
        {
            get; set;
        }

        public string Type => ReCaptchaDefinition.TYPE;

        public List<LabelTranslation> Labels
        {
            get; set;
        }

        public Dictionary<string, object> HtmlAttributes
        {
            get; set;
        }

        public string CssStyle
        {
            get; set;
        }

        public string SiteKey
        {
            get; set;
        }

        public string Value
        {
            get; set;
        }

        public void Apply(JsonNode node)
        {
        }

        public void ExtractJson(JsonObject json)
        {
            json.Add("CaptchaValue", Value);
            json.Add("CaptchaType", "V2ReCaptcha");
        }

        public IFormElementRecord GetChild(string id)
        {
            return null;
        }

        public IFormElementRecord GetChildByCorrelationId(string correlationId)
        {
            return null;
        }
    }
}
