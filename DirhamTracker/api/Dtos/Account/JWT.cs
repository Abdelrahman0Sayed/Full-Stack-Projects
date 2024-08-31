using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Net.Http.Headers;

namespace api.Dtos.Account
{
    public class JWT
    {
        public string? Issuer {get; set;}
        public string? Audience {get; set;}
        public DateTime DurationInMinutes {get; set;} = DateTime.Now.AddMinutes(10);
        public string? Key {get; set;}
    }
}