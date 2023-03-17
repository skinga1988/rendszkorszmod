using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using RKM_Server.DTO;
using RKM_Server.Interfaces;
using RKM_Server.Models;

namespace RKM_Server.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class ProjectController : Controller
    {
        private readonly ProjectInterface _projectInterface;
        private readonly IMapper _mapper;

        public ProjectController(ProjectInterface projectInterface, IMapper mapper)
        {
            _projectInterface = projectInterface;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Project>))]
        public IActionResult GetProjects()
        {
            var projects = _mapper.Map<List<ProjectDto>>(_projectInterface.GetProjects());

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(projects);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(200, Type = typeof(Project))]
        [ProducesResponseType(400)]
        public IActionResult GetProjet(int id)
        {
            if (!_projectInterface.ProjectExist(id))
                return NotFound();

            var project = _mapper.Map<ProjectDto>(_projectInterface.GetProject(id));

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(project);
        }

        [HttpPost]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public IActionResult CreateProject([FromBody] ProjectDto projectCreate)
        {
            if (projectCreate == null)
                return BadRequest(ModelState);


            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var projectMap = _mapper.Map<Project>(projectCreate);


            if (!_projectInterface.CreateProject(projectMap))
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
        public IActionResult UpdateProject(int id, [FromBody] ProjectDto updateProject)
        {
            if (updateProject == null)
                return BadRequest(ModelState);

            if (!_projectInterface.ProjectExist(id))
                return NotFound();

            var projectMap = _mapper.Map<Project>(updateProject);

            if (!_projectInterface.UpdateProject(projectMap))
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
        public IActionResult DeleteProject(int id)
        {
            if (!_projectInterface.ProjectExist(id))
            {
                return NotFound();
            }

            var projectToDelete = _projectInterface.GetProject(id);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_projectInterface.DeleteProject(projectToDelete))
            {
                ModelState.AddModelError("", "Delete Error");
            }

            return NoContent();
        }


    }
}
