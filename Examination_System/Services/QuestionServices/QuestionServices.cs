using AutoMapper;
using AutoMapper.QueryableExtensions;
using Examination_System.DTOs.Question;
using Examination_System.Models;
using Examination_System.Repository.UnitOfWork;
using Examination_System.Services.CourseServices;
using Examination_System.Specifications.SpecsForEntity;
using Microsoft.EntityFrameworkCore;

namespace Examination_System.Services.QuestionServices
{
    public class QuestionServices : IQuestionServices
    {
        readonly IUnitOfWork _unitOfWork;
        private readonly ICourseServices _courseServices;
        private readonly IMapper _mapper;

        public QuestionServices(IUnitOfWork unitOFWork,
            ICourseServices courseServices,
            IMapper Mapper)
        {
            _unitOfWork = unitOFWork;
            _courseServices = courseServices;
            _mapper = Mapper;
        }

      

        public async Task<QuestionToReturnDto> CreateQuestionAsync (CreateQuestionDto createQuestionDto)
        {
           // var transaction = await _unitOfWork.BeginTransactionAsync();
            try
            {
                if (!await _courseServices.IsExistsAsync(createQuestionDto.CourseId))
                    throw new KeyNotFoundException($"Course with ID {createQuestionDto.CourseId} not found");
                var question = _mapper.Map<CreateQuestionDto, Question>(createQuestionDto);
                await _unitOfWork.Repository<Question, int>().AddAsync(question);
                await _unitOfWork.CompleteAsync();
               /* var Choices = _unitOfWork.Repository<Choice, int>();
                foreach (var choice in question.Choices)
                {
                    choice.QuestionId = question.Id;
                    await Choices.AddAsync(choice);
                }*/
                //await _unitOfWork.CompleteAsync();
                //await transaction.CommitAsync();
                var questionToReturnDto = _mapper.Map<Question, QuestionToReturnDto>(question);
                return questionToReturnDto;
            }
            catch (Exception e)
            {
               // await transaction.RollbackAsync();
                throw new Exception("can not add new Question" ,e );
            }
       

        }

        public async Task<IEnumerable<QuestionToReturnDto>> GetQuestionsByInstructorAndCourseAsync(string? instructorId, int? CourseId)
        {
            var QuestionsSpecs = new QuestionSpecifications(c=>c.InstructorId ==  instructorId &&  c.CourseId == CourseId);
            var questions = _unitOfWork.Repository<Question, int>().GetAllWithSpecificationAsync(QuestionsSpecs);
      
            var questionToReturnDto =await  questions.ProjectTo<QuestionToReturnDto>(_mapper.ConfigurationProvider).ToListAsync();
            if (questionToReturnDto == null || !questions.Any())
                throw new KeyNotFoundException("No questions found for the given instructor for this course");
            return questionToReturnDto;
        }

        public async Task<IEnumerable<QuestionToReturnDto>> GetQuestionsByInstructorAsync(string? instructorId)
        {
            var QuestionsSpecs = new QuestionSpecifications(c=>c.InstructorId == instructorId);
            var questions = _unitOfWork.Repository<Question, int>().GetAllWithSpecificationAsync(QuestionsSpecs);

            var questionToReturnDto = await questions.ProjectTo<QuestionToReturnDto>(_mapper.ConfigurationProvider).ToListAsync();
            if (questionToReturnDto == null || !questions.Any())
                throw new KeyNotFoundException("No questions found for the given instructor for this course");
            return questionToReturnDto;
           // throw new NotImplementedException();
        }
    }
}
