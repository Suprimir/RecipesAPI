using AutoMapper;
using RecipesAPI.DTOs;
using RecipesAPI.Models;
using RecipesAPI.Repositories;

namespace RecipesAPI.Services
{
    public class TagService : ITagService
    {
        private readonly ITagRepository _repository;
        private readonly IMapper _mapper;
        private readonly ILogger<TagService> _logger;

        public TagService(ITagRepository repository, IMapper mapper, ILogger<TagService> logger)
        {
            _repository = repository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<TagDTO>> GetAllTagsAsync()
        {
            _logger.LogInformation("Obteniendo todas las etiquetas");
            var tags = await _repository.GetAllAsync();
            return _mapper.Map<IEnumerable<TagDTO>>(tags);
        }

        public async Task<TagDTO?> GetTagByIdAsync(int id)
        {
            _logger.LogInformation("Obteniendo etiqueta con ID: {Id}", id);
            var tag = await _repository.GetByIdAsync(id);
            return tag != null ? _mapper.Map<TagDTO>(tag) : null;
        }

        public async Task<TagDTO> CreateTagAsync(CreateTagDTO createDto)
        {
            _logger.LogInformation("Creando nueva etiqueta: {Name}", createDto.Name);
            var tag = _mapper.Map<Tag>(createDto);
            var createdTag = await _repository.AddAsync(tag);
            return _mapper.Map<TagDTO>(createdTag);
        }

        public async Task<bool> UpdateTagAsync(int id, UpdateTagDTO updateDto)
        {
            _logger.LogInformation("Actualizando etiqueta con ID: {Id}", id);

            var tag = await _repository.GetByIdAsync(id);
            if (tag == null)
            {
                return false;
            }

            _mapper.Map(updateDto, tag);
            tag.Id = id;

            await _repository.UpdateAsync(tag);
            return true;
        }

        public async Task<bool> DeleteTagAsync(int id)
        {
            _logger.LogInformation("Eliminando etiqueta con ID: {Id}", id);
            return await _repository.DeleteAsync(id);
        }

        public async Task<bool> TagExistsAsync(int id)
        {
            return await _repository.ExistsAsync(id);
        }
    }
}