using FormBuilder.Components.Workflow;
using FormBuilder.Link;
using FormBuilder.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Text.Json.Nodes;

namespace FormBuilder.Services;

public interface IFormBuilderJsService
{
    Task<Coordinate> GetPosition(ElementReference eltRef);
    Task<Coordinate> GetOffsetPosition(ElementReference eltRef);
    Task<Size> GetSize(ElementReference eltRef);
    Task<Coordinate> GetPointInSvgSpace(double clientX, double clientY, ElementReference svgEltRef);
    Task Navigate(string url);
    Task NavigateForce(string url);
    Task SubmitForm(string url, JsonObject data, HttpMethods method);
    Task RefreshCss(string id, string href);
}

public class FormBuilderJsService : IFormBuilderJsService
{
    private Dictionary<HttpMethods, string> _httpMethodToName = new Dictionary<HttpMethods, string>
    {
        {  HttpMethods.GET, "get" },
        { HttpMethods.POST, "post" }
    };
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

    public async Task SubmitForm(string url, JsonObject data, HttpMethods method)
    {
        await _jsRuntime.InvokeVoidAsync("FormBuilder.submitForm", url, data, _httpMethodToName[method]);
    }

    public async Task RefreshCss(string id,string css)
    {
        await _jsRuntime.InvokeVoidAsync("FormBuilder.refreshCss", id, css);
    }
}
