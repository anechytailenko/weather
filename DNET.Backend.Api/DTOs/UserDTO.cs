using DNET.Backend.DataAccess.Domain;

namespace DNET.Backend.Api.DTOs;

public class UserDTO
{
    public int Id { get; set; }
    public string Email { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }

    public string? LoginProvider { get; set; }
    public string Role { get; set; }

    public UserDTO(int id, string email, string firstName, string lastName,string role,string? loginProvider = null)

    {
        Id = id;
        Email = email;
        FirstName = firstName;
        LastName = lastName;

        LoginProvider = loginProvider;

        Role = role;

    }

    public UserDTO(UserEntity user)
    {   
        Id = user.Id;
        Email = user.Email;
        FirstName = user.FirstName;
        LastName = user.LastName;
        Role = user.Role;
    }
}