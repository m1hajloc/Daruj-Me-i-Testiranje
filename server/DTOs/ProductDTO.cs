using Models;

namespace DTOs
{
    public class CreateProductDTO
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? ProfilePicture { get; set; }
        public string? ProductTypeId { get; set; }
        public string? OwnerId { get; set; }
        public CreateProductDTO(string name, string desc, string pp, string productTypeId, string ownerId)
        {
            Name=name;
            Description=desc;
            ProfilePicture=pp;
            ProductTypeId=productTypeId;
            OwnerId=ownerId;
        }
        public CreateProductDTO()
        {
            
        }
    }

    public class ChangeStateDTO
    {
        public string? Id {get; set;}
        public Boolean isSent {get; set;}
    }
}