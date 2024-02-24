using System.Runtime.Serialization;

namespace MOVIE_API.Models.Enum
{
    public enum MovieState
    {
        [EnumMember(Value = "Available")]
        Available = 1,

        [EnumMember(Value = "Reserved")]
        Reserved = 2,

        [EnumMember(Value = "NotAvailable")]
        NotAvailable = 3,

    }
}
