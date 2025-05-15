using EmployeesPortal.Data;
using EmployeesPortal.Models;
using MeetingService.DTOs;
using MeetingService.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;

namespace MeetingService.Repositories
{
    public class MeetingRepository : IMeetingRepository
    {
        private readonly ApplicationDbContext _context;

        public MeetingRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Meeting meeting)
        {
            _context.Meetings.Add(meeting);
            await _context.SaveChangesAsync();
        }

        public async Task AddParticipantAsync(MeetingParticipant participant)
        {
            _context.MeetingParticipants.Add(participant);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveParticipantAsync(int meetingId, int employeeId)
        {
            var participant = await _context.MeetingParticipants
                .FirstOrDefaultAsync(mp => mp.MeetingID == meetingId && mp.EmployeeID == employeeId);

            if (participant != null)
            {
                _context.MeetingParticipants.Remove(participant);
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteAsync(int meetingId)
        {
            var meeting = await _context.Meetings.FindAsync(meetingId);
            if (meeting != null)
            {
                _context.Meetings.Remove(meeting);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<MeetingDTO>> GetAllMeetingsAsync()
        {
            var meetings = await _context.Meetings
                .Include(m => m.MeetingParticipants)
                .ThenInclude(mp => mp.Employee).ThenInclude(e => e.ApplicationUser)
                .ToListAsync();

            var meetingDtos = meetings.Select(meeting => new MeetingDTO
            {
                MeetingId = meeting.MeetingId,
                StartDate = meeting.StartDate,
                EndDate = meeting.EndDate,
                VirtualMeeting = meeting.VirtualMeeting,
                Employee = meeting.MeetingParticipants.Select(mp =>new EmployeeDTO 
                {
                    Id= mp.EmployeeID,
                    Name=mp.Employee.ApplicationUser.Name,
                    IsOff=mp.IsOff,
                } ).ToList(),
            }).ToList();

            return meetingDtos;
        }


        public async Task<Meeting> GetByIdAsync(int meetingId)
        {
            return await _context.Meetings
                .Include(m => m.MeetingParticipants)
                .FirstOrDefaultAsync(m => m.MeetingId == meetingId);
        }
        public async Task<IEnumerable<MeetingParticipant>> GetMeetingParticipantAsync(int employeeId)
        {
            var apps =await _context.MeetingParticipants.Include(mp=>mp.Meeting)
                .Where(mp => mp.EmployeeID == employeeId).ToListAsync();
            return apps;
        }

        public async Task UpdateMeetingAsync(int meetingId)
        {
            var meeting = await _context.Meetings.FindAsync(meetingId);
            if (meeting != null)
            {
                _context.Meetings.Update(meeting);
                await _context.SaveChangesAsync();
            }
        }

        public async Task UpdateMeetingParticpantAsync(MeetingParticipant meetingParticipant)
        {
            if (meetingParticipant != null)
            {
                _context.MeetingParticipants.Update(meetingParticipant);
                await _context.SaveChangesAsync();
            }
        }
    }

}
