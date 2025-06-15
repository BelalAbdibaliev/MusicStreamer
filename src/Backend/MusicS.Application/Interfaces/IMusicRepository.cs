using MusicS.Domain.Entities;

namespace MusicS.Application.Interfaces;

public interface IMusicRepository
{
    Task<MusicInfo> GetInfo(string fileName);
    Task<IEnumerable<MusicInfo>> GetAll(int page, int pageSize);
    Task<IEnumerable<MusicInfo>> Search(string pattern);
    Task Add(MusicInfo music);
    Task Update(MusicInfo music);
    Task Delete(MusicInfo music);
}