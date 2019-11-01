// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using ProtectAPIWithClientCredentialsGrantType.Api.Domains;
using ProtectAPIWithClientCredentialsGrantType.Api.Persistence;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace ProtectAPIWithClientCredentialsGrantType.Api.Controllers
{
    [Route("users")]
    public class UsersController : Controller
    {
        private readonly IUserRepository _userRepository;

        public UsersController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [Authorize("GetUser")]
        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var identifiers = (await _userRepository.FindAllUsers()).Select(u => u.Id);
            return new OkObjectResult(identifiers);
        }

        [Authorize("GetUser")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(string id)
        {
            var user = await _userRepository.FindUserByIdentifier(id);
            if (user == null)
            {
                return new NotFoundResult();
            }

            return ToDto(user);
        }

        [Authorize("AddUser")]
        [HttpPost]
        public async Task<IActionResult> AddUser([FromBody] JObject request)
        {
            var user = ToDomain(request);
            _userRepository.AddUser(user);
            await _userRepository.SaveChanges();
            return new ContentResult
            {
                StatusCode = (int)HttpStatusCode.Created,
                ContentType = "application/json",
                Content = new JObject
                {
                    { "_id", user.Id }
                }.ToString()
            };
        }

        public static IActionResult ToDto(User user)
        {
            return new OkObjectResult(new JObject
            {
                { "_id", user.Id },
                { "firstname", user.FirstName },
                { "lastname", user.LastName }
            });
        }

        public static User ToDomain(JObject parameter)
        {
            return new User(Guid.NewGuid().ToString())
            {
                FirstName = parameter["firstname"].ToString(),
                LastName = parameter["lastname"].ToString()
            };
        }
    }
}
