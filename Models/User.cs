using Microsoft.EntityFrameworkCore;

namespace ReservationSystem.Models 
{
  public class User 
  {
    public int ID {get; set;} = 0;
    public string Username {get ; set ;} = ""; 
    public string IP {get ; set; } = ""; 
    public string Password {get; set;} = ""; 
    public string Email {get; set;} = ""; 

  }
}
