using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using AppForSEII.API.Data;
using AppForSEII.API.Models;
using AppForSEII.API.DTOs.Pieza3DDTOs;

namespace AppForSEII.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class Piezas3DController : ControllerBase
    {
        private readonly ApplicationDbContext lecturaDbContext;
        private readonly ILogger<Piezas3DController> logger1;
        public Piezas3DController(ApplicationDbContext appDbContext, ILogger<Piezas3DController> Logger)
        {
            lecturaDbContext = appDbContext;
            logger1 = Logger;
        }

        // Método GET

        [HttpGet]
        [Route("GetPiezas3D")]
        [ProducesResponseType(typeof(IList<Pieza3DForEncargoDTO>), (int)HttpStatusCode.OK)]

        public async Task<ActionResult<IList<Pieza3DForEncargoDTO>>> GetPiezas3D(string? nombre, CategoriaPieza? categoria)
        {
            
        }
        
    }
}
