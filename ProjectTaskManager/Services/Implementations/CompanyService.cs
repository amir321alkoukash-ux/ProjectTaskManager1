using AutoMapper;
using ProjectTaskManager.DTOs;
using ProjectTaskManager.Entities;
using ProjectTaskManager.Data.Repositories.Interfaces;
using ProjectTaskManager.Services.Interfaces;

namespace ProjectTaskManager.Services.Implementations
{
    public class CompanyService : ICompanyService
    {
        private readonly ICompanyRepository _companyRepository;
        private readonly IMapper _mapper;

        public CompanyService(ICompanyRepository companyRepository, IMapper mapper)
        {
            _companyRepository = companyRepository;
            _mapper = mapper;
        }

        public async Task<CompanyDto> GetCompanyByIdAsync(int id)
        {
            var company = await _companyRepository.GetCompanyWithUsersAsync(id);
            if (company == null) throw new KeyNotFoundException("Company not found");
            return _mapper.Map<CompanyDto>(company);
        }

        public async Task<IEnumerable<CompanyDto>> GetAllCompaniesAsync()
        {
            var companies = await _companyRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<CompanyDto>>(companies);
        }

        public async Task<CompanyDto> CreateCompanyAsync(CompanyDto companyDto)
        {
            var company = _mapper.Map<Company>(companyDto);
            await _companyRepository.AddAsync(company);
            await _companyRepository.SaveChangesAsync();
            return _mapper.Map<CompanyDto>(company);
        }

        public async Task UpdateCompanyAsync(int id, CompanyDto companyDto)
        {
            var company = await _companyRepository.GetByIdAsync(id);
            if (company == null) throw new KeyNotFoundException("Company not found");
            _mapper.Map(companyDto, company);
            _companyRepository.Update(company);
            await _companyRepository.SaveChangesAsync();
        }

        public async Task DeleteCompanyAsync(int id)
        {
            var company = await _companyRepository.GetByIdAsync(id);
            if (company == null) throw new KeyNotFoundException("Company not found");

            // Soft delete by setting InactiveDate
            company.InactiveDate = DateTime.UtcNow;
            _companyRepository.Update(company);
            await _companyRepository.SaveChangesAsync();
        }

        public async Task<IEnumerable<CompanyExportDto>> GetAllForExportAsync()
        {
            var companies = await _companyRepository.GetAllAsync();

            return companies.Select(c => new CompanyExportDto
            {
                Id = c.Id,
                Name = c.Name,
                Email = c.Email,
                Location = c.Location,
                ProjectCount = c.Projects?.Count ?? 0,
                UserCount = c.Users?.Count ?? 0,
                CreatedAt = c.CreatedAt
            }).ToList();
        }
    }
}