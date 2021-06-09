using CoreLib.Requests;
using Refit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientApplication.Services
{
    public interface IRegisterService
    {
        [Post("/register")]
        Task Register([Body] RegisterRequest registerRequest);
    }
}
