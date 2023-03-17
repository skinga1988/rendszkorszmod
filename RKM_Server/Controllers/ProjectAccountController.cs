using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using RKM_Server.DTO;
using RKM_Server.Interfaces;
using RKM_Server.Models;

namespace RKM_Server.Controllers
{
    
        [Route("api/[controller]")]
        [ApiController]
        public class ProjectAccountController : Controller
        {
            private readonly ProjectAccountInterface _projectAccountInterface;
            private readonly IMapper _mapper;

            public ProjectAccountController(ProjectAccountInterface projectAccountInterface, IMapper mapper)
            {
                _projectAccountInterface = projectAccountInterface;
                _mapper = mapper;
            }

            [HttpGet]
            [ProducesResponseType(200, Type = typeof(IEnumerable<ProjectAccount>))]
            public IActionResult GetProjectAccount()
            {
                var projectAccounts = _mapper.Map<List<ProjectAccountDto>>(_projectAccountInterface.GetProjectAccounts());

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                return Ok(projectAccounts);
            }

            [HttpGet("{id}")]
            [ProducesResponseType(200, Type = typeof(ProjectAccount))]
            [ProducesResponseType(400)]
            public IActionResult GetProjectAccount(int id)
            {
                if (!_projectAccountInterface.ProjectAccountExist(id))
                    return NotFound();

                var projectAccounts = _mapper.Map<ProjectAccountDto>(_projectAccountInterface.GetProjectAccount(id));

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                return Ok(projectAccounts);
            }

            [HttpPost]
            [ProducesResponseType(204)]
            [ProducesResponseType(400)]
            public IActionResult CreateProjectAccount([FromBody] ProjectAccountDto projectAccountCreate)
            {
                if (projectAccountCreate == null)
                    return BadRequest(ModelState);



                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var projectAccountMap = _mapper.Map<ProjectAccount>(projectAccountCreate);


                if (!_projectAccountInterface.CreateProjectAccount(projectAccountMap))
                {
                    ModelState.AddModelError("", "Something went wrong while savin");
                    return StatusCode(500, ModelState);
                }

                return Ok("Successfully created");
            }

            [HttpPut]
            [ProducesResponseType(400)]
            [ProducesResponseType(204)]
            [ProducesResponseType(404)]
            public IActionResult UpdateProjectAccount(int id, [FromBody] ProjectAccountDto updateProjectAccount)
            {
                if (UpdateProjectAccount == null)
                    return BadRequest(ModelState);

                if (!_projectAccountInterface.ProjectAccountExist(id))
                    return NotFound();

                var projectAccountMap = _mapper.Map<ProjectAccount>(updateProjectAccount);

                if (!_projectAccountInterface.UpdateProjectAccount(projectAccountMap))
                {
                    ModelState.AddModelError("", "Update Error");
                    return StatusCode(500, ModelState);
                }

                return NoContent();
            }

            [HttpDelete("{id}")]
            [ProducesResponseType(400)]
            [ProducesResponseType(204)]
            [ProducesResponseType(404)]
            public IActionResult DeleteProjectAccount(int id)
            {
                if (!_projectAccountInterface.ProjectAccountExist(id))
                {
                    return NotFound();
                }

                var projectAccountToDelete = _projectAccountInterface.GetProjectAccount(id);

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                if (!_projectAccountInterface.DeleteProjectAccount(projectAccountToDelete))
                {
                    ModelState.AddModelError("", "Delete Error");
                }

                return NoContent();
            }


        }
}
