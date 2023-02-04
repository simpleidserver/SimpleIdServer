// Copyright (c) SimpleIdServer. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.
using Fluxor;
using Microsoft.EntityFrameworkCore;
using Radzen;
using SimpleIdServer.IdServer.Domains;
using SimpleIdServer.IdServer.Store;
using SimpleIdServer.IdServer.Website.Resources;
using System.Linq.Dynamic.Core;

namespace SimpleIdServer.IdServer.Website.Stores.UserStore
{
    public class UserEffects
    {
        private readonly IUserRepository _userRepository;

        public UserEffects(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [EffectMethod]
        public async Task Handle(SearchUsersAction action, IDispatcher dispatcher)
        {
            IQueryable<User> query = _userRepository.Query().Include(u => u.OAuthUserClaims).AsNoTracking();
            if (!string.IsNullOrWhiteSpace(action.Filter))
                query = query.Where(SanitizeExpression(action.Filter));

            if (!string.IsNullOrWhiteSpace(action.OrderBy))
                query = query.OrderBy(SanitizeExpression(action.OrderBy));

            var users = await query.Skip(action.Skip.Value).Take(action.Take.Value).ToListAsync(CancellationToken.None);
            dispatcher.Dispatch(new SearchUsersSuccessAction { Users = users });

            string SanitizeExpression(string expression) => expression.Replace("Value.", "");
        }

        [EffectMethod]
        public async Task Handle(GetUserAction action, IDispatcher dispatcher)
        {
            var user = await _userRepository.Query().Include(u => u.OAuthUserClaims).Include(u => u.Consents).Include(u => u.Sessions).Include(u => u.ExternalAuthProviders).AsNoTracking().SingleOrDefaultAsync(a => a.Id == action.UserId);
            if (user == null) {
                dispatcher.Dispatch(new GetUserFailureAction { ErrorMessage = string.Format(Global.UnknownUser, action.UserId) });
                return;
            }

            dispatcher.Dispatch(new GetUserSuccessAction { User = user });
        }

        [EffectMethod]
        public async Task Handle(UpdateUserDetailsAction action, IDispatcher dispatcher)
        {
            var user = await _userRepository.Query().Include(u => u.OAuthUserClaims).SingleOrDefaultAsync(a => a.Id == action.UserId);
            user.UpdateEmail(action.Email);
            user.UpdateName(action.Firstname);
            user.UpdateLastname(action.Lastname);
            user.UpdateDateTime = DateTime.UtcNow;
            await _userRepository.SaveChanges(CancellationToken.None);
            dispatcher.Dispatch(new UpdateUserDetailsSuccessAction { Email = action.Email, Firstname = action.Firstname, Lastname = action.Lastname, UserId = action.UserId });
        }

        [EffectMethod]
        public async Task Handle(RevokeUserConsentAction action, IDispatcher dispatcher)
        {
            var user = await _userRepository.Query().Include(u => u.Consents).SingleAsync(a => a.Id == action.UserId);
            var consent = user.Consents.Single(c => c.Id == action.ConsentId);
            user.Consents.Remove(consent);
            await _userRepository.SaveChanges(CancellationToken.None);
            dispatcher.Dispatch(new RevokeUserConsentSuccessAction { ConsentId = action.ConsentId, UserId = action.UserId });
        }

        [EffectMethod]
        public async Task Handle(UnlinkExternalAuthProviderAction action, IDispatcher dispatcher)
        {
            var user = await _userRepository.Query().Include(u => u.ExternalAuthProviders).SingleAsync(a => a.Id == action.UserId);
            var externalAuthProvider = user.ExternalAuthProviders.Single(c => c.Scheme == action.Scheme && c.Subject == action.Subject) ;
            user.ExternalAuthProviders.Remove(externalAuthProvider);
            await _userRepository.SaveChanges(CancellationToken.None);
            dispatcher.Dispatch(new UnlinkExternalAuthProviderSuccessAction { Scheme = action.Scheme, Subject = action.Subject, UserId = action.UserId });
        }

        [EffectMethod]
        public async Task Handle(RevokeUserSessionAction action, IDispatcher dispatcher)
        {
            var user = await _userRepository.Query().Include(u => u.Sessions).SingleAsync(a => a.Id == action.UserId);
            var session = user.Sessions.Single(s => s.SessionId == action.SessionId);
            session.State = UserSessionStates.Rejected;
            await _userRepository.SaveChanges(CancellationToken.None);
            dispatcher.Dispatch(new RevokeUserSessionSuccessAction { SessionId = action.SessionId, UserId = action.UserId });
        }
    }

    public class SearchUsersAction
    {
        public string? Filter { get; set; } = null;
        public string? OrderBy { get; set; } = null;
        public int? Skip { get; set; } = null;
        public int? Take { get; set; } = null;
    }

    public class SearchUsersSuccessAction
    {
        public IEnumerable<User> Users { get; set; } = new List<User>();
    }

    public class ToggleUserSelectionAction
    {
        public bool IsSelected { get; set; } = false;
        public string UserId { get; set; } = null!;
    }

    public class ToggleAllUserSelectionAction
    {
        public bool IsSelected { get; set; } = false;
    }

    public class GetUserAction
    {
        public string UserId { get; set; } = null!;
    }

    public class GetUserSuccessAction
    {
        public User User { get; set; } = null!;
    }

    public class GetUserFailureAction
    {
        public string UserId { get; set; } = null!;
        public string ErrorMessage { get; set; } = null!;
    }

    public class UpdateUserDetailsAction
    {
        public string UserId { get; set; } = null!;
        public string? Email { get; set; } = null;
        public string? Firstname { get; set; } = null;
        public string? Lastname { get; set; } = null;
    }

    public class UpdateUserDetailsSuccessAction
    {
        public string UserId { get; set; } = null!;
        public string? Email { get; set; } = null;
        public string? Firstname { get; set; } = null;
        public string? Lastname { get; set; } = null;
    }

    public class RevokeUserConsentAction
    {
        public string UserId { get; set; } = null!;
        public string ConsentId { get; set; } = null!;
    }

    public class RevokeUserConsentSuccessAction
    {
        public string UserId { get; set; } = null!;
        public string ConsentId { get; set; } = null!;
    }

    public class UnlinkExternalAuthProviderAction
    {
        public string UserId { get; set; } = null!;
        public string Subject { get; set; } = null!;
        public string Scheme { get; set; } = null!;
    }

    public class UnlinkExternalAuthProviderSuccessAction
    {
        public string UserId { get; set; } = null!;
        public string Subject { get; set; } = null!;
        public string Scheme { get; set; } = null!;
    }

    public class RevokeUserSessionAction
    {
        public string UserId { get; set; } = null!;
        public string SessionId { get; set; } = null!;
    }

    public class RevokeUserSessionSuccessAction
    {
        public string UserId { get; set; } = null!;
        public string SessionId { get; set; } = null!;
    }
}
