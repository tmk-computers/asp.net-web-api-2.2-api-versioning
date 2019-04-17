using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace TheCodeCamp.Controllers
{
  public class OperationsController : ApiController
  {
    [HttpOptions]
    [Route("api/v{version:apiVersion}/refreshconfig")]
    public IHttpActionResult RefreshAppSettings()
    {
      try
      {
        ConfigurationManager.RefreshSection("AppSettings");
        return Ok();
      }
      catch (Exception ex)
      {
        return InternalServerError(ex);
      }
    }
  }
}
