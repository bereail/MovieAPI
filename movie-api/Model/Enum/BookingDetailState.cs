using System.Runtime.Serialization;

namespace MOVIE_API.Models.Enum
{
    public enum BookingDetailState
    {
        [EnumMember(Value = "Avaiable")]
        Available = 1,

        [EnumMember(Value = "Pending")]
        Pending = 2,

        [EnumMember(Value
            = "Returned")]
        Returned = 3,

        [EnumMember(Value = "Canceled")]
        Canceled = 4,

    }
}
