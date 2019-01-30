using System;
using DotEasy.Rpc.Core.ApiGateway.OAuth;

namespace doteasy.rpc.webserver
{
    public class AuthorizationServerProvider : IAuthorizationServerProvider
    {
        public bool ValidateClientAuthentication(string token)
        {
            Console.WriteLine(token);
            return false;
        }

        public string GetPayloadString(string token)
        {
            throw new System.NotImplementedException();
        }
    }
}