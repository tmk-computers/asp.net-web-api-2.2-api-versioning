using AutoMapper;
using Microsoft.Web.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using TheCodeCamp.Data;
using TheCodeCamp.Models;

namespace TheCodeCamp.Controllers
{
  [ApiVersion("1.0")]
  [ApiVersion("1.1")]
  [RoutePrefix("api/v{version:apiVersion}/camps")]
  public class CampsController : ApiController
  {
    private readonly ICampRepository _repository;
    private readonly IMapper _mapper;

    public CampsController(ICampRepository repository, IMapper mapper)
    {
      _repository = repository;
      _mapper = mapper;
    }

    [Route()]
    public async Task<IHttpActionResult> Get(bool includeTalks = false)
    {
      try
      {
        var result = await _repository.GetAllCampsAsync(includeTalks);

        // Mapping 
        var mappedResult = _mapper.Map<IEnumerable<CampModel>>(result);

        return Ok(mappedResult);
      }
      catch (Exception ex)
      {
        // TODO Add Logging
        return InternalServerError(ex);
      }
    }

    [MapToApiVersion("1.0")]
    [Route("{moniker}", Name = "GetCamp")]
    public async Task<IHttpActionResult> Get(string moniker, bool includeTalks = false)
    {
      try
      {
        var result = await _repository.GetCampAsync(moniker, includeTalks);
        if (result == null) return NotFound();

        return Ok(_mapper.Map<CampModel>(result));
      }
      catch (Exception ex)
      {
        return InternalServerError(ex);
      }
    }

    [MapToApiVersion("1.1")]
    [Route("{moniker}", Name = "GetCamp11")]
    public async Task<IHttpActionResult> Get(string moniker)
    {
      try
      {
        var result = await _repository.GetCampAsync(moniker, true);
        if (result == null) return NotFound();

        return Ok(_mapper.Map<CampModel>(result));
      }
      catch (Exception ex)
      {
        return InternalServerError(ex);
      }
    }

    [Route("searchByDate/{eventDate:datetime}")]
    [HttpGet]
    public async Task<IHttpActionResult> SearchByEventDate(DateTime eventDate, bool includeTalks = false)
    {
      try
      {
        var result = await _repository.GetAllCampsByEventDate(eventDate, includeTalks);

        return Ok(_mapper.Map<CampModel[]>(result));

      }
      catch (Exception ex)
      {
        return InternalServerError(ex);
      }
    }

    [Route()]
    public async Task<IHttpActionResult> Post(CampModel model)
    {
      try
      {
        if (await _repository.GetCampAsync(model.Moniker) != null)
        {
          ModelState.AddModelError("Moniker", "Moniker in use");
        }

        if (ModelState.IsValid)
        {
          var camp = _mapper.Map<Camp>(model);

          _repository.AddCamp(camp);

          if (await _repository.SaveChangesAsync())
          {
            var newModel = _mapper.Map<CampModel>(camp);

            return CreatedAtRoute("GetCamp", new { moniker = newModel.Moniker }, newModel);
          }
        }
      }
      catch (Exception ex)
      {
        return InternalServerError(ex);
      }

      return BadRequest(ModelState);
    }

    [Route("{moniker}")]
    public async Task<IHttpActionResult> Put(string moniker, CampModel model)
    {
      try
      {
        var camp = await _repository.GetCampAsync(moniker);
        if (camp == null) return NotFound();

        _mapper.Map(model, camp);

        if (await _repository.SaveChangesAsync())
        {
          return Ok(_mapper.Map<CampModel>(camp));
        }
        else
        {
          return InternalServerError();
        }
      }
      catch (Exception ex)
      {
        return InternalServerError(ex);
      }
    }

    [Route("{moniker}")]
    public async Task<IHttpActionResult> Delete(string moniker)
    {
      try
      {
        var camp = await _repository.GetCampAsync(moniker);
        if (camp == null) return NotFound();

        _repository.DeleteCamp(camp);

        if (await _repository.SaveChangesAsync())
        {
          return Ok();
        }
        else
        {
          return InternalServerError();
        }
      }
      catch (Exception ex)
      {
        return InternalServerError(ex);
      }
    }
  }
}
