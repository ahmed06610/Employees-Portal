namespace Vacation_Service.DTOs
{
    public class VacationRequestUpdateDTO
    {
        public DateTime StartDateOfVacation { get; set; }
        public DateTime EndDateOfVacation { get; set; }
        public int RequestID { get; set; }
        public string Reason { get; set; }
    }
}
