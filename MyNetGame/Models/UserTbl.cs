namespace MyNetGame.Models
{
    public class UserTbl
    {
        public string? user_id { get; set; }
        public string? name { get; set; }
        public string? password { get; set; }
        public string? mobile { get; set; }
        public short? level { get; set; }
        public int? health { get; set; }
        public int? gold { get; set; }
        public DateTime? m_date { get; set; }
    }
}
