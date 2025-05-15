using EmployeesPortal.Models;
using MeetingService.DTOs;

namespace MeetingService.Interfaces
{
    public interface IMeetingRepository
    {
        Task AddAsync(Meeting meeting);
        Task AddParticipantAsync(MeetingParticipant participant);
        Task RemoveParticipantAsync(int meetingId, int employeeId);
        Task DeleteAsync(int meetingId);
        Task<List<MeetingDTO>> GetAllMeetingsAsync();
        Task<Meeting> GetByIdAsync(int meetingId);
        Task<IEnumerable<MeetingParticipant>> GetMeetingParticipantAsync(int employeeId);
        Task  UpdateMeetingAsync(int meetingId);
        Task UpdateMeetingParticpantAsync(MeetingParticipant meetingParticipant);

    }
}
