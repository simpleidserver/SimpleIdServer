// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Mvc;
using SimpleIdServer.IdServer.DTOs;
using SimpleIdServer.IdServer.Resources;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json.Nodes;

namespace SimpleIdServer.IdServer.UI
{
    public class FormController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            var queryCollection = Request.Query;
            if (!queryCollection.ContainsKey("redirect_url"))
            {
                var jObj = new JsonObject
                {
                    { ErrorResponseParameters.Error, ErrorCodes.INVALID_REQUEST },
                    { ErrorResponseParameters.ErrorDescription, string.Format(Global.MissingParameter, "redirect_url") }
                };
                var payload = Encoding.UTF8.GetBytes(jObj.ToString());
                return new ContentResult
                {
                    ContentType = "application/json",
                    Content = jObj.ToString(),
                    StatusCode = (int)HttpStatusCode.BadRequest
                };
            }

            var inputs = string.Join(" ", queryCollection.Where(q => q.Key != "redirect_url").Select(q => $"<input type='hidden' value='{q.Value.First().ToString()}' name='{q.Key}' />"));
            var html = "<html>"+
                "<head><title> Submit This Form</title></head>"+
                "<body onload='javascript:document.forms[0].submit()'>"+
                    "<form method='post' action='"+queryCollection["redirect_url"].First().ToString()+"'>"+ inputs+ "</form>" +    
                "</body>"+
            "</html>";
            return new ContentResult
            {
                ContentType = "text/html",
                Content = html
            };
        }
    }
}
