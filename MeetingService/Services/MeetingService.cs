using EmployeesPortal.Models;
using MeetingService.DTOs;
using MeetingService.Enums;
using MeetingService.Interfaces;
using MeetingService.IServices;

namespace MeetingService.Services
{
    public class MeetingService : IMeetingService
    {
        private readonly IScheduleRepository _scheduleRepository;
        private readonly IMeetingRepository _meetingRepository;

        public MeetingService(IScheduleRepository scheduleRepository, IMeetingRepository meetingRepository)
        {
            _scheduleRepository = scheduleRepository;
            _meetingRepository = meetingRepository;
        }

        // Recommendation endpoint
        public async Task<List<(DateTime Start, DateTime End)>> GetAvailableTimeSlots(List<int> employeeIds, DateTime intervalStart, DateTime intervalEnd, TimeSpan duration)
        {
            var allAppointments = await _scheduleRepository.GetAppointmentsForEmployeesInInterval(employeeIds, intervalStart, intervalEnd);
            var sortedAppointments = allAppointments.OrderBy(a => a.AppointmentDate).ThenBy(a => a.StartTime).ToList();

            var availableSlots = new List<(DateTime Start, DateTime End)>();
            DateTime workDayStart = intervalStart.Date.AddHours(9);
            DateTime workDayEnd = intervalStart.Date.AddHours(16);

            // Condition 1: From intervalStart to the first appointment
            if (sortedAppointments.Count > 0)
            {
                var firstAppointmentStart = sortedAppointments[0].AppointmentDate.Add(sortedAppointments[0].StartTime);

                if (intervalStart.Date == firstAppointmentStart.Date)
                {
                    if (firstAppointmentStart.Subtract(workDayStart) >= duration)
                    {
                        availableSlots.Add((workDayStart, firstAppointmentStart));
                    }
                }
                else
                {
                    // Loop through days from intervalStart to the day before the first appointment
                    for (var date = intervalStart.Date; date < firstAppointmentStart.Date; date = date.AddDays(1))
                    {
                        var dayStart = date.AddHours(9);
                        var dayEnd = date.AddHours(16);

                        if (dayEnd.Subtract(dayStart) >= duration)
                        {
                            availableSlots.Add((dayStart, dayEnd));
                        }
                    }

                    // Add the range from 9 AM to the first appointment on the day of the first appointment
                    if ((firstAppointmentStart.Date).AddHours(9).Subtract(firstAppointmentStart) >= duration)
                    {
                        availableSlots.Add((firstAppointmentStart.Date.AddHours(9), firstAppointmentStart));
                    }
                }
            }

            // Loop through sorted appointments and check gaps between them
            for (int i = 0; i < sortedAppointments.Count - 1; i++)
            {
                var currentStart = sortedAppointments[i].AppointmentDate.Add(sortedAppointments[i].StartTime);
                var currentEnd = sortedAppointments[i].AppointmentDate.Add(sortedAppointments[i].EndTime);
                var nextStart = sortedAppointments[i + 1].AppointmentDate.Add(sortedAppointments[i + 1].StartTime);

                // Condition 2: Same day appointments - check the gap between them
                if (currentEnd.Date == nextStart.Date)
                {
                    if (nextStart.Subtract(currentEnd) >= duration)
                    {
                        availableSlots.Add((currentEnd, nextStart));
                    }
                }
                else
                {
                    // Condition 3: Different day appointments - multiple ranges to handle
                    // Add from current appointment end to 4 PM on the current day
                    if ((currentEnd.Date).AddHours(16).Subtract(currentEnd) >= duration)
                    {
                        availableSlots.Add((currentEnd, currentEnd.Date.AddHours(16)));
                    }

                    // Loop through full days between the current appointment and the next appointment
                    for (var date = currentEnd.AddDays(1).Date; date < nextStart.Date; date = date.AddDays(1))
                    {
                        var dayStart = date.AddHours(9);
                        var dayEnd = date.AddHours(16);

                        if (dayEnd.Subtract(dayStart) >= duration)
                        {
                            availableSlots.Add((dayStart, dayEnd));
                        }
                    }

                    // Add from 9 AM on the day of the next appointment to the start of the next appointment
                    var x = (nextStart.Date).AddHours(9);
                    if (nextStart.Subtract(x) >= duration)
                    {
                        availableSlots.Add((nextStart.Date.AddHours(9), nextStart));
                    }
                }
            }

            // Condition 4: From the end of the last appointment until intervalEnd
            if (sortedAppointments.Count > 0)
            {
                var lastEnd = sortedAppointments.Last().AppointmentDate.Add(sortedAppointments.Last().EndTime);

                // Handle last appointment to intervalEnd
                if (lastEnd.Date == intervalEnd.Date)
                {
                    var lastEndWorkDay = intervalEnd.Date.AddHours(16);
                    if (lastEndWorkDay.Subtract(lastEnd) >= duration)
                    {
                        availableSlots.Add((lastEnd, lastEndWorkDay));
                    }
                }
                else
                {
                    // Add from the last appointment to 4 PM on the same day
                    if ((lastEnd.Date).AddHours(16).Subtract(lastEnd) >= duration)
                    {
                        availableSlots.Add((lastEnd, lastEnd.Date.AddHours(16)));
                    }

                    // Loop through full days between the last appointment and intervalEnd
                    for (var date = lastEnd.AddDays(1).Date; date < intervalEnd.Date; date = date.AddDays(1))
                    {
                        var dayStart = date.AddHours(9);
                        var dayEnd = date.AddHours(16);

                        if (dayEnd.Subtract(dayStart) >= duration)
                        {
                            availableSlots.Add((dayStart, dayEnd));
                        }
                    }

                    // Add from 9 AM on intervalEnd day to intervalEnd
                    var intervalEndStart = intervalEnd.Date.AddHours(9);
                    if (intervalEnd.Subtract(intervalEndStart) >= duration)
                    {
                        availableSlots.Add((intervalEndStart, intervalEnd));
                    }
                }
            }

            return availableSlots;
        }

