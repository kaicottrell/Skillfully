﻿using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using Urava.Server.Documents;
using Urava.Server.Interfaces;
using Urava.Server.Repository;

namespace Urava.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class JobPostingController : ControllerBase
    {
        private readonly IRepository<JobPosting> _jobPostingRepo;

        public JobPostingController(IRepository<JobPosting> jobPostingRepo)
        {
            _jobPostingRepo = jobPostingRepo;
        }

        // Create a new job posting
        [HttpPost]
        public IActionResult CreateJobPosting([FromBody] JobPosting jobPosting)
        {
            if (jobPosting == null)
            {
                return BadRequest("Job posting is null.");
            }

            _jobPostingRepo.Add(jobPosting);
            _jobPostingRepo.SaveChanges();

            return CreatedAtAction(nameof(GetJobPostingById), new { id = jobPosting._id }, jobPosting);
        }

        // Get a job posting by ID
        [HttpGet]
        public async Task<IActionResult> GetJobPostingById(ObjectId id)
        {
            var jobPosting = await _jobPostingRepo.GetById(id);

            if (jobPosting == null)
            {
                return NotFound();
            }

            return Ok(jobPosting);
        }
        // Get all job postings
        [HttpGet("GetAllJobPostings")]
        public async Task<IActionResult> GetAllJobPostings()
        {
            var jobPostings = await _jobPostingRepo.GetAll();
            return Ok(jobPostings.ToArray());
        }

        // Update an existing job posting
        [HttpPut]
        public  IActionResult UpdateJobPosting([FromBody] JobPosting jobPosting)
        {
            if (jobPosting == null)
            {
                return BadRequest("Job posting is null or ID mismatch.");
            }

            var existingJobPosting = _jobPostingRepo.GetById(jobPosting._id);
            if (existingJobPosting == null)
            {
                return NotFound();
            }

            _jobPostingRepo.Update(jobPosting);

            return NoContent();
        }

        // Delete a job posting
        [HttpDelete("{id}")]
        public IActionResult DeleteJobPosting(string id)
        {
            if (!ObjectId.TryParse(id, out var objectId))
            {
                return BadRequest("Invalid ID format.");
            }


            var jobPosting = _jobPostingRepo.GetById(objectId);
            if (jobPosting == null)
            {
                return NotFound();
            }

            _jobPostingRepo.Remove(objectId);

            return NoContent();
        }
    }
}
