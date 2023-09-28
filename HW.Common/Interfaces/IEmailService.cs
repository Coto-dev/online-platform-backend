namespace HW.Common.Interfaces; 

public interface IEmailService {
  public Task SendEmailAsync(string email, string subject, string message);
  public Task CheckIfMailExist(string subject);

}