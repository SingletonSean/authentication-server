using CoreLib.Responses;
using Refit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientApplication.Services
{
    public interface IDataService
    {
        [Get("/data")]
        Task<DataResponse> GetData();
    }
}
