namespace Vacation_Service.DTOs
{
    public class VacationRequestCreateDTO
    {
        public int EmployeeID { get; set; }
        public DateTime StartDateOfVacation { get; set; }
        public DateTime EndDateOfVacation { get; set; }
        public string Reason { get; set; }

    }
}
