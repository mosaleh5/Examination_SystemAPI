using Examination_System.DTOs.Exam;
using Examination_System.Models;
using Examination_System.Mappers;
using Examination_System.Repository.UnitOfWork;
using Examination_System.Specifications.SpecsForEntity;
using Microsoft.AspNetCore.Mvc;

namespace Examination_System.Services
{
    public class ExamServices
    {
        private readonly IUnitOfWork _unitOfWork;

        public ExamServices(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ActionResult<IEnumerable<ExamDto>>> GetAllExamAsync()
        {
            var examSpecifications = new ExamSpecifications();  
            var exams = await _unitOfWork.Repository<Exam>().GetAllWithSpecificationAsync(examSpecifications);
            return exams;
        }

    }
}
