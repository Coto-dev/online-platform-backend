using Microsoft.AspNetCore.Http;

namespace HW.Common.Interfaces; 

public interface IParserService {
    public Task ParseFile(IFormFile file, Guid moduleId);
}