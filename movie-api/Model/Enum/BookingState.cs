using System.Runtime.Serialization;

namespace movie_api.Model.Enum
{
    public enum BookingState
    {
        [EnumMember(Value = "Available")]
        Available = 1,

        [EnumMember(Value = "Returned")]
        Returned = 2,

        [EnumMember(Value = "Pending")]
        Pending = 3,

        [EnumMember(Value = "Error")]
        Error = 4,

        [EnumMember(Value = "Canceled")]
        Canceled = 5,


    }
}
