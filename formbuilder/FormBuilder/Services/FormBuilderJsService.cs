using FormBuilder.Components.Workflow;
using FormBuilder.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace FormBuilder.Services;

public interface IFormBuilderJsService
{
    Task<Coordinate> GetPosition(ElementReference eltRef);
    Task<Coordinate> GetOffsetPosition(ElementReference eltRef);
    Task<Size> GetSize(ElementReference eltRef);
    Task<Coordinate> GetPointInSvgSpace(double clientX, double clientY, ElementReference svgEltRef);
    Task Navigate(string url);
    Task NavigateForce(string url);
}

public class FormBuilderJsService : IFormBuilderJsService
{
    private readonly IJSRuntime _jsRuntime;

    public FormBuilderJsService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public async Task<Coordinate> GetPosition(ElementReference eltRef)
    {
        var result = await _jsRuntime.InvokeAsync<Coordinate>("FormBuilder.getPosition", eltRef);
        result.Round();
        return result;
    }

    public async Task<Coordinate> GetOffsetPosition(ElementReference eltRef)
    {
        var result = await _jsRuntime.InvokeAsync<Coordinate>("FormBuilder.getOffsetPosition", eltRef);
        result.Round();
        return result;
    }
        
    public async Task<Size> GetSize(ElementReference eltRef)
    {
        var result = await _jsRuntime.InvokeAsync<Size>("FormBuilder.getSize", eltRef);
        return result;
    }


    public async Task<Coordinate> GetPointInSvgSpace(double clientX, double clientY, ElementReference svgEltRef)
    {
        var result = await _jsRuntime.InvokeAsync<Coordinate>("FormBuilder.getPointInSvgSpace", clientX, clientY, svgEltRef);
        result.Round();
        return result;
    }

    public async Task Navigate(string url)
        => await _jsRuntime.InvokeVoidAsync("FormBuilder.navigate", url);

    public async Task NavigateForce(string url)
        => await _jsRuntime.InvokeVoidAsync("FormBuilder.navigateForce", url);
}
