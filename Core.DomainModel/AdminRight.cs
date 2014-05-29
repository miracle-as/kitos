﻿namespace Core.DomainModel
{
    public class AdminRight : Entity, IRight<Organization, AdminRole>
    {
        public int UserId { get; set; }
        public int RoleId { get; set; }
        public int ObjectId { get; set; }
        public virtual User User { get; set; }
        public virtual AdminRole Role { get; set; }
        public virtual Organization Object { get; set; }
    }
}
