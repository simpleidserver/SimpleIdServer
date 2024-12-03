using Microsoft.AspNetCore.Components;
using System.Text.Json.Serialization;

namespace FormBuilder.Models;

public class WorkflowStep
{
    public string Id { get; set; }
    public string FormRecordName { get; set; }
    public Coordinate Coordinate { get; set; }
    [JsonIgnore]
    public ElementReference EltRef { get; set; }
    [JsonIgnore]
    public Size Size { get; set; }
}