        // Create Meeting
        public async Task<bool> CreateMeetingAsync(MeetingCreateDTO dto)
        {
            var isAvailable = await _scheduleRepository.CheckAvailabilityForEmployees(dto.EmployeeIds, dto.StartDate, dto.EndDate);

            if (!isAvailable) return false;

            var meeting = new Meeting
            {
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                VirtualMeeting = dto.VirtualMeeting
            };
            // Create meeting participants
            foreach (var employeeId in dto.EmployeeIds)
            {
                var meetingParticipant = new MeetingParticipant
                {
                    Meeting = meeting,  // Link to the meeting being created
                    EmployeeID = employeeId
                };

                meeting.MeetingParticipants.Add(meetingParticipant);
            }

            await _meetingRepository.AddAsync(meeting);

            // Create appointments in all employee schedules
            foreach (var employeeId in dto.EmployeeIds)
            {
                var appointment = new Schedule
                {
                    EmployeeID = employeeId,
                    AppointmentDate = dto.StartDate.Date,
                    StartTime = dto.StartDate.TimeOfDay,
                    EndTime = dto.EndDate.TimeOfDay,
                    Description = dto.Description,
                    ScheduleStatusId=(int)ScheduleStatues.Meeting
                };
                await _scheduleRepository.AddAppointmentAsync(appointment);
            }

            return true;
        }

        // Add employee to an existing meeting
        public async Task<bool> AddEmployeeToMeetingAsync(int meetingId, int employeeId)
        {
            var meeting = await _meetingRepository.GetByIdAsync(meetingId);
            if (meeting == null) return false;

            // Check if employee is available in the given time slot
            var isAvailable = await _scheduleRepository.CheckAvailabilityForEmployee(employeeId, meeting.StartDate, meeting.EndDate);

            if (!isAvailable) return false;

            // Add appointment to employee schedule
            var appointment = new Schedule
            {
                EmployeeID = employeeId,
                AppointmentDate = meeting.StartDate.Date,
                StartTime = meeting.StartDate.TimeOfDay,
                EndTime = meeting.EndDate.TimeOfDay,
                Description = "Meeting"
            };
            await _scheduleRepository.AddAppointmentAsync(appointment);

            // Add employee to meeting
            var participant = new MeetingParticipant { MeetingID = meetingId, EmployeeID = employeeId };
            await _meetingRepository.AddParticipantAsync(participant);

            return true;
        }

        // Remove employee from meeting
        public async Task RemoveEmployeeFromMeetingAsync(int meetingId, int employeeId)
        {
            await _meetingRepository.RemoveParticipantAsync(meetingId, employeeId);
            await _scheduleRepository.RemoveAppointmentForMeetingAsync(meetingId, employeeId);
        }

        // Cancel Meeting
        public async Task CancelMeetingAsync(int meetingId)
        {
            await _meetingRepository.DeleteAsync(meetingId);
            await _scheduleRepository.DeleteAppointmentsForMeetingAsync(meetingId);
        }

        // Get all meetings
        public async Task<List<MeetingDTO>> GetAllMeetingsAsync()
        {
            return await _meetingRepository.GetAllMeetingsAsync();
        }

        // Get a specific meeting
        public async Task<Meeting> GetMeetingByIdAsync(int meetingId)
        {
            return await _meetingRepository.GetByIdAsync(meetingId);
        }

        public async Task RemoveEmpsGetVacation(VacationRequest vs)
        {
            var apps =await _meetingRepository.GetMeetingParticipantAsync(vs.EmployeeID);
            foreach (var app in apps)
            {
                if((vs.StartDateOfVacation<=app.Meeting.StartDate && vs.EndDateOfVacation>=app.Meeting.StartDate)
                    ||(vs.StartDateOfVacation<=app.Meeting.EndDate && vs.EndDateOfVacation >= app.Meeting.EndDate))
                {
                    app.IsOff = true;
                  await  _meetingRepository.UpdateMeetingParticpantAsync(app);
                }
            }
        }
    }

}
